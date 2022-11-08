using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineSceneManager : MonoBehaviour
{
    [SerializeField] GameObject[] mViewTransforms;

    public void BTN_ChangeViewPos(int val)
    {
        Camera.main.transform.parent = mViewTransforms[val].transform;
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.identity;
    }
}
