using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ImageTour
{
    public class ImageTour
    {
        string ffmpegPath;
        private string folder;
        private string inputPath;
        private int width;
        private int height;
        private int totalWidth;
        private int totalHeight;
        private string outputPath;
        private double fps;
        private int totalFrames;
        private int currentStage;
        private Transition[] transitions;
        private Action<Progress>? progress;
        //private bool dontDeleteFrames;
        private bool hasBeenKilled;
        private Process currentProcess;

        public ImageTour(string ffmpegPath)
        {
            this.ffmpegPath = ffmpegPath;
        }

        public async Task<Payload> Animate(string inputFileName, string outputFileName, int outputWidth, int outputHeight, double outputFps, IEnumerable<Transition> transitionSteps, Action<Progress>? progress = default, bool dontDeleteGeneratedFrames = false)
        {
            inputPath = inputFileName;
            outputPath = outputFileName;
            width = outputWidth;
            height = outputHeight;
            fps = outputFps;
            transitions = transitionSteps.ToArray();
            this.progress = progress;
            //dontDeleteFrames = dontDeleteGeneratedFrames;

            if (!transitions.Any())
            {
                return new Payload
                {
                    ErrorMessage = "No transitions specified"
                };
            }

            await Setup();

            if (outputWidth > totalWidth || outputHeight > totalHeight)
            {
                return new Payload
                {
                    ErrorMessage = "One or more of the output dimensions is greater than the corresponding dimension of the input image"
                };
            }

            var lastPosition = transitions.First().StartPosition;
            var lastFrame = 1;
            await GenerateFrame(lastPosition.X, lastPosition.Y, lastFrame); //Generate first frame
            RecordProgress(1);

            try
            {
                foreach (var transition in transitions)
                {
                    if (!Equals(transition.StartPosition, lastPosition))
                        throw new InvalidOperationException("Current transition does not flow from last transition");

                    lastFrame += await ProcessTransitionFrames(transition, lastFrame);
                    lastPosition = transition.EndPosition;
                    currentStage++;
                }
            }
            catch (Exception e)
            {
                if(!dontDeleteGeneratedFrames) Directory.Delete(folder, true);
                return new Payload
                {
                    ErrorMessage = $"An error occurred during frame generation: {e}"
                };
            }

            try
            {
                File.Delete(outputPath);
                var x = $"-r {fps} -i \"{folder}/frame%08d.png\" -c:v libx264 -vf scale=out_color_matrix=bt709,format=yuv420p \"{outputPath}\"";
                await StartProcess(ffmpegPath, x, (a, b) => { Console.WriteLine(b.Data); }, (sender, args) =>
                {
                    if (string.IsNullOrWhiteSpace(args.Data) || hasBeenKilled) return;
                    var matchCollection = Regex.Matches(args.Data, @"^frame=\s*(\d+).+");
                    if (matchCollection.Count == 0) return;
                    RecordProgress(int.Parse(matchCollection[0].Groups[1].Value));
                });
            }
            catch (Exception e)
            {
                if (!dontDeleteGeneratedFrames) Directory.Delete(folder, true);
                return new Payload
                {
                    ErrorMessage = $"An error occurred during video creation: {e}"
                };
            }
            if (!dontDeleteGeneratedFrames) Directory.Delete(folder, true);

            return new Payload
            {
                Success = true,
                FramesGenerated = lastFrame
            };
        }

        string GetOutputFolder(string path)
        {
            string inputName = Path.GetFileNameWithoutExtension(path);
            string parentFolder = Path.GetDirectoryName(path) ?? throw new NullReferenceException("The specified path is null");
            string outputFolder = Path.Combine(parentFolder, $"{inputName}_Frames");
            //pathForView = outputFolder;

            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
            }
            Directory.CreateDirectory(outputFolder);

            //filesCreated = new[] { outputFolder };
            return outputFolder;
        }

        async Task Setup()
        {
            folder = GetOutputFolder(inputPath);

            await StartProcess(ffmpegPath, $"-i \"{inputPath}\"", null, (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(args.Data)) return;
                if (totalWidth + totalHeight == 0)
                {
                    MatchCollection matchCollection = Regex.Matches(args.Data, @"\s*Stream #0:0: .+, (\d+)x(\d+).+");
                    if (matchCollection.Count == 0) return;
                    totalWidth = int.Parse(matchCollection[0].Groups[1].Value);
                    totalHeight = int.Parse(matchCollection[0].Groups[2].Value);
                }
            });

            totalFrames = 1 + transitions.Sum(GetTransitionFrameCount);
            currentStage = 1;
        }

        int GetTransitionFrameCount(Transition transition) => Convert.ToInt32(fps * transition.Duration) - 1;

        async Task<int> ProcessTransitionFrames(Transition transition, int totalFramesSoFar)
        {
            var totalFramesExcludingFirst = GetTransitionFrameCount(transition);

            var xDiff = transition.EndPosition.X - transition.StartPosition.X;
            var yDiff = transition.EndPosition.Y - transition.StartPosition.Y;
            var lastShift = new Position { X = 0, Y = 0 };

            for (var i = 1; i <= totalFramesExcludingFirst; i++)
            {
                var xShift = Convert.ToInt32(i * xDiff / (double)totalFramesExcludingFirst);
                var yShift = Convert.ToInt32(i * yDiff / (double)totalFramesExcludingFirst);
                if (xShift == lastShift.X && yShift == lastShift.Y) CopyFrame(totalFramesSoFar + i - 1, 1);
                else await GenerateFrame(transition.StartPosition.X + xShift, transition.StartPosition.Y + yShift, totalFramesSoFar + i);
                RecordProgress(totalFramesSoFar + i);
                lastShift.X = xShift;
                lastShift.Y = yShift;
            }

            return totalFramesExcludingFirst;
        }

        async Task GenerateFrame(int x, int y, int frameNumber)
        {
            await StartProcess(ffmpegPath, $"-i \"{inputPath}\" -filter:v  \"crop={width}:{height}:{x}:{y}\" \"{folder}/frame{frameNumber:D8}.png\"", null, null);
            //await StartProcess(ffmpegPath, $"-i \"{inputPath}\" -filter:v  \"crop={width}:{height}:{x}:{y}\" \"{folder}/frame{frameNumber:D8}.png\"", null, (sender, args) =>
            //{
            //    if (string.IsNullOrWhiteSpace(args.Data) || hasBeenKilled) Console.WriteLine("N");
            //    else Console.WriteLine(y);
            //});
        }

        void CopyFrame(int frameNumber, int amountOfCopies)
        {
            var framePath = $"{folder}/frame{frameNumber:D8}.png";
            for (var j = 1; j <= amountOfCopies; j++)
            {
                File.Copy(framePath, $"{folder}/frame{frameNumber + j:D8}.png");
            }
        }

        void RecordProgress(int currentFrame) => progress?.Invoke(new Progress { CurrentFrame = currentFrame, TotalFrames = totalFrames, CurrentStage = currentStage });

        async Task StartProcess(string processFileName, string arguments, DataReceivedEventHandler? outputEventHandler, DataReceivedEventHandler? errorEventHandler)
        {
            Process ffmpeg = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = processFileName,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                },
                EnableRaisingEvents = true
            };
            ffmpeg.OutputDataReceived += outputEventHandler;
            ffmpeg.ErrorDataReceived += errorEventHandler;
            ffmpeg.Start();
            ffmpeg.BeginErrorReadLine();
            ffmpeg.BeginOutputReadLine();
            currentProcess = ffmpeg;
            await ffmpeg.WaitForExitAsync();
            ffmpeg.Dispose();
        }

        public struct Position
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct Transition
        {
            public Position StartPosition { get; set; }
            public Position EndPosition { get; set; }
            public int Duration { get; set; }
        }

        public struct Payload
        {
            public bool Success { get; set; }
            public int FramesGenerated { get; set; }
            public string ErrorMessage { get; set; }
        }

        public struct Progress
        {
            public int CurrentFrame { get; set; }
            public int TotalFrames { get; set; }
            public int CurrentStage { get; set; }
        }
    }
}
