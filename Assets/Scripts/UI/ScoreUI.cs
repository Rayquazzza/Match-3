using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float duration = 0.5f;

    private int currentDisplayedScore = 0;

    private Tween scoreTween;

    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        GameServiceLocator.Get<IScoreService>().OnScoreUpdated += UpdateScore;

        scoreText.text = "Score : 0";
        initialScale = transform.localScale;
    }

    private void UpdateScore(int scoreToAdd)
    {
        scoreTween?.Kill();

        scoreTween = DOTween.To(() => currentDisplayedScore, x => currentDisplayedScore = x, scoreToAdd, duration).SetEase(Ease.OutQuad).OnUpdate(() =>
            {
                scoreText.text = $"Score : {currentDisplayedScore}";
            });

        //scoreText.transform.DOPunchScale(initialScale * 0.1f, 0.2f);

    }

    private void OnDestroy()
    {
        GameServiceLocator.Get<IScoreService>().OnScoreUpdated -= UpdateScore;
    }


}
