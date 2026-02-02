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
        // 1. Afficher l'écran de chargement et lancer les sauts
        transitionUI.SetVisible(true);

        // 2. Attendre un peu pour que le joueur voie les bonbons sauter (ex: 2 secondes)
        yield return new WaitForSeconds(2.0f);

        // 3. Changer l'état du jeu (chargement de scène, reset score, etc.)
        GameServiceLocator.Get<IGameStateService>().ChangeGameState(state);

        // 4. Attendre la fin du chargement si nécessaire, puis cacher la transition
        yield return new WaitForSeconds(0.5f);
        transitionUI.SetVisible(false);
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ITransitionService>();
    }
}
