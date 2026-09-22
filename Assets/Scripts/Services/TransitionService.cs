using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransitionService : MonoBehaviour, ITransitionService
{
    [SerializeField] private TransitionUI transitionUI;
    private void Awake()
    {
        GameServiceLocator.Register<ITransitionService>(this);
    }

    private void Start()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
        
    }

    private void GameStateChanged(E_GameState state)
    {
        if(state == E_GameState.GAME_ENDED)
        {
            TransitionToState(E_GameState.LEVEL_SELECTION);
        }
    }

    public void TransitionToState(E_GameState state)
    {
        StartCoroutine(TransitionToStateCoroutine(state));
    }
    public IEnumerator TransitionToStateCoroutine(E_GameState state)
    {
        if (transitionUI) transitionUI.SetVisible(true);

        yield return new WaitForSeconds(2.0f);

        GameServiceLocator.Get<IGameStateService>().ChangeGameState(state);

        yield return new WaitForSeconds(0.5f);
        if (transitionUI) transitionUI.SetVisible(false);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ITransitionService>();
    }
}
