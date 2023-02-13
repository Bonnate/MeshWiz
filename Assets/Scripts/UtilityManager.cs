using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UtilityManager : MonoBehaviour
{
    //자식 레이어 전체 레이어를 변경하는 함수
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //비활성화된 자식 컴포넌트를 찾음
    static public TType GetComponentInChildren<TType>(GameObject objRoot) where TType : Component
    {
        // if we don't find the component in this object 
        // recursively iterate children until we do
        TType tRetComponent = objRoot.GetComponent<TType>();

        if (null == tRetComponent)
        {
            // transform is what makes the hierarchy of GameObjects, so 
            // need to access it to iterate children
            Transform trnsRoot = objRoot.transform;
            int iNumChildren = trnsRoot.childCount;

            // could have used foreach(), but it causes GC churn
            for (int iChild = 0; iChild < iNumChildren; ++iChild)
            {
                // recursive call to this function for each child
                // break out of the loop and return as soon as we find 
                // a component of the specified type
                tRetComponent = GetComponentInChildren<TType>(trnsRoot.GetChild(iChild).gameObject);
                if (null != tRetComponent)
                {
                    break;
                }
            }
        }

        return tRetComponent;
    }

    /// <summary>
    /// 대상 트랜스폼을 지정한 위치로 옮김, 대상의 피벗을 무시한 채 박스콜라이더의 최하단부분을 기준으로 설정
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="destination"></param>
    /// <param name="boxCollider"></param>
    public static void SetPositionViaBottom(Transform targetTransform, Vector3 destination)
    {
        bool isColliderExists = true;

        BoxCollider collider = targetTransform.GetComponent<BoxCollider>();
        if (collider == null)
        {
            isColliderExists = false;
            collider = targetTransform.gameObject.AddComponent<BoxCollider>();
        }

        Quaternion prevRot = targetTransform.rotation; //회전각 임시 저장
        targetTransform.rotation = Quaternion.identity;

        var min = collider.center - collider.size * 0.5f;
        var max = collider.center + collider.size * 0.5f;
        var P000 = targetTransform.TransformPoint(new Vector3(min.x, min.y, min.z));

        float delta = (targetTransform.position.y - P000.y); //피벗과 하단위치 차이 크기 구하기

        targetTransform.position = destination; //일단 대상을 피벗기준으로 해당 위치로 옮김
        targetTransform.Translate(Vector3.up * delta, Space.World); //차이만큼 보정

        targetTransform.rotation = prevRot;

        if (!isColliderExists)
        {
            Debug.Log("콜라이더 파괴");
            Destroy(collider);
        }        
    }

    public static void SetPositionViaCenter(Transform targetTransform, Vector3 destination)
    {
        bool isColliderExists = true;

        BoxCollider collider = targetTransform.GetComponent<BoxCollider>();
        if (collider == null)
        {
            isColliderExists = false;
            collider = targetTransform.gameObject.AddComponent<BoxCollider>();
        }

        targetTransform.transform.position = destination;

        targetTransform.Translate(targetTransform.position - targetTransform.TransformPoint(collider.center), Space.World);

        if (!isColliderExists)
        {
            Debug.Log("콜라이더 파괴");
            Destroy(collider);
        }
    }
}
