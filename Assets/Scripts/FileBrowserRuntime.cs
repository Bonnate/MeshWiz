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

    public string mCurrentPath;
    public string mCurrentFileName;

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        IsDialogEnabled = false;

        if (FileBrowser.Success)
        {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            mCurrentPath = FileBrowser.Result[0];
            mCurrentFileName = Path.GetFileName(mCurrentPath);

            Debug.Log(mCurrentPath);
            Debug.Log(mCurrentFileName);

            GameObject newObj = new OBJLoader().Load(new MemoryStream(bytes));
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