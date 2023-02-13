using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeshController : Singleton<MeshController>
{
    [SerializeField] public Material SampleMaterial;
    [SerializeField] private Transform mCenterPivotTransform;
    [SerializeField] private TMP_InputField mResizeInputField;

    //현재 활성화된 오브젝트
    private GameObject mCurrentObject = null;

    //오브젝트 회전을 위한 마우스 입력
    private Vector2 mMouseInput;

    /// <summary>
    /// 오브젝트의 크기를 설정
    /// </summary>
    /// <param name="mCurrentObject.transform"></param>
    /// <param name="meter"></param>
    private void SetScale(float meter)
    {
        Debug.LogFormat("Set Scale to {0} meter", meter);

        bool isColliderExists = true;

        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            isColliderExists = false;
            collider = mCurrentObject.AddComponent<BoxCollider>();
        }

        mCurrentObject.transform.transform.localScale = Vector3.one;

        float maxBoxScale = Mathf.Max(collider.size.x, collider.size.y, collider.size.z);
        float maxLocalScale = Mathf.Max(mCurrentObject.transform.localScale.x, mCurrentObject.transform.localScale.y, mCurrentObject.transform.localScale.z);

        float currentScale = maxBoxScale * maxLocalScale;
        mCurrentObject.transform.localScale = transform.localScale / currentScale * meter;

        mCurrentObject.transform.transform.Translate(Vector3.forward * (mCurrentObject.transform.transform.localPosition.z - collider.center.z));

        if (!isColliderExists)
        {
            Debug.Log("콜라이더 파괴");
            Destroy(collider);
        }
    }

    #region Load / Export
    /// <summary>
    /// 파일 탐색기로부터 불러온 obj파일을 로드
    /// </summary>
    public void Load(GameObject obj)
    {
        mCenterPivotTransform.rotation = Quaternion.identity;
        mCenterPivotTransform.localScale = Vector3.one;

        Destroy(mCurrentObject);

        mCurrentObject = obj;
        mCurrentObject.AddComponent<OBJExportManager>();

        UtilityManager.SetPositionViaCenter(mCurrentObject.transform, Vector3.zero);

        mCurrentObject.transform.SetParent(mCenterPivotTransform);

        float currentObjectWidth = CheckCurrentObjSize();

        if(currentObjectWidth > 5.0f)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Object size is too large. \nIf necessary, adjust the size in the input field.", "OK");
        }
        if(currentObjectWidth < 0.1f)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "Object size is too tiny. \nIf necessary, adjust the size in the input field.", "OK");
        }

        RotateSystemManager.Instance.SetAxisScale(currentObjectWidth * 5f);
    }

    private float CheckCurrentObjSize()
    {
        bool isColliderExists = true;

        BoxCollider collider = mCurrentObject.GetComponent<BoxCollider>();
        if (collider == null)
        {
            isColliderExists = false;
            collider = mCurrentObject.AddComponent<BoxCollider>();
        }

        var min = collider.center - collider.size * 0.5f;
        var max = collider.center + collider.size * 0.5f;

        var P000 = mCurrentObject.transform.TransformPoint(new Vector3(min.x, min.y, min.z));
        var P111 = mCurrentObject.transform.TransformPoint(new Vector3(max.x, max.y, max.z));

        if (!isColliderExists) { Destroy(collider); }

        return (P000 - P111).magnitude;
    }

    /// <summary>
    /// 현재 씬에 있는 obj를 파일로 출력
    /// </summary>
    public void Export()
    {
        if(mCurrentObject == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "There are currently no objects loaded.", "OK");
            return;
        }

        mCurrentObject.GetComponent<OBJExportManager>().ExportToLocal();
    }
    #endregion

    #region Button Events
    public void BTN_ResizeObj()
    {
        if(mCurrentObject == null)
        {
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "There are currently no objects loaded.", "OK");
            return;
        }

        if(mResizeInputField.text.Length == 0) 
        { 
            DialogBoxGenerator.Instance.CreateSimpleDialogBox("Warning", "No input value.\nPlease check again.", "OK");
        }

        SetScale(float.Parse(mResizeInputField.text) * 0.01f);
        UtilityManager.SetPositionViaCenter(mCurrentObject.transform, Vector3.zero);

        RotateSystemManager.Instance.SetAxisScale(CheckCurrentObjSize() * 5f);
    }
    #endregion
}
