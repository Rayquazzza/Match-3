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
   

    public void TransitionToState(E_GameState state)
    {
        StartCoroutine(TransitionToStateCoroutine(state));
    }
    public IEnumerator TransitionToStateCoroutine(E_GameState state)
    {
        transitionUI.SetVisible(true);

        yield return new WaitForSeconds(2.0f);

        GameServiceLocator.Get<IGameStateService>().ChangeGameState(state);

        yield return new WaitForSeconds(0.5f);
        transitionUI.SetVisible(false);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ITransitionService>();
    }
}
