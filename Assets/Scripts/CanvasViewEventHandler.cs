using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasViewEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 캔버스 영역으로 포인터가 들어왔는가?
    /// </summary>
    public static bool IsPointerEnter { private set; get; }
    public static Coroutine? mCoPointerEvent;

    private IEnumerator CoPointerEvent()
    {
        while (true)
        {
            if(Input.GetMouseButtonDown(0))
                break;

            yield return null;
        }

        IsPointerEnter = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mCoPointerEvent is not null)
            StopCoroutine(mCoPointerEvent);
        mCoPointerEvent = StartCoroutine(CoPointerEvent());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerEnter = false;

        if (mCoPointerEvent is not null)
            StopCoroutine(mCoPointerEvent);
    }
}
