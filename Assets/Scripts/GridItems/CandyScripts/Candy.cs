using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Candy : GridItem
{

    private Vector3 initialScale;
    private bool hasCachedScale = false;

    public override bool IsMovable => true;

    public override bool IsMatchable => true;

    private bool isBeingDestroyed = false;

    private void Awake()
    {
        CacheInitialScale();
    }

    public virtual void ExecuteEffect(Vector2Int myPos)
    {
        grid.ClearMatchAt(myPos.x, myPos.y);
    }

    public virtual IEnumerator TriggerSpecialEffect(Candy swappedWith)
    {
        return null;
    }

    private void CacheInitialScale()
    {
        if (!hasCachedScale)
        {
            initialScale = transform.localScale;
            hasCachedScale = true;
        }
    }

    public override void SetType(GridItemData data)
    {
        base.SetType(data);
        candyType = data as CandyItemData;
    }

    private void OnEnable()
    {
        isBeingDestroyed = false;

        CacheInitialScale();
        transform.localScale = Vector3.zero;

        AnimateAppearance();
    }

    private void AnimateAppearance()
    {
        transform.DOKill();
        transform.DOScale(initialScale, 0.5f).SetEase(Ease.OutBack);
    }

    public override void OnDestroyItem(int multiplier = 1)
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;

        int baseScore = 50;
        int finalScore = baseScore * multiplier;


        GameServiceLocator.Get<IScoreService>()?.AddScore(finalScore);

        GameServiceLocator.Get<IMatchService>()?.NotifyItemDestroyed(transform.position, candyType);

        GameServiceLocator.Get<IMatchService>()?.NotifyScorePoint(transform.position, finalScore);

        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
            GetComponent<PoolMember>().ReturnToPool();
        });
    }

}