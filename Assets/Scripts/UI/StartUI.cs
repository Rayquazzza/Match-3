using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged += GameStateChanged;
    }

    private void GameStateChanged(E_GameState state)
    {
        if(state == E_GameState.MENU)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    public void StartGame()
    {
        GameServiceLocator.Get<ITransitionService>().TransitionToState(E_GameState.LEVEL_SELECTION);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        GameServiceLocator.Get<IGameStateService>().OnGameStateChanged -= GameStateChanged;
    }
}
