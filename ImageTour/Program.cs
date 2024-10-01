// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text.RegularExpressions;

const string ffmpegPath = "ffmpeg.exe";
string inputPath = @"C:\Users\PeterEgunjobi\source\repos\ImageTour\ImageTour\input.png";
string outputPath = @"C:\Users\PeterEgunjobi\source\repos\ImageTour\ImageTour\output.mp4";
int width = 496;
int height = 726;
int totalWidth = 0, totalHeight = 0;
int fps = 60;
Process? currentProcess;
bool hasBeenKilled = false;
string folder;

//int x = 3, y = 665;
//for (int i = 1; i <= 150; i++)
//{

//    Console.WriteLine($"{x1}  {x2}");
//}
await Setup();

Console.WriteLine("Hello, World!");


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

    var transitions = new Transition[]
    {
        new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 0 }, Duration = 1 },
        new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 10 },
        new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 1 },
        new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 10 },
        new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 1 },
        new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 10 },
        new() { StartPosition = new Position { X = 653, Y = 665 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 2 },
        //new() { StartPosition = new Position { X = 0, Y = 665 }, EndPosition = new Position { X = 0, Y = 665 }, Duration = 2 }
    };

    var lastPosition = transitions.First().StartPosition;
    var lastFrame = 1;
    await GenerateFrame(lastPosition.X, lastPosition.Y, lastFrame); //Generate first frame

    foreach (var transition in transitions)
    {
        if(!Equals(transition.StartPosition, lastPosition))
            throw new InvalidOperationException("Current transition does not flow from last transition");

        if (Equals(transition.StartPosition, transition.EndPosition))
        {
            lastFrame += ProcessStillFrames(transition, lastFrame);
        }
        else
        {
            lastFrame += await ProcessTransitionFrames(transition, lastFrame);
        }
        lastPosition = transition.EndPosition;
    }

    File.Delete(outputPath);
    var x = $"-r {fps} -i \"{folder}/frame%08d.png\" -c:v libx264 -vf scale=out_color_matrix=bt709,format=yuv420p \"{outputPath}\"";
    await StartProcess(ffmpegPath, x, (a, b) => { Console.WriteLine(b.Data); }, (sender, args) =>
    {
        if (string.IsNullOrWhiteSpace(args.Data) || hasBeenKilled) return;
        //if (CheckNoSpaceDuringBreakMerge(args.Data)) return;
        MatchCollection matchCollection = Regex.Matches(args.Data, @"^frame=\s*\d+\s.+?time=(\d{2}:\d{2}:\d{2}\.\d{2}).+");
        if (matchCollection.Count == 0) return;
        //Console.WriteLine(TimeSpan.Parse(matchCollection[0].Groups[1].Value));
        Console.WriteLine(args.Data);
    });
}

int ProcessStillFrames(Transition transition, int totalFramesSoFar)
{
    var totalFramesExcludingFirst = fps * transition.Duration - 1;
    CopyFrame(totalFramesSoFar, totalFramesExcludingFirst);
    return totalFramesExcludingFirst;
}

async Task<int> ProcessTransitionFrames(Transition transition, int totalFramesSoFar)
{
    var totalFramesExcludingFirst = fps * transition.Duration - 1;

    var xDiff = transition.EndPosition.X - transition.StartPosition.X;
    var yDiff = transition.EndPosition.Y - transition.StartPosition.Y;
    var lastShift = new Position { X = 0, Y = 0 };

    for (var i = 1; i <= totalFramesExcludingFirst; i++)
    {
        var xShift = Convert.ToInt32(i * xDiff / (double)totalFramesExcludingFirst);
        var yShift = Convert.ToInt32(i * yDiff / (double)totalFramesExcludingFirst);
        if (xShift == lastShift.X && yShift == lastShift.Y) CopyFrame(totalFramesSoFar + i - 1, 1);
        else await GenerateFrame(transition.StartPosition.X + xShift, transition.StartPosition.Y + yShift, totalFramesSoFar + i);
        lastShift.X = xShift;
        lastShift.Y = yShift;
    }

    return totalFramesExcludingFirst;
}

async Task GenerateFrame(int x, int y, int frameNumber)
{
    await StartProcess(ffmpegPath, $"-i \"{inputPath}\" -filter:v  \"crop={width}:{height}:{x}:{y}\" \"{folder}/frame{frameNumber:D8}.png\"", null, (sender, args) =>
    {
        if (string.IsNullOrWhiteSpace(args.Data) || hasBeenKilled) Console.WriteLine("N");
        else Console.WriteLine(y);
    });
}

void CopyFrame(int frameNumber, int amountOfCopies)
{
    var framePath = $"{folder}/frame{frameNumber:D8}.png";
    for (var j = 1; j <= amountOfCopies; j++)
    {
        File.Copy(framePath, $"{folder}/frame{frameNumber + j:D8}.png");
    }
}

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

struct Position
{
    public int X { get; set; }
    public int Y { get; set; }
}

class Transition
{
    public Position StartPosition { get; set; }
    public Position EndPosition { get; set; }
    public int Duration { get; set; }
}
