using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using System;
using Dummiesman;
using System.Text;
using System.Linq;
using UnityEngine.UI;

public class FileBrowserRuntime : Singleton<FileBrowserRuntime>
{
    /// <summary>
    /// 현재 다이얼로그 창이 열려있는가?
    /// </summary>
    /// <value></value>
    public static bool IsDialogEnabled { set; get; } = false;

    [SerializeField] private Button mExportBtn;

    [HideInInspector] public string CurrentPath { private set; get; }
    [HideInInspector] public string CurrentFileName { private set; get; }

    private System.Text.StringBuilder receivedString = new System.Text.StringBuilder();
    private int expectedLength = 0;

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load");

        IsDialogEnabled = false;

        if (FileBrowser.Success)
        {
            string filePath = FileBrowser.Result[0];
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);
            CurrentPath = filePath;

            string fileExtension = Path.GetExtension(filePath).ToLower();

            if (fileExtension == ".obj")
            {
                LoadObjFileViaMemoryStream(new MemoryStream(bytes), Path.GetFileName(CurrentPath));
            }
            else
            {
                DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Selected file is not .obj format!", "OK");
            }
        }
    }

    public void LoadObjFileViaMemoryStream(MemoryStream bytes, string fileName)
    {
        GameObject newObj = new OBJLoader().Load(bytes);
        newObj.name = CurrentFileName = fileName;
        newObj.SetActive(true);
        newObj.transform.localScale = Vector3.one;

        //메시 컨트롤러에 로드한 obj를 등록
        MeshController.Instance.Load(bytes, newObj);

        // 머티리얼 리프레쉬
        MeshController.Instance.RefreshMaterial();
    }

    public void BTN_LoadObjFile()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return;

        // WebGL이 아닌 타입이라면?
        IsDialogEnabled = true;

        FileBrowser
           .SetFilters(true, new FileBrowser
           .Filter("Files", ".json")
           , new FileBrowser.Filter("Text Files", ".json"));

        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Coroutine
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void BTN_ExportObjFile()
    {
        MeshController.Instance.Export();

        StartCoroutine(CoReloadExportButton());

        IEnumerator CoReloadExportButton()
        {
            mExportBtn.interactable = false;
            yield return new WaitForSeconds(2.0f);
            mExportBtn.interactable = true;
        }
    }
}