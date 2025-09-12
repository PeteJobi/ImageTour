<p align="center"><img alt="Gif showing the text 'Image Tour' being panned and zoomed across" src="https://github.com/user-attachments/assets/81c2457e-ec15-4056-b392-c31121cd2670" style="display: block; margin: 0 auto" /></p>

Using **ImageTour**, you can create a video that "tours" around a large-resolution image, especially for cases where said image is too huge to share. You control the movement by setting multiple keyframes in the image, and the intermediate frames are interpolated. You also control the duration of each transition, and the width, height and frames per second (fps) of the output video. Video input files are also supported.
Only supports Windows 10 and 11 (not tested on other versions of Windows). Powered by FFMPEG.

<img width="1793" height="1022" alt="image" src="https://github.com/user-attachments/assets/b164adf7-f32f-4d1d-b01b-5671eb4dd357" />

Result of the above tour:

https://github.com/user-attachments/assets/0f1430ee-f72f-44cb-938a-6b2a95373594

## How to build
You need to have at least .NET 9 runtime installed to build the software. Download the latest runtime [here](https://dotnet.microsoft.com/en-us/download). If you're not sure which one to download, try [.NET 9.0 Version 9.0.203](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-9.0.203-windows-x64-installer)

In the project folder, run the below
```
dotnet publish -c Release -p:Platform=x64 -p:WindowsAppSDKSelfContained=true -p:WindowsPackageType=None
```
When that completes, go to `\bin\Release\net<version>-windows\win-x64` and you'll find the **ImageTour.exe**.

## Run without building
You can also just download the release builds if you don't wish to build manually. Unfortunately packages created in WinUI 3 have to be signed with a certificate, and certificates sourced from trusted companies cost hundreds of dollars. If you wish to install the package, you'll have to install a certificate signed by myself, as described [here](https://github.com/PeteJobi/ImageTour/releases/tag/cert). You only need to do this once - future updates will not require different certificates.

## How to use
When you open the program, you will be prompted to upload a media file. Once that is done, you will be taken to the tour interface page. You'll notice a **transition** has been created for you, whose **keyframes** have been centered within the media and are 500 pixels wide and tall, and whose duration is 5 seconds. A transition defines a **Start keyframe** and an **End keyframe**, and the duration of movement between those keyframes. A keyframe is a section in the image that the program moves from or to during a transition, and this section is defined by four properties: **X**, **Y**, **Width** and **Height**. X and Y respectively are the distances from the top-left corner of the keyframe to the left and top edges of the media, while the Width and Height represent respectively how wide and tall in pixels the keyframe is. You are able to drag around and resize keyframes, but keyframes are not allowed to extend beyond the bounds of the media. Keyframes are numbered, starting from 1, and this helps to understan the order and direction of transitions, as well as easily identify them in the transitions list. There are also dashed lines between keyframes that show direction of transitions. When two or more keyframes are within 20 pixels of each other (remember that their top-left corners represent their positions) their number labels are stacked horizontally. This makes it so that their labels are always visible and don't overlap each other.

You can zoom into or out of the media with the mouse wheel. You can also click and drag on an unoccupied area of the media to pan around. Right-clicking on an unoccupied area will provide you with a few options:
- **Add keyframe here**: This will add a new keyframe at the point where you right-clicked. But this is a simplification: what really happens is that a new transition is added, whose End keyframe is positioned at the point where you right-clicked, and whose Start keyframe is merged with the End keyframe of the last transition. Merged keyframes will be explained later. The new keyframe will have the same size as the last keyframe.
- **Add transition here**: This will add a new transition whose Start and End keyframes are positioned at the point where you right-clicked. Both keyframes will have the same size as the last keyframe.
- **Fit to view**: This will adjust the zoom to fit the media within the view, and the pan to center it within the view. Useful if you zoomed so far in and want to see everything at once without having to zoom back out.

Right-clicking on an unobstructed keyframe will provide you a bunch of options depending on the state of the keyframe:
- **Move to**: Use this menu to position the keyframe at the **top center**, **bottom center**, **left center**, **right center** or the very **center** of the media.
- **Copy frame position**: If there are keyframes present that are not within 20 pixels of this keyframe, they will be listed in this submenu. When you select a keyframe from the list, the position (X and Y) of the keyframe you right-clicked on will be set to the position of the keyframe you selected from the list.
- **Swap frame position**: Same as above, except that the positions of the keyframe that owns the menu and the keyframe you selected from the menu will be swapped.
- **Copy frame size**: If there are keyframes present that do not have the same size as this keyframe, they will be listed in this submenu. When you select a keyframe from the list, the size (Width and Height) of the keyframe you right-clicked on will be set to the size of the keyframe you selected from the list.
- **Swap frame size**: Same as above, except that the sizes of the keyframe that owns the menu and the keyframe you selected from the menu will be swapped.
- **Output size**: If the size of this keyframe and the size set for the output video do not match, you have options to **set the keyframe's size** to that of the output, or **set the output size** to that of the keyframe.
- **Merge with frame [B]**: Using this option, you can merge a keyframe with another adjacent keyframe from another transition, i.e, you can merge keyframe 2 with keyframe 3 or keyframe 5 with keyframe 4. You cannot merge keyframes 6 and 7 because they're in the same transition, and you cannot merge keyframes 7 and 9 because they're not adjacent. When you merge 2 keyframes, you can drag and resize them as one, i.e their X, Y, Width and Height properties will always match. This is useful for making connected/seamless transitions, i.e transitions that begin where the previous one ends. A merged keyframe will have a label showing the numbers of the two keyframes, such as '2,3'.
- **Separate frames [A] and [B]**: If this keyframe represents 2 merged keyframes, this option allows you to separate them. This allows each keyframe to be updated independently, and will disconnect the two transitions that own the keyframes, unless you keep both keyframes' properties in sync.
- **Add here**: This submenu provides options to **add keyframes or transitions** at the same position and with the same size of this keyframe.
- **Delete frames [A] and [B]**: Use this to delete this keyframe and the other keyframe in this transition (this essentially deletes the transition). This option is only available if there is more than 1 transition. After deleting, numbers of other keyframes will be automatically adjusted so that the numbers are contiguous and start from 1.
- **Delete frame [A,B]**: If this is a merged keyframe, you also have the option to delete it to join the other two keyframes that connect to it. For example, if you have keyframes (1), (2,3) and (4), you can delete (2,3) to link (1) and (4) together. After deleting, numbers of other keyframes will be automatically adjusted so that the numbers are contiguous and start from 1.

If the input is a video file, a simple video control panel is available at the bottom of the view to control playback of the video.

On the right side, there's the **Output Width** and **Output Height** textboxes. Use these to specify the resolution of the resulting video. The default is 500p, if the media is as big as that. The size of individual keyframes can be bigger or smaller than the output resolution - frames that are of a different resolution from the output will be scaled up or down to fit. There's also the **Output Frame Rate**, which you can use to specify the fps (frames per second) of the resulting video. Note that if the input is a video, the real frame rate of the video will not change. Only the panning and zooming transitions will reflect the specified frame rate. The higher the frame rate, the more the frames generated, and the longer it will take to process.

By default, the aspect ratio of keyframes is maintained when you resize them. Untick the **Lock Aspect Ratio** checkbox to resize keyframes freely.

The list of transitions come after. Each item contains controls for the Start keyframe, the duration and the End keyframe. For each keyframe, there are 4 numberboxes you can use to control the properties of the keyframe: **X**, **Y**, **Width** and **Height**, in order of top-left to bottom-right. Changes to any of these will reflect in the view. When the aspect ratio of a keyframe does not match that of the output resolution, the width and height numberboxes will have a yellow colour to warn you. This won't prevent you from touring, but note that any frame with an incorrect aspect ratio will be stretched or compressed to fit in the output video. 

Between the controls for the keyframes, there's a timespan box you can use to specify the **duration** of the transition. The default is 5 seconds, but can be changed to any value, and supports fractional seconds. The format expected is **_hh:mm:ss:fff_**. The longer the duration, the more the frames generated for the transition, and the longer it will take to process that transition.

Next to the controls for each keyframe, there are numeric labels. You can right-click them for options to perform operations on a keyframe:
- **Bring to top**: If a keyframe you want to drag or right-click on is obstructed by another keyframe, you can use this option to bring them to the top, over all the other keyframes.
- **Center in view**: Clicking this option will zoom into the keyframe and center it in the view. Useful if you've zoomed far in or out and need to quickly focus on a keyframe.
- **Highlight**: Toggling this option will switch the colour of the keyframe's border and label between white and a different colour. Useful to mark a keyframe amongst many other keyframes.
- **Hide**: Toggling this option will hide the keyframe from the view or show it. Useful if you want to click something underneath the keyframe.

When you're done setting up the keyframes, click the **Tour!** button to start the tour process. A section will pop up at the bottom of the view with progressbars showing you the progress, and buttons allowing you to **pause** or **cancel** the process. When the process is complete, you can **close** this section or **view** the output video. This video will have no audio, even if the input media is a video with an audio track. The video is created in the same folder as the input media and it will have the same file name with "__TOURED_" appended to it. Be warned that if such a file exists, it will be deleted. Frames are generated during the process and deleted when it's done. If, for whatever reason, you want to keep those frames, tick the **Don't delete generated frames** checkbox before clicking the Tour! button.

Click the **Go back** button to return to the file selection page.
