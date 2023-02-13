

# Mesh Resizer 0.2v (23.02.13)


A program that sets the **size(scale)**, **rotation**, and **pivot(to center)** of the **.obj** file.
![enter image description here](https://drive.google.com/uc?export=view&id=1IYhnw9Sz-7lfkFfvnsVb4V-sbScF9gu0)

## Newly Updated!

 1. Add Rotation Gizmo 
 2. Worldspace, local space rotatable 
 3. Add Dialog Box(for warning)
 4. Changing the automatic name generation logic
 5. Rotating relative to the center axis regardless of the pivot of the object file
 6. Automatically resize using [cm]

  ㅤ

## Program Features

The .obj file itself modifies the vertex value of the mesh to set the size, rotation value, and overall pivot position.
UV mapping and image mapping will remain the same, and the output file will be applicable to **most programs**  *(maybe)*.

ㅤ

## How to download
Supports Windows and MacOS. Download from the Google Drive link below.

| Platform| Link |
|--|--|
| Windows| [\[Link\]](https://drive.google.com/file/d/1PCVvpivkXQaWI4raJApmqTSkvWHTTzWF/view?usp=sharing) |
| MacOS| [\[Link\]](https://drive.google.com/file/d/1HpRLAF_PgbghBRa3Tdd-kLh-tfV2dxNn/view?usp=sharing) |

Older Versions Achives are [\[Here\]](https://drive.google.com/drive/folders/1255flaD3pOOSMzXRwbupYDtUh8jRuuCM?usp=sharing)



ㅤ

## How To Use?

> Watch the example video below!!

[![Video Label](https://img.youtube.com/vi/dsj9x4qgeqQ/0.jpg)](https://youtu.be/dsj9x4qgeqQ)


1. Run the program.  
2. Load the obj file you want to change from the top left button '**Open**'.
>  **Note:** If the size of the obj file (the size of the mesh) is too large or too small, a warning window appears. You can adjust it to a specific size in the input field.

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
