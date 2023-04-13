using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAxisViewer : Singleton<CameraAxisViewer>
{
    [SerializeField] private GameObject[] mViewTransforms;
    [SerializeField] private Image mCameraStateImage;

    [SerializeField] Camera mMainCamera;
    private Camera[] mOverlayCameras;

    [SerializeField] private float mMinFOV;
    [SerializeField] private float mMaxFOV;

    [SerializeField] private Slider mFOVSlider;

    private float mCurrentFOV, mPrevFOV;

    private void Start()
    {
        mOverlayCameras = FindObjectsOfType<Camera>(true);

        mCurrentFOV = mMainCamera.orthographicSize;

        BTN_ChangeViewPos(0);
    }

    private void Update()
    {
        if(FileBrowserRuntime.IsDialogEnabled)
            return;

        UpdateCameraSize();

        if(Input.GetKeyDown(KeyCode.F))
            BTN_ChangeViewPos(0);

        if(Input.GetKeyDown(KeyCode.T))
            BTN_ChangeViewPos(1);

        if(Input.GetKeyDown(KeyCode.R))
            BTN_ChangeViewPos(2);     

        if(Input.GetKeyDown(KeyCode.O))
        {
            RotateSystemManager.Instance.InputHandler(RotateSystemManager.START_ROTATE);
            MeshController.Instance.BTN_ResetRotation();
        }
    }

    public void SetCurrentFOV(float amount)
    {
        mFOVSlider.SetValueWithoutNotify(amount);

        mPrevFOV = mCurrentFOV;
        mCurrentFOV = amount;
    }

    private void UpdateCameraSize()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        mCurrentFOV += wheelInput * 30.0f * Time.deltaTime;

        if(wheelInput != 0f)
            mFOVSlider.SetValueWithoutNotify(mCurrentFOV);

        mCurrentFOV = Mathf.Clamp(mCurrentFOV, mMinFOV, mMaxFOV);

        mMainCamera.orthographicSize = mPrevFOV = Mathf.Lerp(mPrevFOV, mCurrentFOV, Time.deltaTime * 2.5f);

        foreach (Camera camera in mOverlayCameras)
            camera.orthographicSize = mMainCamera.orthographicSize;
    }

    public void BTN_ChangeViewPos(int val)
    {
        Camera.main.transform.parent = mViewTransforms[val].transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;

        switch (val)
        {
            case 0:
                mCameraStateImage.color = Color.blue;
                break;
            case 1:
                mCameraStateImage.color = Color.green;
                break;
            case 2:
                mCameraStateImage.color = Color.red;
                break;
        }
    }

    public void SLIDER_ModifyFOV()
    {
        SetCurrentFOV(mFOVSlider.value);
    }
}
