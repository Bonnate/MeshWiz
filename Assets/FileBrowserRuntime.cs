using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using System;

//Runtime OBJ Importer의 에셋 제공자인 Dummiesman의 네임스페이스 사용
using Dummiesman;

public class FileBrowserRuntime : Singleton<FileBrowserRuntime>
{
    public string mCurrentPath;
    public string mCurrentFileName;
    private GameObject mCurrentGameObject;

    private void Start() 
    {

    }

    /// <summary>
    /// 아이템 셋(씬에서 사용하는 오브젝트의 설정) 데이터를 읽도록 하는 함수
    /// </summary>
    public void OpenItemSetFile()
    {
        Destroy(mCurrentGameObject);

		 FileBrowser
            .SetFilters(true, new FileBrowser
            .Filter("Files", ".json")
            , new FileBrowser.Filter("Text Files", ".json"));

        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Coroutine
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    /// <summary>
    /// 아이템 셋에 대한 파일을 읽고, 이를 해석하도록 하는 함수
    /// </summary>
    /// <returns></returns>
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

            mCurrentGameObject = new OBJLoader().Load(new MemoryStream(bytes));
            mCurrentGameObject.SetActive(true);
            MainSceneController.Instance.LoadCurrentObj(mCurrentGameObject);
        }
    }

    /// <summary>
    /// 아이템 셋을 json형식으로 저장하도록 하는 함수
    /// </summary>
    /// <param name="text">직렬화 된 스트림</param>
    public void SaveItemSet(string text)
    {
        string initialPath = "C:\\Users\\[YourPath]";
        string initialFilename = "SaveData_" + DateTime.Now.ToString(("MM_dd_HH_mm_ss")) + ".json";
        FileBrowser.ShowSaveDialog(null, null, FileBrowser.PickMode.Files, false, initialPath, initialFilename, "Save As", "Save");

        StartCoroutine(ShowSaveDialogCoroutine(text, initialPath, initialFilename));
    }

    /// <summary>
    /// 아이템 셋을 저장하는 코루틴
    /// </summary>
    /// <param name="text"></param>
    /// <param name="initialPath"></param>
    /// <param name="initialFilename"></param>
    /// <returns></returns>
    IEnumerator ShowSaveDialogCoroutine(string text, string initialPath = null, string initialFilename = null)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, initialPath, initialFilename, "Save Files and Folders", "Save");

        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];
            File.WriteAllText(path, text);
        }
    }
}