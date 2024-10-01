// See https://aka.ms/new-console-template for more information
using static ImageTour.ImageTour;

const string ffmpegPath = "ffmpeg.exe";
string inputPath = "input.png";
string outputPath = "output.mp4";
int width = 496;
int height = 726;
double fps = 60;
var transitions = new Transition[]
{
    new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 0 }, Duration = 1 },
    new() { StartPosition = new Position { X = 158, Y = 0 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 10 },
    new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 158, Y = 665 }, Duration = 1 },
    new() { StartPosition = new Position { X = 158, Y = 665 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 10 },
    new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 0 }, Duration = 1 },
    new() { StartPosition = new Position { X = 653, Y = 0 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 10 },
    new() { StartPosition = new Position { X = 653, Y = 665 }, EndPosition = new Position { X = 653, Y = 665 }, Duration = 2 }
};

var imageTour = new ImageTour.ImageTour(ffmpegPath);
await imageTour.Animate(inputPath, outputPath, width, height, fps, transitions);