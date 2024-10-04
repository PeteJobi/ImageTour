![ImageTour](https://www.dropbox.com/scl/fi/ld4fzpdtvx8ufbl7qsxg0/ImageTourGithubImage.gif?rlkey=8tdq447nqqmghdo3xygwdpt71&st=92zwpht0&raw=1)

Using **ImageTour**, you can create a video that "tours" around a large-resolution image, especially for cases where said image is too huge to share. You control the movement by setting multiple points in the image, and the "camera" (a window smaller than the image) moves between these points. You also control the duration of each transition, and the frames per second (fps) of the output video.</br>
Only supports Windows 10 and 11 (not tested on other versions of Windows).


## How to build
You need to have at least .NET 6 runtime installed to build the software. Download the latest runtime [here](https://dotnet.microsoft.com/en-us/download). If you're not sure which one to download, try [.NET 6.0 Version 6.0.16](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-6.0.408-windows-x64-installer)

In the project folder, run the below
```
dotnet publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false
```
When that completes, go to `\bin\Release\net<version>-windows\win-x64\publish` and you'll find the **ImageTour.exe**.

## Run without building
You can also just download the release builds if you don't wish to build manually. The assets release contains the assets used by the software, which is just the ffmpeg executable. If you already have this on your system (or you'd rather get it from elsewhere), you won't need to download this. The main release contains the compiled executable.

If you wish to run the software without installing the required .NET runtime, download the self-contained release.

## How to use
You can either run the program using the console or the command line interface. The below images show them respectively.
![Screenshot (246)](https://github.com/user-attachments/assets/28ecf2c7-20b7-4c38-8b39-e645af175c99) ![Screenshot (245)](https://github.com/user-attachments/assets/5380c77c-41a2-433a-8867-29155d53139d)
If all the parameters are entered correctly and are valid, the program starts the process of generating the video. First, the frames are interpolated depending on your parameters and are created using ffmpeg's crop tool. Then the frames are merged into a video.

## Parameters:
The command-line equivalents are shown in brackets.
- **Input filename (-i)**: This is the path to the image you want the program to tour. It can be the full absolute path or a path relative to the path of the program.
- **Output filename (-o)**: This is the path where the resulting video should be saved to. It can be the full absolute path or a path relative to the path of the program. The file doesn't have to exist, and if it does, it is overwritten.
- **Output width (-w)**: This is the width of the resulting video. It must be a non-zero integer divisible by 2 and smaller than the width of the input image. Together with the Output height, they represent the dimensions of the window that moves around the input image.
- **Output height (-h)**: This is the height of the resulting video. It must be a non-zero integer divisible by 2 and smaller than the height of the input image. Together with the Output width, they represent the dimensions of the window that moves around the input image.
- **Output fps (-fps)**: This is the fps you want to resulting video to run at. This should be a non-zero integer/double. A higher value means the video will run smoother, but it also means more frames will have to be generated and that means the program will take longer to generate the video. If you don't specify this parameter, 24 is used.
- **FFMPEG path (-ffmpegPath)**: This is the path to the ffmpeg executable. It can be the full absolute path or a path relative to the path of the program. If you don't specify this parameter, it is assumed the ffmpeg executable is in the same directory as the program.
- **Don't delete generated frames (-dontDelete)**: By default, the frames generated during the process are deleted after the process completes. If you want to keep the frames, specify this parameter. If you're using the CLI, you don't need to give a value to this parameter.
- **Transition steps (-t)**: These define the data that the program uses to generate the frames that create the video. A single transition steps is defined in the format **[(x1, y1), (x2, y2), s]**. Multiple steps are separated by a vertical bar "**|**". The parameters of the format are explained below:
  -  **(x1, y1)**: These are the coordinates of the starting position for the transition. x1 and y1 should not be less than 0, x1 shouldn't be greater than the width of the input image and y1 shouldn't be greater than the height of the input image.
  -  **(x2, y1)**: These are the coordinates of the ending position for the transition. The same rules for the starting position apply. The ending position can be the same as the starting position and you can use that to create a _still_ in the video.
  -  **s**: This is the duration the transition should last for.
  
  Note that the top-left corner of the input image is (0, 0) and the bottom-right corner is (_inputImageWidth_, _inputImageHeight_). The starting position of a transition must be the same as the ending position of the previous transition, if the transition is not the first.
  As an example, if you have an image that is 30 pixels wide and 100 pixels long, and you want a video 20 pixels wide and long that scrolls from the top-left corner to bottom-right of the image in 5 seconds and linger at the bottom-right for 2 more seconds, the transition steps would be **[(0, 0), (10, 80), 5]|[(10, 80), (10, 80), 2]**.

## Planned features:
- A GUI for the program, which will make it easier to create the transition steps.
- An option to specify amount of frames in a transition step instead of duration. This can give users more control.

