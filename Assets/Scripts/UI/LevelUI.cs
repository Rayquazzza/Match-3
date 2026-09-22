using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour
{

    [SerializeField] private GameObject root;
    private void Start()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(E_GameState state)
    {
        if (state == E_GameState.LEVEL_SELECTION)
        {
            root.SetActive(true);
        }
        else
        {
            root.SetActive(false);
        }
    }
}
