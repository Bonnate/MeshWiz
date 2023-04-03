using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAxisViewer : MonoBehaviour
{
    [SerializeField] private GameObject[] mViewTransforms;
    [SerializeField] private Image mCameraStateImage;

    [SerializeField] Camera mMainCamera;
    [SerializeField] Camera[] mOverlayCameras;

    [SerializeField] private float mMinFOV;
    [SerializeField] private float mMaxFOV;
    private float mCurrentFOV;

    private void Start()
    {
        mCurrentFOV = mMainCamera.orthographicSize;

        BTN_ChangeViewPos(0);
    }

    private void Update()
    {
        UpdateCameraSize();
    }

    private void UpdateCameraSize()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        mCurrentFOV += wheelInput * 30.0f * Time.deltaTime;

        mCurrentFOV = Mathf.Clamp(mCurrentFOV, mMinFOV, mMaxFOV);

        mMainCamera.orthographicSize = Mathf.Lerp(mMainCamera.orthographicSize, mCurrentFOV, Time.deltaTime * 5.0f);

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

}
