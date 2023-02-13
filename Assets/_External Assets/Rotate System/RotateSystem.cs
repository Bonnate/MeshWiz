using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    X,
    Y,
    Z,
}

public class RotateSystem : MonoBehaviour
{
    public static bool IsEnabled = false; //현재 어떠한 축이 사용중인가?

    [SerializeField] private Axis mRotateAxis; //담당하는 회전 축
    [SerializeField] private bool mIsLocalRotation = false;

    [SerializeField] private Transform mRotateTargetTransform = null; //회전각이 돌릴 대상의 트랜스폼

    private MeshRenderer mMeshRenderer; //메시 렌더러
    private Material mAxisMat; //축의 머티리얼

    Coroutine mFadeGizmoAlphaCor; //알파값 조절 코루틴
    bool mIsRotateEnabled = false;

    private void Start()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMeshRenderer.material = mAxisMat = new Material(mMeshRenderer.material);

        switch (mRotateAxis)
        {
            case Axis.X:
                mAxisMat.SetColor("_BaseColor", Color.red);
                break;
            case Axis.Y:
                mAxisMat.SetColor("_BaseColor", Color.green);
                break;
            case Axis.Z:
                mAxisMat.SetColor("_BaseColor", Color.blue);
                break;
        }
    }

    private void Update()
    {
        if (mIsRotateEnabled) { RotateTargetAxis(); }
    }

    public void HitByRay()
    {
        if (Input.GetMouseButton(0))
        {
            mIsRotateEnabled = true;
            IsEnabled = true;

            if (mFadeGizmoAlphaCor != null) { StopCoroutine(mFadeGizmoAlphaCor); }
            mFadeGizmoAlphaCor = StartCoroutine(COR_FadeGizmoAlpha(true));
        }
    }

    private void RotateTargetAxis()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        transform.Rotate(new Vector3(0, 0, mouseDelta.x + mouseDelta.y), Space.Self);

        switch (mRotateAxis)
        {
            case Axis.X:
                mRotateTargetTransform.transform.Rotate(Vector3.right * (mouseDelta.x + mouseDelta.y), mIsLocalRotation ? Space.Self : Space.World);
                break;

            case Axis.Y:
                mRotateTargetTransform.transform.Rotate(Vector3.up * (mouseDelta.x + mouseDelta.y), mIsLocalRotation ? Space.Self : Space.World);
                break;

            case Axis.Z:
                mRotateTargetTransform.transform.Rotate(Vector3.forward * (mouseDelta.x + mouseDelta.y), mIsLocalRotation ? Space.Self : Space.World);
                break;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mIsRotateEnabled = false;
            IsEnabled = false;

            if (mFadeGizmoAlphaCor != null) { StopCoroutine(mFadeGizmoAlphaCor); }
            mFadeGizmoAlphaCor = StartCoroutine(COR_FadeGizmoAlpha(false));
        }
    }

    private IEnumerator COR_FadeGizmoAlpha(bool isForward)
    {
        float process = 0f;
        float currentAlpha = mAxisMat.GetFloat("_Alpha");

        while (process < 1.0f)
        {
            process += Time.deltaTime;
            currentAlpha = Mathf.Lerp(currentAlpha, isForward ? 1.0f : 0.5f, process);

            mAxisMat.SetFloat("_Alpha", currentAlpha);

            yield return null;
        }
    }
}
