# Mesh Resizer 0.5v (23.04.13)


A program that sets the **size(scale)**, **rotation**, and **pivot(to center)** of the **.obj** file.
![enter image description here](https://drive.google.com/uc?export=view&id=1AvBUYeZucwIweaQgKIsBp79vtq-S4DcU)

## Newly Updated!

[ENG]
1. The reference algorithm for setting the size of the object has been changed. Previously, it was set to the longest axis of the mesh, but this can refer to the length of the diagonal, so we change it to a new way because we think it is inappropriate.
2. From now on, the criteria for setting the size of the object is to create a rectangular parallelepiped area from the current appearance and resize it by referring to the length of entering the longest axis of the three sides.
3. Hot keys have been added.
4. An icon is added in the upper left to show a brief description.
5. The axis referencing the size and the rectangular parallelepiped are visualized.

[KOR]
1. 오브젝트의 크기를 설정하는 기준 알고리즘이 변경되었습니다. 기존에는 Mesh의 가장 긴 축으로 설정했으나, 이것은 대각선의 길이를 참조할 수 있으므로 부적절하다고 판단하여 새로운 방식으로 변경합니다.
2. 이제부터 오브젝트의 크기를 설정하는 기준은 현재 보이는 모습에서 직육면체 영역을 생성하여 세변의 길이 중 가장 긴 축을 입력하는 길이로 참조하여 사이즈를 조절합니다.
3. 단축키가 추가되었습니다.
4. 좌측 상단에 간단한 설명을 보여줄 아이콘이 추가됩니다.
5. 사이즈를 참조하는 축과 직육면체가 시각화되어 나타납니다.

## Bug Fixed

[ENG]
1. When the dialog window to open the file is open, the ability to adjust the drag and object size temporarily stops.

[KOR]
1. 파일을 여는 다이얼로그 창이 열린 상태에서는 드래그 및 오브젝트 사이즈를 조절하는 기능이 일시적으로 멈춥니다.

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

![Simple Tooltip in program!](https://drive.google.com/uc?export=view&id=1iY3dwi4EG0wm4tyUhremVVYJob810wTo)

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
