using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using System;
using Dummiesman;

public class FileBrowserRuntime : Singleton<FileBrowserRuntime>
{
    /// <summary>
    /// 현재 다이얼로그 창이 열려있는가?
    /// </summary>
    /// <value></value>
    public static bool IsDialogEnabled { private set; get; }

    [HideInInspector] public string CurrentPath { private set; get; }
    [HideInInspector] public string CurrentFileName { private set; get; }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        IsDialogEnabled = false;

        if (FileBrowser.Success)
        {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            CurrentPath = FileBrowser.Result[0];
            CurrentFileName = Path.GetFileName(CurrentPath);

            Debug.Log(CurrentPath);
            Debug.Log(CurrentFileName);

            GameObject newObj = new OBJLoader().Load(new MemoryStream(bytes));
            newObj.name = CurrentFileName;
            newObj.SetActive(true);
            newObj.transform.localScale = Vector3.one;

            //메시 컨트롤러에 로드한 obj를 등록
            MeshController.Instance.Load(newObj);
        }
    }

    public void BTN_LoadObjFile()
    {
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
    }
}