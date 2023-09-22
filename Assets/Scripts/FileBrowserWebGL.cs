using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using SFB;

[RequireComponent(typeof(Button))]
public class FileBrowserWebGL : MonoBehaviour, IPointerDownHandler
{
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Application.platform != RuntimePlatform.WebGLPlayer)
            return;

        UploadFile(gameObject.name, "OnFileUpload", ".obj", false);
    }

    public void OnFileUpload(string url)
    {
        StartCoroutine(OutputRoutine(url, EventReceiverWebGL._FileNames[0]));
    }

    private IEnumerator OutputRoutine(string url, string fileName)
    {
        var loader = new WWW(url);
        yield return loader;

        // .obj 파일을 byte 배열로 변환
        byte[] bytes = loader.bytes;

        // 오브젝트 불러오기
        FileBrowserRuntime.Instance.LoadObjFileViaMemoryStream(new MemoryStream(bytes), fileName);
    }
}