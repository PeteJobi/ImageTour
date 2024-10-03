// See https://aka.ms/new-console-template for more information
using static ImageTour.ImageTour;

const string ffmpegPath = "ffmpeg.exe";
string inputPath = "input.png";
string outputPath = "output.mp4";
int width = 496;
int height = 726;
double fps = 10;
var transitions = new Transition[]
{
    //new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 0 }, Duration = 1 },
    new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 10 },
    //new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 1 },
    //new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 10 },
    //new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 1 },
    //new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 10 },
    //new() { StartPosition = new Position { X = 653, Y = 665 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 2 }
};

var imageTour = new ImageTour.ImageTour(ffmpegPath);
//await imageTour.Animate(inputPath, outputPath, width, height, fps, transitions);
await imageTour.Animate(@"C:\Users\PeterEgunjobi\Pictures\Screenshots\Screenshot (243).png", @"C:\Users\PeterEgunjobi\Pictures\video.mp4", 572, 818, 60, [
    new() { StartPosition = new Position { X = 372, Y = 0 }, EndPosition = new Position { X = 372, Y = 0 }, Duration = 1 },
    new() { StartPosition = new Position { X = 372, Y = 0 }, EndPosition = new Position { X = 372, Y = 1742 }, Duration = 15 },
    new() { StartPosition = new Position { X = 372, Y = 1742 }, EndPosition = new Position { X = 372, Y = 1742 }, Duration = 2 }
], progress =>
{
    Console.WriteLine($"Stage: {progress.CurrentStage}, Progress: {progress.CurrentFrame}/{progress.TotalFrames}");
});
Console.WriteLine("Done");