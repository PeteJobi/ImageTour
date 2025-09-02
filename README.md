![input_TOURED](https://github.com/user-attachments/assets/7ad98053-2871-4ea1-9724-9593361cf775)

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
