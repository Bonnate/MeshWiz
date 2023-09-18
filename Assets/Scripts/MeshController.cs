using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

public class MeshController : Singleton<MeshController>
{
    [SerializeField] public Material SampleMaterial;
    [SerializeField] private Transform mCenterPivotTransform;
    [SerializeField] private TMP_InputField mResizeInputField;

    //현재 활성화된 오브젝트
    [HideInInspector] public GameObject CurrentGo = null;
    [HideInInspector] public Bounds CurrentGoBounds;
    [HideInInspector] public float CurrentGoMaxLength;

    [HideInInspector] public string? CurrentGoMtlLibStr = null; // 현재 오브젝트의 mtllib 값
    [HideInInspector] public string? CurrentGoUseMtlValue = null; // 현재 오브젝트의 usemtl 값
    [HideInInspector] public string? CurrentGoGroupName = null; // 현재 오브젝트의 usemtl 값

    private MemoryStream mOriginalFileStream;

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

    public void RefreshMaterial()
    {
        if(CurrentGo == null)
            return;

        Material mat = CurrentGo.GetComponent<MeshRenderer>().material;

        // if(ImageLoadController._TEXTURE_IMAGE != null)
            mat.SetTexture("_BaseMap", ImageLoadController._TEXTURE_IMAGE);

        // if(ImageLoadController._NORMAL_IMAGE != null)
            mat.SetTexture("_BumpMap", ImageLoadController._NORMAL_IMAGE);
            
        // if(ImageLoadController._TEXTURE_IMAGE != null)
            mat.SetTexture("_OcclusionMap", ImageLoadController._AO_IMAGE);

    }

    #region Load / Export
    /// <summary>
    /// 파일 탐색기로부터 불러온 obj파일을 로드
    /// </summary>
    public void Load(MemoryStream stream, GameObject obj)
    {
        mOriginalFileStream = stream;

        mCenterPivotTransform.rotation = Quaternion.identity;

        Destroy(CurrentGo);

        CurrentGo = obj;

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

        // 오브젝트를 파일로 쓰기
        string str = UtilityManager.MeshToObjString(CurrentGo.GetComponent<MeshFilter>());

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            WebGLFileSaver.SaveFile(str, FileBrowserRuntime.Instance.CurrentFileName.Replace(".obj", "") + "_modified.obj");
        }
        else
        {
            // 첫 번째 파일 (modifiedStream)
            string modifiedFilePath = FileBrowserRuntime.Instance.CurrentPath;
            try
            {
                using (FileStream modifiedStream = new FileStream(modifiedFilePath, FileMode.Truncate))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(str);
                    modifiedStream.Write(bytes, 0, bytes.Length);
                    Debug.Log("Modified file overwritten successfully.");
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Error overwriting the modified file: " + e.Message);
            }

            // 두 번째 파일 (originalStream)
            string originalFilePath = FileBrowserRuntime.Instance.CurrentPath.Replace(".obj", "") + $"_original_{System.DateTime.Now.ToString("yyMMdd_HHmmss")}.obj";
            try
            {
                using (FileStream originalStream = new FileStream(originalFilePath, FileMode.OpenOrCreate))
                {
                    byte[] bytes = mOriginalFileStream.ToArray();
                    originalStream.Write(bytes, 0, bytes.Length);
                    Debug.Log("Original file created or overwritten successfully.");
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Error creating or overwriting the original file: " + e.Message);
            }
        }
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
