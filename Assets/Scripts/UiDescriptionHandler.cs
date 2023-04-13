using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UiDescriptionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Fixed UI의 페이드 시간
    /// </summary>
    public static readonly float _FIXED_UI_FADE_DURATION = 0.1f;

    [SerializeField] private GameObject[] mFixedUiGos;

    private List<CanvasGroup> mFixedUiCanvasGroup = new List<CanvasGroup>(); // 고정 UI의 캔버스 그룹
    private Coroutine? mCoFadeFixedUi; // 페이드 인 아웃 코루틴 \

    private void Awake()
    {
        foreach(GameObject fixedUiGo in mFixedUiGos)
        {
            CanvasGroup? canvasGroup = fixedUiGo.GetComponentInChildren<CanvasGroup>(true);

            if(canvasGroup is null)
                Debug.LogError($"{fixedUiGo}의 하위 오브젝트에는 캔버스그룹이 없음!");

            canvasGroup.alpha = 0f;
            fixedUiGo.SetActive(false);

            mFixedUiCanvasGroup.Add(canvasGroup);
        }
    }

    private void FadeFixedUi(bool isEnable)
    {
        if (mCoFadeFixedUi is not null)
            StopCoroutine(mCoFadeFixedUi);
        mCoFadeFixedUi = StartCoroutine(CoFadeFixedUi(isEnable));
    }

    private IEnumerator CoFadeFixedUi(bool isEnable)
    {
        foreach(GameObject fixedUiGo in mFixedUiGos)
            fixedUiGo.SetActive(true);

        // 알파값 가져오기
        float currentAlpha = mFixedUiCanvasGroup[0].alpha;

        float process = 0f;
        while (process < 1f)
        {
            process += Time.deltaTime / _FIXED_UI_FADE_DURATION;
            foreach(CanvasGroup canvasGroup in mFixedUiCanvasGroup)
                canvasGroup.alpha = Mathf.Lerp(currentAlpha, isEnable ? 1 : 0, process);

            yield return null;
        }

        if (!isEnable)
            foreach (GameObject fixedUiGo in mFixedUiGos)
                fixedUiGo.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        FadeFixedUi(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        FadeFixedUi(false);
    }
}
