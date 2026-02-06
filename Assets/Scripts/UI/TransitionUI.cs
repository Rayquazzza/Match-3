using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [SerializeField] private Sprite[] allCandySprites;
    [SerializeField] private Image[] candyImages;

    [Header("Settings")]
    [SerializeField] private float jumpHeight = 50f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float delayBetweenCandies = 0.15f;
    [SerializeField] private float delayBetweenLoops = 1.5f; 

    [SerializeField] private GameObject visualRoot;

    [SerializeField] private CanvasGroup canvasGroup;

    void Start()
    {
        StartCoroutine(CandyLoopRoutine());
        visualRoot.SetActive(false);
    }

    public void SetVisible(bool isVisible)
    {
        IUIService UISvc = GameServiceLocator.Get<IUIService>();

        if (isVisible)
        {
            UISvc.ToggleRaycast(!isVisible);
            visualRoot.SetActive(isVisible);
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1, 1f);
            StartCoroutine(CandyLoopRoutine());

        }
        else
        {
            canvasGroup.DOFade(0f, 1f).OnComplete(() =>
            {
                visualRoot.SetActive(isVisible);
                StopAllCoroutines();
                UISvc.ToggleRaycast(!isVisible);
            });
        }
    }

    IEnumerator CandyLoopRoutine()
    {
        yield return new WaitForEndOfFrame();

        foreach (var img in candyImages)
        {
            img.sprite = allCandySprites[Random.Range(0, allCandySprites.Length)];
            img.transform.localScale = Vector3.one;
            img.type = Image.Type.Simple;
            img.preserveAspect = true;
        }

        while (visualRoot.activeSelf == true)
        {
            foreach (var img in candyImages)
            {
                float startY = img.rectTransform.anchoredPosition.y;

                Sequence s = DOTween.Sequence();
                s.Append(img.rectTransform.DOAnchorPosY(startY + jumpHeight, duration / 2).SetEase(Ease.OutQuad));
                s.Append(img.rectTransform.DOAnchorPosY(startY, duration / 2).SetEase(Ease.InQuad));

                yield return new WaitForSeconds(delayBetweenCandies);
            }

            yield return new WaitForSeconds(delayBetweenLoops);
        }
    }
}