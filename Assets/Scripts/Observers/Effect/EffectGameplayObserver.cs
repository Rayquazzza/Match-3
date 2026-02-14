using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectGameplayObserver : MonoBehaviour
{
    private IMatchService matchService;
    private void Start()
    {
        matchService = GameServiceLocator.Get<IMatchService>();

        matchService.OnMatchExecuted += OnMatch;
        matchService.OnItemDestroyed += OnItemDestroyed;
        matchService.OnColorBombEffect += OnColorBombEffect;
        matchService.OnScorePointGenerated += OnScorePointGenerated;
    }

    private void OnScorePointGenerated(Vector3 position, int score)
    {
        var effects = GameServiceLocator.Get<IEffectService>();
        effects.ShowScorePopup(position, score);
    }

    private void OnColorBombEffect(Vector3 vector, List<Vector3> list, float arg3)
    {
        GameServiceLocator.Get<IEffectService>().PlayColorBombEffect(vector, list, arg3);
    }

    private void OnMatch(MatchData data)
    {
        //var effects = GameServiceLocator.Get<IEffectService>();
        //effects.ShowScorePopup(data.worldPosition, data.score);
    }

    private void OnItemDestroyed(Vector3 position, CandyItemData data)
    {
        GameServiceLocator.Get<IEffectService>().PlayExplosion(position);
    }



    private void OnDestroy()
    {
        if (matchService != null) 
        {
            matchService.OnMatchExecuted -= OnMatch; 
            matchService.OnItemDestroyed -= OnItemDestroyed;
            matchService.OnColorBombEffect -= OnColorBombEffect;
            matchService.OnScorePointGenerated -= OnScorePointGenerated;
        }
    }
}
