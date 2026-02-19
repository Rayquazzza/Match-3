using System.Collections.Generic;
using UnityEngine;

public class EffectService : MonoBehaviour, IEffectService
{
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private GameObject colorBombEffectPrefab;

    private void Awake()
    {
        GameServiceLocator.Register<IEffectService>(this);
    }

    public void PlayExplosion(Vector3 position)
    {
        if(explosionParticlesPrefab)
        {
            GameObject effect = GameServiceLocator.Get<IPoolingService>().GetFromPool(explosionParticlesPrefab, position, Quaternion.identity);
            effect.transform.position = position;

            var ps = effect.GetComponentInChildren<ParticleSystem>();
            if(ps != null)
            {
                var main = ps.main;
                ps.Play();
            }
            
        }
        
    }

    public void ShowScorePopup(Vector3 position, int score)
    {
        if(scoreTextPrefab)
        {
            GameObject popupGo = GameServiceLocator.Get<IPoolingService>().GetFromPool(scoreTextPrefab, position, Quaternion.identity);

            popupGo.transform.position = position;

            PopupScore popupScript = popupGo.GetComponent<PopupScore>();
            if (popupScript != null)
            {
                popupScript.Setup(score);
            }
        }
        
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IEffectService>();
    }

    public void PlayColorBombEffect(Vector3 position, List<Vector3> targets, float travelTime)
    {
       Debug.Log("Playing color bomb effect at " + position + " affecting " + targets.Count + " targets for " + travelTime + " seconds.");

       if(colorBombEffectPrefab)
       {
            foreach (var target in targets)
            {
                GameObject effect = GameServiceLocator.Get<IPoolingService>().GetFromPool(colorBombEffectPrefab, position, Quaternion.identity);
                effect.transform.position = position;
                LightingBoltEffect colorBombScript = effect.GetComponent<LightingBoltEffect>();
                if (colorBombScript != null)
                {
                    colorBombScript.Setup(target, travelTime);
                }
            }
            
        }

    }
}