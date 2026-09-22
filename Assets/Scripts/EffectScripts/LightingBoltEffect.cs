using UnityEngine;
using DG.Tweening; 

public class LightingBoltEffect : MonoBehaviour
{
    public void Setup(Vector3 targetPos, float duration)
    {
        transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic).OnComplete(() =>{ DOVirtual.DelayedCall(0.1f, () => GetComponent<PoolMember>()?.ReturnToPool());});
    }
}