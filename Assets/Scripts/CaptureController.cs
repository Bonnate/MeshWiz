using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;
using TMPro;

public class CaptureController : MonoBehaviour
{
    [SerializeField] private TMP_InputField mWidthInputField;
    [SerializeField] private TMP_InputField mHeightInputField;

    public static int _GIF_INDEX_CNT;
    private string gifExportFolderName = "";
    private string screenshotsFolder;

    public Camera mainCamera; // Main Camera를 Inspector에서 할당해야 합니다.
    public GameObject[] mDisableGos; // 비활성화할 게임 오브젝트 배열
    public GameObject mAxisParent;

    private List<Texture2D> mGIFTextures = new List<Texture2D>();

    public void BTN_Capture()
    {
        if (IsValid())
            StartCoroutine(CoTakeScreenshot());
    }

    public void BTN_CaptureAsGIF()
    {
        if (IsValid())
            StartCoroutine(CoTakeAsGIF());
    }

    private bool IsValid()
    {
        if (mWidthInputField.text.Length == 0)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Width Field is Empty!", "OK");
            return false;
        }

        if (mHeightInputField.text.Length == 0)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Height Field is Empty!", "OK");
            return false;
        }

        if (MeshController.Instance.CurrentGo == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "No .obj file selected!", "OK");
            return false;
        }

        return true;
    }

    IEnumerator CoTakeAsGIF()
    {
        mGIFTextures.Clear();
        string gifFolderName = $"GIF_{System.DateTime.Now:yyyyMMddHHmmss}";
        _GIF_INDEX_CNT = 0;

        for (int i = 0; i < 30; ++i)
        {
            // yield return new WaitForSeconds(1.0f);
            mAxisParent.transform.eulerAngles = Vector3.up * i * 12.0f;
            yield return StartCoroutine(CoTakeScreenshot(gifFolderName));
        }

        mAxisParent.transform.eulerAngles = Vector3.zero;
        uGIF.CaptureToGIF.Instance.ExportAsGIF(mGIFTextures, screenshotsFolder);
    }

    IEnumerator CoTakeScreenshot(string reservedFolderName = "")
    {
        foreach (Transform tr in mAxisParent.GetComponentsInChildren<Transform>(true))
            if (tr.gameObject.name.Contains("Clone"))
                Destroy(tr.gameObject);

        // 게임 오브젝트를 비활성화
        foreach (GameObject go in mDisableGos)
            go.SetActive(false);

        yield return new WaitForEndOfFrame(); // 화면이 모두 렌더링되기를 기다립니다.

        // 텍스쳐 생성
        TextureFormat format = TextureFormat.ARGB32;
        int desiredWidth = int.Parse(mWidthInputField.text);  // 원하는 가로 해상도
        int desiredHeight = int.Parse(mHeightInputField.text); // 원하는 세로 해상도
        Texture2D screenShot = new Texture2D(desiredWidth, desiredHeight, format, false);

        // RenderTexture 생성
        RenderTexture rt = new RenderTexture(desiredWidth, desiredHeight, 32);

        // 카메라 백업
        RenderTexture camtargetTexture = mainCamera.targetTexture;
        RenderTexture RenderTextureactive = RenderTexture.active;
        CameraClearFlags camclearFlags = mainCamera.clearFlags;
        UnityEngine.Color cambackground = mainCamera.backgroundColor;

        // 카메라 설정
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
        screenShot.ReadPixels(new Rect(0, 0, desiredWidth, desiredHeight), 0, 0);
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
        string currentDirectoryName = FileBrowserRuntime.Instance.CurrentPath;

        // 현재 디렉토리에서 파일 이름만 추출
        string currentFileName = Path.GetFileName(currentDirectoryName);

        // 디렉토리 경로만 추출
        currentDirectoryName = currentDirectoryName.Replace(currentFileName, "");

        // "Screenshots" 폴더의 경로를 설정
        if (reservedFolderName != "")
        {
            screenshotsFolder = Path.Combine(currentDirectoryName, reservedFolderName);
            gifExportFolderName = screenshotsFolder;
        }
        else
        {
            screenshotsFolder = Path.Combine(currentDirectoryName, "Screenshots");
        }

        string screenshotFileName = $"thumb{(reservedFolderName == "" ? "" : $"_{_GIF_INDEX_CNT++}")}_{System.DateTime.Now:yyyyMMddHHmmssms}.png";

        // "Screenshots" 폴더가 이미 존재하는지 확인
        if (!Directory.Exists(screenshotsFolder))
            Directory.CreateDirectory(screenshotsFolder);

        // 스크린샷을 저장
        byte[] screenshotBytes = screenShot.EncodeToPNG();
        string screenshotPath = Path.Combine(screenshotsFolder, screenshotFileName);

        System.Threading.Tasks.Task.Run(() =>
        {
            File.WriteAllBytes(screenshotPath, screenshotBytes);
        });

        // 게임 오브젝트를 다시 활성화
        foreach (GameObject go in mDisableGos)
            go.SetActive(true);

        for (int i = 0; i < 2; ++i)
            RotateSystemManager.Instance.BTN_ToggleRotateAxis();

        if (reservedFolderName != "")
            mGIFTextures.Add(screenShot);
    }
}