using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainSceneController : Singleton<MainSceneController>
{
    //현재 활성화된 오브젝트
    private GameObject mCurrentObject = null;

    //오브젝트 회전을 위한 마우스 입력
    private Vector2 mMouseInput;

    //외부 매니저
    private ObjFromStream mStream;  //오브젝트 다운로드 스트림 매니저

    //다운로드 입력값을 받기위한 입력필드
    [SerializeField] private TMP_InputField mDownloadNameField;

    private void Start()
    {
        mStream = FindObjectOfType<ObjFromStream>();
    }

    private void Update()
    {
        //현재 오브젝트가 등록되지 않으면 실행하지 않는다.
        if (mCurrentObject == null) { return; }

        mMouseInput.x = Input.GetAxis("Mouse X");
        mMouseInput.y = Input.GetAxis("Mouse Y");

        //회전시키기
        if (Input.GetMouseButton(0))
        {
            mCurrentObject.transform.Rotate(Camera.main.transform.up * -mMouseInput.x * Time.deltaTime * 500f, Space.World);
            mCurrentObject.transform.Rotate(Camera.main.transform.right * mMouseInput.y * Time.deltaTime * 500f, Space.World);
        }
        
        //움직이기
        if (Input.GetMouseButton(2))
        {
            mCurrentObject.transform.Translate(Camera.main.transform.right * mMouseInput.x * Time.deltaTime, Space.World);
            mCurrentObject.transform.Translate(Camera.main.transform.up * mMouseInput.y * Time.deltaTime, Space.World);
        }

    }

    /// <summary>
    /// 오브젝트를 등록한다.
    /// </summary>
    /// <param name="obj">등록 대상</param>
    public void LoadCurrentObj(GameObject obj)
    {
        mCurrentObject = obj;

        mCurrentObject.AddComponent<OBJExportManager>();
    }

    public void BTN_DownloadFromURL()
    {
        mStream.StartDownloadFile(mDownloadNameField.text);
    }

    public void BTN_ExportToOBJ()
    {
        mCurrentObject.GetComponent<OBJExportManager>().Export();
    }
}
