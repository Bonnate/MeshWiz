
# Mesh Wiz 1.0v (23.09.22)

![Main Image](https://blog.kakaocdn.net/dn/bKKSdF/btsvb3NiUxk/bxiklIUqcfVyUkKg91IcLk/img.png)

## Program Features

MeshWiz automatically rearranges Pivot to the Center when files are loaded, offering a range of object editing capabilities:

1. **Pivot Realignment**: Upon loading a file, the Pivot is automatically repositioned to the Center, irrespective of the object file's original pivot. You can manipulate it using a rotating gizmo, and the object itself can be modified.
   
2. **Texture Application**: Apply Albedo (Texture), Normal, and AO images to view them on the object.
   
3. **Convenient Sizing**: Easily reset the length of object files using real-world measurements.
   
4. **Quality Modification**: Adjust the quality of object files using the Quality Slider.
   
5. **Optimization**: Reduce vertices to optimize object capacity.
   
6. **Capture and GIFs**: Capture screenshots with transparent backgrounds and create Loop GIFs for objects in 360 directions.

## Newly Updated!

Updated to version 1.0 after several modifications!

![MeshWiz Update](https://blog.kakaocdn.net/dn/bL9lUV/btsvkRrrEop/mxiqLKIRBrKsttIJv8Ozs0/img.gif)

- **GIF Creation**: Capture GIFs for object files with transparent backgrounds.
- **Fast ScreenShot**: ScreenShot processing in the subthread ensures fast and smooth captures.

## How to Download

Supports both Windows and MacOS. Download from the Google Drive link below.

Download Here: [Google Drive](https://drive.google.com/drive/folders/1255flaD3pOOSMzXRwbupYDtUh8jRuuCM?usp=sharing)

## How To Use?
### [Installation]

1. Download the program from the Google Drive link above.
2. After extracting the files, run the program.

### [Object Rotation]

1. Click and drag the gizmo with the mouse to rotate the object.
2. Use the Axis settings in the bottom-left corner to choose between Global and Local Axis rotation.
3. Press the "Export File" button in the top-left corner to apply the current settings to the loaded file.

### [Object Scaling]

1. Enter the desired size in centimeters (cm) into the "Set Object Size" input field at the bottom.
2. Click the "Set" button to adjust the object's size.
3. Determine the object's size based on the longest axis visible in the current view, and enter it in centimeters (cm). The longest axis is indicated by a pink line.
4. Press the "Export File" button in the top-left corner to apply the current settings to the loaded file.

### [Object Quality Adjustment]

1. Adjust the Mesh Quality slider at the bottom to modify the object's quality.
2. Press the "Export File" button in the top-left corner to apply the current settings to the loaded file.

### [Screenshot and GIF]

1. Enter the desired width and height in the "Width" and "Height" input fields at the top-right.
2. After clicking the button, a new folder will be created in the loaded file's directory, containing files with "Thumb (Capture)" or "Rotate (GIF)" appended to their names.

## Application Example

> In Unity, the large chair is set to appear at 1 meter with the default scale of Vector.one (1,1,1).

![Chair Example](https://drive.google.com/uc?id=1uR2ZJV-Lq8RLUFLhmIawyBd21O0TGRza)

> These changes are also fully implemented in 3ds Max.

![3ds Max Example](https://drive.google.com/uc?id=1vz0LRepQJGgpjDebLMGo34-4gGf9dmtE)

> The texture also works perfectly.

![Texture Example](https://drive.google.com/uc?export=view&id=1pA4GnLoJ5HTSD_CHjUC0PApmGkhaTQVZ)

## Reference

- https://github.com/yasirkula/UnitySimpleFileBrowser
- https://github.com/Dummiesman
- https://github.com/simonwittber/uGIF
- https://github.com/Whinarn/UnityMeshSimplifier
