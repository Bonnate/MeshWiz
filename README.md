
# Mesh Resizer 0.9v (23.09.18)


A program that sets the **size(scale)**, **rotation**, and **pivot(to center)** of the **.obj** file.
![](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https%3A%2F%2Fblog.kakaocdn.net%2Fdn%2Fn1NLd%2FbtsueikpfgY%2FTeWF8LWpfZsoIeEfBiLAMK%2Fimg.png)

## Newly Updated!

![](https://blog.kakaocdn.net/dn/baTu1b/btsuh6KiTBj/qMdTfDwvomkmVxN0o7UNC0/img.png)

[ENG]
1. Modified to take screenshots. Except for objects, UI or background is removed and processed transparently.
2. Modified to keep 'usemtl' item in metadata.
3. From now on, you can recall Albedo, Normal, and Occlusion.

[KOR]
1. 스크린샷을 찍을 수 있게 수정되었습니다. 오브젝트를 제외한 UI나 배경은 제거되어 투명하게 처리됩니다.
2. 메타데이터 중 'usemtl' 항목을 유지하게 수정되었습니다.
3. 이제부터 Albedo, Normal, Occlusion을 불러올 수 있습니다.

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

</br></br>

Simple Tooltip in program!

![Simple Tooltip in program!](https://img1.daumcdn.net/thumb/R1280x0/?scode=mtistory2&fname=https%3A%2F%2Fblog.kakaocdn.net%2Fdn%2F0kWn5%2FbtsuGBbl8gz%2FzIIz5I8bXg05wXKHdrhEk0%2Fimg.png)

</br></br>

1. Run the program.  
2. Load the obj file you want to change from the top left button '**Open**'.
>  **Note:** If the size of the obj file (the size of the mesh) is too large or too small, a warning window appears. You can adjust it to a specific size in the input field.

>  **Note:** Scene can be zoomed through the mouse wheel.

ㅤ

3. If necessary, you can adjust the size by specifying the size in cm in the input field.
4. Rotate the object using the 3-axis gizmo. You can rotate to local and world reference in the toggle at the bottom left.
ㅤ
ㅤ

>  **Note:** Exporting creates a new file with the **suffix _original** attached to the file at the location of the imported file **without the need to set a new path.** and the original loaded file will be modified and saved.

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
