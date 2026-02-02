using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupScore : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;

    public void Setup(int scoreValue)
    {
        transform.DOKill();
        scoreText.DOKill();

        scoreText.text = $"+{scoreValue}";
        scoreText.alpha = 1f;
        transform.localScale = Vector3.one;

        transform.DOMoveY(transform.position.y + 1f, 0.6f);

        scoreText.DOFade(0, 0.6f).SetDelay(0.2f).OnComplete(() => 
        {
            PoolMember member = GetComponent<PoolMember>();
            if (member != null)
            {
                transform.SetParent(null);
                GameServiceLocator.Get<IPoolingService>().ReturnToPool(member.myPrefab, gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        });
    }
}
