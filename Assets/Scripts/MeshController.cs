using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogBox;

public class MeshController : Singleton<MeshController>
{
    [SerializeField] public Material SampleMaterial;
    [SerializeField] private Transform mCenterPivotTransform;
    [SerializeField] private TMP_InputField mResizeInputField;

    //현재 활성화된 오브젝트
    [SerializeField] public GameObject CurrentGo = null;

    /// <summary>
    /// 오브젝트의 크기를 설정
    /// </summary>
    /// <param name="CurrentGo.transform"></param>
    /// <param name="meter"></param>
    private void SetScale(float meter)
    {
        Transform currentObjTransform = CurrentGo.transform;

        float longestLength = UtilityManager.GetLongestVertexLengthWithScale(CurrentGo.GetComponent<MeshFilter>().sharedMesh, currentObjTransform, false);

        float scaleDelta = longestLength;
        currentObjTransform.localScale = currentObjTransform.localScale / scaleDelta * meter;

        // 중앙으로 이동
        UtilityManager.SetPositionViaCenter(CurrentGo.transform, Vector3.zero);

        // 축 크기 설정
        RotateSystemManager.Instance.SetAxisScale(UtilityManager.GetLongestVertexLengthWithScale(CurrentGo.GetComponent<MeshFilter>().sharedMesh, CurrentGo.transform, true));
    }

    #region Load / Export
    /// <summary>
    /// 파일 탐색기로부터 불러온 obj파일을 로드
    /// </summary>
    public void Load(GameObject obj)
    {
        mCenterPivotTransform.rotation = Quaternion.identity;

        Destroy(CurrentGo);

        CurrentGo = obj;
        CurrentGo.AddComponent<OBJExportManager>();

        CurrentGo.transform.SetParent(mCenterPivotTransform);
        mCenterPivotTransform.localScale = new Vector3(-1, 1, 1);
        CurrentGo.transform.transform.localScale = Vector3.one;

        // 중앙으로 이동
        UtilityManager.SetPositionViaCenter(CurrentGo.transform, Vector3.zero);

        // 가장 긴 축 길이 획득
        float longestLength = UtilityManager.GetLongestVertexLengthWithScale(CurrentGo.GetComponent<MeshFilter>().sharedMesh, CurrentGo.transform, true);

        // 축 크기 설정
        RotateSystemManager.Instance.SetAxisScale(longestLength);

        mResizeInputField.text = (longestLength * 100f).ToString("F0");
    }

    /// <summary>
    /// 현재 씬에 있는 obj를 파일로 출력
    /// </summary>
    public void Export()
    {
        if (CurrentGo == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "There are currently no objects loaded.", "OK");
            return;
        }

        CurrentGo.GetComponent<OBJExportManager>().ExportToLocal();
    }
    #endregion

    #region Button Events
    public void BTN_ResizeObj()
    {
        if (CurrentGo == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "There are currently no objects loaded.", "OK");
            return;
        }

        if (mResizeInputField.text.Length == 0)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "No input value.\nPlease check again.", "OK");
            return;
        }

        if (int.Parse(mResizeInputField.text) == 0)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Zero is not available.\nPlease check again.", "OK");
            return;
        }        

        // 크기 조정
        SetScale(float.Parse(mResizeInputField.text) * 0.01f);
    }

    public void BTN_ResetRotation()
    {
        mCenterPivotTransform.eulerAngles = Vector3.zero;
    }

    #endregion
}
