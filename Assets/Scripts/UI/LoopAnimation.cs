using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class LoopAnimation : MonoBehaviour
{
    [SerializeField] private float offset = 20f;

    [SerializeField] private float duration = 1.5f;

    private Vector3 initialPosition;
    private RectTransform rectTransform;
    private bool positionCaptured = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // On cache l'objet (scale 0) pour préparer le "Boing"
        rectTransform.localScale = Vector3.zero;
        StartCoroutine(DelayedLaunch());
    }

    private IEnumerator DelayedLaunch()
    {
        // Attend que l'UI soit bien placée par les Layout Groups
        yield return new WaitForEndOfFrame();

        if (!positionCaptured)
        {
            initialPosition = rectTransform.anchoredPosition;
            positionCaptured = true;
        }

        LaunchFullAnimation();
    }

    private void LaunchFullAnimation()
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = initialPosition;

        // Création d'une séquence
        Sequence s = DOTween.Sequence();

        // 1. L'effet BOING (Apparition avec rebond)
        s.Append(rectTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // 2. La LOOP (Flottement infini)
        // On l'ajoute après le scale
        s.AppendCallback(() => {
            rectTransform.DOAnchorPosY(initialPosition.y + offset, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
    }
}