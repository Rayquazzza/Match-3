using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    private void Start()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(E_GameState state)
    {
        if (state == E_GameState.IN_GAME)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged -= GameStateChanged;
    }
}
