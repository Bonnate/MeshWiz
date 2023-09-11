
# Mesh Resizer 0.7v (23.09.11)


A program that sets the **size(scale)**, **rotation**, and **pivot(to center)** of the **.obj** file.
![enter image description here](https://drive.google.com/uc?export=view&id=1mn3qOkuF11n3GWUwA92hN54C1Ha9QDr9)

## Newly Updated!

[ENG]
1. When you export, the suffix _modified will no longer be added, and the changed values ​​will be overwritten in the loaded file. (The loaded file is changed itself)
2. File with the _modified suffix are no longer created, and loaded files are appended with the _original suffix to back up the original files. (time stamp added)
3. To avoid duplication of timestamps, the button is temporarily disabled so that Export can be performed every 2 seconds.

[KOR]
1. Export를 하면 이제부터 _modified라는 수식어가 붙지 않고, 불러온 파일에 변경된 값을 덮어쓰기합니다. (불러온 파일 자체가 변경됨)
2. _modified 수식어가 붙은 파일은 더 이상 생성되지 않으며, 불러온 파일이 _original이라는 수식어로 붙어 원본 파일을 백업합니다. (타임 스탬프가 추가됨)
3. 타임 스탬프의 중복을 피하기 위해 Export는 2초마다 수행할 수 있도록 버튼이 일시적으로 비활성화됩니다.


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

![Simple Tooltip in program!](https://drive.google.com/uc?export=view&id=1fioz1-1JymbZthaRD0hIEIIHaWq2ABv_)

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
