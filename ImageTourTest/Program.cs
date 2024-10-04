using static ImageTour.ImageTour;

string inputPath = "input.png";
string outputPath = "output.mp4";
int width = 1256;
int height = 846;
double fps = 24;
var transitions = new Transition[]
{
    new() { StartPosition = new Position { X = 0, Y = 0 }, EndPosition = new Position { X = 0, Y = 410 }, Duration = 2 },
    new() { StartPosition = new Position { X = 0, Y = 410 }, EndPosition = new Position { X = 1836, Y = 410 }, Duration = 5 },
    new() { StartPosition = new Position { X = 1836, Y = 410 }, EndPosition = new Position { X = 1836, Y = 0 }, Duration = 2 },
    new() { StartPosition = new Position { X = 1836, Y = 0 }, EndPosition = new Position { X = 0, Y = 0 }, Duration = 5 },
};

bool nowMerging = false;
Console.WriteLine();
var imageTour = new ImageTour.ImageTour();
var payload = await imageTour.Animate(inputPath, outputPath, width, height, fps, transitions, ProgressCallback);
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

//To run the program using the CLI, you can run the command below
//-i input.png -o output.mp4 -w 1256 -h 846 -fps 24 -t "[(0, 0), (0, 410), 2]|[(0, 410), (1836, 410), 5]|[(1836, 410), (1836, 0), 2]|[(1836, 0), (0, 0), 5]"