using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateSystemManager : Singleton<RotateSystemManager>
{
    public const string START_ROTATE = "_START_ROTATE";
    public const string UNDO_ROTATE = "_UNDO_ROTATE";

    [SerializeField] private Transform mRootRotateAxis;
    [SerializeField] private GameObject[] mLocalRotateAxises;
    [SerializeField] private GameObject[] mWorldRotateAxises;

    [Space][SerializeField] private Toggle mRotateWorldAxisToggle;

    private Camera mMainCamera;
    private Ray mCamRay;

    private Stack<Vector3> mEulerRotHistoryStack = new Stack<Vector3>();
    private Stack<Vector3> mEulerRotHistoryStackRedo = new Stack<Vector3>();

    private void Start()
    {
        mMainCamera = Camera.main;

        //Generate Ray
        mCamRay = new Ray(mMainCamera.transform.position, mMainCamera.transform.forward);

        SetAxisScale(0f);
    }

    private void Update()
    {
        if (RotateSystem.IsEnabled)
            return;
        if (FileBrowserRuntime.IsDialogEnabled)
            return;

        Ray ray = mMainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.tag == "RotateAxis")
            {
                hit.transform.GetComponent<RotateSystem>().HitByRay();
            }
        }

        if(RotateSystem.IsEnabled == false && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKeyDown(KeyCode.Z))
            InputHandler(UNDO_ROTATE);

        if(Input.GetKeyDown(KeyCode.A))
            mRotateWorldAxisToggle.isOn = !mRotateWorldAxisToggle.isOn;
    }

    public void InputHandler(string message)
    {
        switch (message)
        {
            case START_ROTATE:
                {
                    mEulerRotHistoryStack.Push(mRootRotateAxis.eulerAngles);
                    break;
                }

            case UNDO_ROTATE:
                {
                    if(mEulerRotHistoryStack.Count > 0)
                    {
                        Vector3 eulerAngles = mEulerRotHistoryStack.Pop();
                        mRootRotateAxis.eulerAngles = eulerAngles;
                    }
                    break;
                }
        }
    }

    public void SetAxisScale(float scale)
    {
        scale *= 5.0f;

        foreach (GameObject axisTransform in mLocalRotateAxises) { axisTransform.transform.localScale = Vector3.one * scale; }
        foreach (GameObject axisTransform in mWorldRotateAxises) { axisTransform.transform.localScale = Vector3.one * scale; }
    }

    public void BTN_ToggleRotateAxis()
    {
        foreach (GameObject axisTransform in mLocalRotateAxises) { axisTransform.SetActive(!mRotateWorldAxisToggle.isOn); }
        foreach (GameObject axisTransform in mWorldRotateAxises) { axisTransform.SetActive(mRotateWorldAxisToggle.isOn); }
    }
}