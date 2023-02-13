using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using System;
using Dummiesman;

public class FileBrowserRuntime : Singleton<FileBrowserRuntime>
{
    public string mCurrentPath;
    public string mCurrentFileName;

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load");

        if (FileBrowser.Success)
        {
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);

            mCurrentPath = FileBrowser.Result[0];
            mCurrentFileName = Path.GetFileName(mCurrentPath);

            Debug.Log(mCurrentPath);
            Debug.Log(mCurrentFileName);

            GameObject newObj = new OBJLoader().Load(new MemoryStream(bytes));
            newObj.SetActive(true);

            //메시 컨트롤러에 로드한 obj를 등록
            MeshController.Instance.Load(newObj);
        }
    }

    public void SaveItemSet(string text)
    {
        string initialPath = "C:\\Users\\[YourPath]";
        string initialFilename = "SaveData_" + DateTime.Now.ToString(("MM_dd_HH_mm_ss")) + ".json";
        FileBrowser.ShowSaveDialog(null, null, FileBrowser.PickMode.Files, false, initialPath, initialFilename, "Save As", "Save");

        StartCoroutine(ShowSaveDialogCoroutine(text, initialPath, initialFilename));
    }

    IEnumerator ShowSaveDialogCoroutine(string text, string initialPath = null, string initialFilename = null)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, initialPath, initialFilename, "Save Files and Folders", "Save");

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];
            File.WriteAllText(path, text);
        }
    }

    public void BTN_LoadObjFile()
    {
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