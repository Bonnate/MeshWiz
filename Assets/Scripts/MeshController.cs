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
    [HideInInspector] public GameObject CurrentGo = null;
    [HideInInspector] public Bounds CurrentGoBounds;
    [HideInInspector] public float CurrentGoMaxLength;

    /// <summary>
    /// 오브젝트의 크기를 설정
    /// </summary>
    /// <param name="CurrentGo.transform"></param>
    /// <param name="meter"></param>
    private void SetScale(float meter)
    {
        // Bounding Box 재구성
        CurrentGoBounds = UtilityManager.RecalculateBoundingBox(CurrentGo.GetComponent<MeshFilter>());
        (Vector3 startPos, Vector3 endPos) = UtilityManager.GetLongestAxisVectors(CurrentGoBounds);
        CurrentGoMaxLength = (startPos - endPos).magnitude;

        // 크기 설정
        CurrentGo.transform.localScale = CurrentGo.transform.localScale / CurrentGoMaxLength * meter;

        // 중앙으로 이동
        UtilityManager.SetPositionViaCenter(CurrentGo.transform, Vector3.zero);

        // 변경된 크기를 기반으로 Bounding Box 재구성 및 렌더링
        CurrentGoBounds = UtilityManager.RecalculateBoundingBox(CurrentGo.GetComponent<MeshFilter>());
        (startPos, endPos) = UtilityManager.GetLongestAxisVectors(CurrentGoBounds);
        CurrentGoMaxLength = (startPos - endPos).magnitude;        
        UtilityManager.DrawLine(startPos, endPos, CurrentGoMaxLength);
        UtilityManager.DrawBounds(CurrentGoBounds, CurrentGoMaxLength);

        // 축 크기 설정
        RotateSystemManager.Instance.SetAxisScale(CurrentGoMaxLength);

        // 카메라 뷰 설정
        SetCameraFOV();
    }

    private void SetCameraFOV()
    {
        float minInput = 1.0f;
        float maxInput = 1000.0f;
        float minOutput = 0.01f;
        float maxOutput = 5.0f;

        float inputValueRange = maxInput - minInput;
        float outputValueRange = maxOutput - minOutput;
        float normalizedValue = ((CurrentGoMaxLength * 100f) - minInput) / inputValueRange;
        float convertedValue = (normalizedValue * outputValueRange) + minOutput;

        CameraAxisViewer.Instance.SetCurrentFOV(convertedValue);
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

        // Bounding Box 생성 및 렌더링
        CurrentGoBounds = UtilityManager.RecalculateBoundingBox(CurrentGo.GetComponent<MeshFilter>());
        (Vector3 startPos, Vector3 endPos) = UtilityManager.GetLongestAxisVectors(CurrentGoBounds);
        CurrentGoMaxLength = (startPos - endPos).magnitude;        
        UtilityManager.DrawLine(startPos, endPos, CurrentGoMaxLength);
        UtilityManager.DrawBounds(CurrentGoBounds, CurrentGoMaxLength);

        // 축 크기 설정
        RotateSystemManager.Instance.SetAxisScale(CurrentGoMaxLength);

        // 현재 길이를 표시
        mResizeInputField.text = (CurrentGoMaxLength * 100f).ToString("F0");

        // 카메라 뷰 설정
        SetCameraFOV();
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
