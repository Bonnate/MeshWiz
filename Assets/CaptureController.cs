using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;

public class CaptureController : MonoBehaviour
{
    public static int _GIF_INDEX_CNT;
    private string gifExportFolderName = "";

    public Camera mainCamera; // Main Camera를 Inspector에서 할당해야 합니다.
    public GameObject[] mDisableGos; // 비활성화할 게임 오브젝트 배열
    public GameObject mAxisParent;
    public GameObject mCenterRotationGo;

    public void BTN_Capture()
    {
        if (MeshController.Instance.CurrentGo == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "No .obj file selected!", "OK");
            return;
        }

        foreach (Transform tr in mAxisParent.GetComponentsInChildren<Transform>(true))
            if (tr.gameObject.name.Contains("Clone"))
                Destroy(tr.gameObject);

        // 게임 오브젝트를 비활성화합니다.
        foreach (GameObject go in mDisableGos)
            go.SetActive(false);

        StartCoroutine(CoTakeScreenshot());
    }

    public void BTN_CaptureAsGIF()
    {
        StartCoroutine(CoTakeAsGIF());
    }

    IEnumerator CoTakeAsGIF()
    {
        string gifFolderName = $"GIF_{System.DateTime.Now:yyyyMMddHHmmss}";
        _GIF_INDEX_CNT = 0;

        for (int i = 0; i < 3; ++i)
        {
            yield return new WaitForSeconds(1.0f);
            mCenterRotationGo.transform.eulerAngles = Vector3.up * i * 10.0f;
            yield return StartCoroutine(CoTakeScreenshot(gifFolderName));
        }

        ExportToGIF();
    }

    public void ExportToGIF()
    {
    }

    IEnumerator CoTakeScreenshot(string reservedFolderName = "")
    {
        yield return new WaitForEndOfFrame(); // 화면이 모두 렌더링되기를 기다립니다.

        // 텍스쳐 생성.
        TextureFormat format = TextureFormat.ARGB32;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, format, false);
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 32);

        // 카메라 백업.
        RenderTexture camtargetTexture = mainCamera.targetTexture;
        RenderTexture RenderTextureactive = RenderTexture.active;
        CameraClearFlags camclearFlags = mainCamera.clearFlags;
        UnityEngine.Color cambackground = mainCamera.backgroundColor;

        // 카메라 설정.
        RenderTexture.active = rt;
        mainCamera.targetTexture = rt;

        var urpCamSettings = mainCamera.GetComponent<UniversalAdditionalCameraData>();
        urpCamSettings.renderType = CameraRenderType.Base;
        urpCamSettings.renderPostProcessing = false;

        // URP
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = UnityEngine.Color.clear;

        Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
        mainCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        mainCamera.targetTexture = camtargetTexture;
        RenderTexture.active = RenderTextureactive;
        mainCamera.clearFlags = camclearFlags;
        mainCamera.backgroundColor = cambackground;

        mainCamera.Render();

        // URP
        urpCamSettings.renderType = CameraRenderType.Overlay;
        urpCamSettings.renderPostProcessing = true;

        // FileBrowserRuntime.CurrentPath에서 디렉토리 명을 획득
        string currentDirectory = FileBrowserRuntime.Instance.CurrentPath;

        // 현재 디렉토리에서 파일 이름만 추출
        string currentFileName = Path.GetFileName(currentDirectory);

        // 디렉토리 경로만 추출
        currentDirectory = currentDirectory.Replace(currentFileName, "");

        string screenshotsFolder;
        // "Screenshots" 폴더의 경로를 설정
        if (reservedFolderName != "")
        {
            screenshotsFolder = Path.Combine(currentDirectory, reservedFolderName);
            gifExportFolderName = screenshotsFolder;
        }
        else
            screenshotsFolder = Path.Combine(currentDirectory, "Screenshots");


        string screenshotFileName = $"Screenshot{currentFileName}_{_GIF_INDEX_CNT++}_{System.DateTime.Now:yyyyMMddHHmmssms}.png";


        // "Screenshots" 폴더가 이미 존재하는지 확인
        if (Directory.Exists(screenshotsFolder))
        {
            // 스크린샷을 저
            byte[] screenshotBytes = screenShot.EncodeToPNG();
            string screenshotPath = Path.Combine(screenshotsFolder, screenshotFileName);
            File.WriteAllBytes(screenshotPath, screenshotBytes);
        }
        else
        {
            // "Screenshots" 폴더를 생성하고 스크린샷을 저장
            Directory.CreateDirectory(screenshotsFolder);

            // 스크린샷을 저장합니다.
            byte[] screenshotBytes = screenShot.EncodeToPNG();
            string screenshotPath = Path.Combine(screenshotsFolder, screenshotFileName);
            File.WriteAllBytes(screenshotPath, screenshotBytes);
        }

        // 게임 오브젝트를 다시 활성화
        foreach (GameObject go in mDisableGos)
            go.SetActive(true);

        for (int i = 0; i < 2; ++i)
            RotateSystemManager.Instance.BTN_ToggleRotateAxis();
    }
}