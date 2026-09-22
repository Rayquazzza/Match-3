using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{

    [SerializeField] private GameObject root;

    private void Start()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(E_GameState state)
    {
        if (state == E_GameState.IN_GAME)
        {
            root.SetActive(true);
        }
        else
        {
            root.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged -= GameStateChanged;
    }
}
