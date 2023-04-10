

# Mesh Resizer 0.4v (23.04.10)


A program that sets the **size(scale)**, **rotation**, and **pivot(to center)** of the **.obj** file.
![enter image description here](https://drive.google.com/uc?export=view&id=1zH1yMuTDMcmQsQm8aR7h8ZTx2VJPaZRb)

## Newly Updated!

1. New algorithm for scaling and rotation
2. When loading an obj file, the longest length of the obj file is displayed.
3. When setting the size, it shows as a line which standard the distance was referred to
4. Rotation value changes can be reversed by pressing Ctrl +Z

## Bug Fixed

1. Add action for invalid input value when setting object size
2. Increased accuracy of clicks on gizmos
3. A fatal problem in which objects are reversed and stored

## Program Features

The .obj file itself modifies the vertex value of the mesh to set the size, rotation value, and overall pivot position.
UV mapping and image mapping will remain the same, and the output file will be applicable to **most programs**  *(maybe)*.

ㅤ

## How to download
Supports Windows and MacOS. Download from the Google Drive link below.
</br></br>
Download Here: [\[Google Drive\]](https://drive.google.com/drive/folders/1255flaD3pOOSMzXRwbupYDtUh8jRuuCM?usp=sharing)



ㅤ

## How To Use?

> Watch the example video below!!

[![Video Label](https://img.youtube.com/vi/dsj9x4qgeqQ/0.jpg)](https://youtu.be/dsj9x4qgeqQ)


1. Run the program.  
2. Load the obj file you want to change from the top left button '**Open**'.
>  **Note:** If the size of the obj file (the size of the mesh) is too large or too small, a warning window appears. You can adjust it to a specific size in the input field.

>  **Note:** Scene can be zoomed through the mouse wheel.

ㅤ

3. If necessary, you can adjust the size by specifying the size in cm in the input field.
4. Rotate the object using the 3-axis gizmo. You can rotate to local and world reference in the toggle at the bottom left.
ㅤ
ㅤ

>  **Note:** Exporting creates a new file with the **suffix _resized** attached to the file at the location of the imported file **without the need to set a new path.**

>  **Note:** The **size of the grid means one meter in a unity meter** (for example, if a person is two meters, it takes up two compartments)

ㅤ

## Application example
> The large chair is set to appear 1 meter with the default scale of Vector.one (1,1,1) in Unity.
![Application example](https://drive.google.com/uc?id=1uR2ZJV-Lq8RLUFLhmIawyBd21O0TGRza)

> These changes are also fully implemented in 3ds Max.
![These changes are also fully implemented in 3ds Max](https://drive.google.com/uc?id=1vz0LRepQJGgpjDebLMGo34-4gGf9dmtE)

> The texture also works perfectly.
![The texture also works perfectly](https://drive.google.com/uc?export=view&id=1pA4GnLoJ5HTSD_CHjUC0PApmGkhaTQVZ)

ㅤ

## Reference

Use a file browser within Unity Project.
https://github.com/yasirkula/UnitySimpleFileBrowser

Enables import and export of obj files.
https://github.com/Dummiesman
