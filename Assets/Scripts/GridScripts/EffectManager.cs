using UnityEngine;

public class EffectManager : MonoBehaviour, IEffectService
{
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private GameObject scoreTextPrefab; 

    private void Awake()
    {
        GameServiceLocator.Register<IEffectService>(this);
    }

    public void PlayExplosion(Vector3 position)
    {
        GameObject effect = GameServiceLocator.Get<IPoolingService>().GetFromPool(explosionParticlesPrefab, position, Quaternion.identity);
        effect.transform.position = position;

        var ps = effect.GetComponentInChildren<ParticleSystem>();
        var main = ps.main;

        ps.Play();
    }

    public void ShowScorePopup(Vector3 position, int score)
    {
        GameObject popupGo = GameServiceLocator.Get<IPoolingService>().GetFromPool(scoreTextPrefab, position, Quaternion.identity);

        popupGo.transform.position = position;

        PopupScore popupScript = popupGo.GetComponent<PopupScore>();
        if (popupScript != null)
        {
            popupScript.Setup(score);
        }
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IEffectService>();
    }
}