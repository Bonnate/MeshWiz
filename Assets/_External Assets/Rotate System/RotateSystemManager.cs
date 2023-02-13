using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateSystemManager : Singleton<RotateSystemManager>
{
    [SerializeField] private GameObject[] mLocalRotateAxises;
    [SerializeField] private GameObject[] mWorldRotateAxises;

    [Space][SerializeField] private Toggle mRotateWorldAxisToggle;

    private Camera mMainCamera;
    private Ray mCamRay;

    private void Start()
    {
        mMainCamera = Camera.main;

        //Generate Ray
        mCamRay = new Ray(mMainCamera.transform.position, mMainCamera.transform.forward);
    }

    private void Update()
    {
        if (RotateSystem.IsEnabled) { return; }

        Ray ray = mMainCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.tag == "RotateAxis")
            {
                hit.transform.GetComponent<RotateSystem>().HitByRay();
            }
        }
    }

    public void SetAxisScale(float scale)
    {
        foreach (GameObject axisTransform in mLocalRotateAxises) { axisTransform.transform.localScale = Vector3.one * scale; }
        foreach (GameObject axisTransform in mWorldRotateAxises) { axisTransform.transform.localScale = Vector3.one * scale; }
    }

    public void BTN_ToggleRotateAxis()
    {
        foreach (GameObject axisTransform in mLocalRotateAxises) { axisTransform.SetActive(!mRotateWorldAxisToggle.isOn); }
        foreach (GameObject axisTransform in mWorldRotateAxises) { axisTransform.SetActive(mRotateWorldAxisToggle.isOn); }
    }
}
