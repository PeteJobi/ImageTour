using System.Text.RegularExpressions;
using static ImageTour.ImageTour;

string? ffmpegPath = null;
var inputPath = string.Empty;
var outputPath = string.Empty;
var width = 0;
var height = 0;
double fps = 24;
var dontDeleteFrames = false;
var transitions = Array.Empty<Transition>();

var arguments = Environment.GetCommandLineArgs();
if (arguments.Length < 2)
{
    Console.WriteLine("Please enter the parameters below:");
    Console.Write("Input filename (the path to the image you wish to tour): ");
    inputPath = Console.ReadLine() ?? Invalid(nameof(inputPath));
    Console.Write("Output filename (the path where the resulting video should be saved): ");
    outputPath = Console.ReadLine() ?? Invalid(nameof(outputPath));
    Console.Write("Output width (the width of the resulting video): ");
    width = int.Parse(Console.ReadLine() ?? Invalid(nameof(width)));
    Console.Write("Output height (the height of the resulting video): ");
    height = int.Parse(Console.ReadLine() ?? Invalid(nameof(height)));
    Console.Write("Output fps (the frames per seconds of the resulting video): ");
    var fpsInput = Console.ReadLine();
    if(!string.IsNullOrWhiteSpace(fpsInput)) fps = double.Parse(fpsInput);
    Console.Write("FFMPEG path (the path to the FFMPEG executable): ");
    ffmpegPath = Console.ReadLine();
    Console.Write("Don't delete generated frames? (do you want to keep frames generated during the process? [y/n]): ");
    var dontInput = Console.ReadLine();
    dontDeleteFrames = !string.IsNullOrWhiteSpace(dontInput) && dontInput.ToLower() == "y";
    Console.Write("Transition steps (the points in the image that should be toured in the resulting video." +
                  " Each transition should be in the format [(x1, y1), (x2, y2), s]. Multiple transitions should be separated by |): ");
    transitions = ProcessTransitionString(Console.ReadLine() ?? Invalid(nameof(transitions)));
}
else
{
    for (var i = 1; i < arguments.Length; i++)
    {
        var argument = arguments[i];
        switch (argument)
        {
            case "-i": inputPath = arguments[i + 1];
                i++; break;
            case "-o": outputPath = arguments[i + 1];
                i++; break;
            case "-w": width = int.Parse(arguments[i + 1]);
                i++; break;
            case "-h": height = int.Parse(arguments[i + 1]);
                i++; break;
            case "-t": transitions = ProcessTransitionString(arguments[i + 1]);
                i++; break;
            case "-fps": fps = double.Parse(arguments[i + 1]);
                i++; break;
            case "-ffmpegPath": ffmpegPath = arguments[i + 1];
                i++; break;
            case "-dontDelete": dontDeleteFrames = true;
                break;
        }
    }
}

var nowMerging = false;
Console.WriteLine();
var imageTour = new ImageTour.ImageTour(ffmpegPath);
var payload = await imageTour.Animate(inputPath, outputPath, width, height, fps, transitions, ProgressCallback, dontDeleteFrames);
Console.WriteLine(payload.Success ? $"Done! {payload.FramesGenerated} frames generated" : payload.ErrorMessage);

void ProgressCallback(Progress progress)
{
    if (!nowMerging && progress.CurrentStage > transitions.Length)
    {
        nowMerging = true;
        Console.WriteLine();
    }
    Console.Write("\r");
    Console.Write($"{(nowMerging ? "Merging video" : "Generating frames")}: {progress.CurrentFrame}/{progress.TotalFrames}{new string(' ', 10)}");
}

Transition[] ProcessTransitionString(string transitionString)
{
    var transitionStrings = transitionString.Split('|');
    return transitionStrings.Select(s =>
    {
        var matchCollection = Regex.Matches(Regex.Replace(s, "\\s", string.Empty), @"\[\((\d+),(\d+)\),\((\d+),(\d+)\),(\d+)\]");
        if (!matchCollection.Any()) _ = Invalid(nameof(transitions));
        return new Transition
        {
            StartPosition = new Position { X = ParseMatch(matchCollection, 1), Y = ParseMatch(matchCollection, 2) },
            EndPosition = new Position { X = ParseMatch(matchCollection, 3), Y = ParseMatch(matchCollection, 4) },
            Duration = ParseMatch(matchCollection, 5)
        };
    }).ToArray();
}

string Invalid(string argument) => throw new InvalidOperationException($"Argument invalid: {argument}");
int ParseMatch(MatchCollection collection, int capture) => int.Parse(collection[0].Groups[capture].Value);
