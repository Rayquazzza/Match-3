using DG.Tweening;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Candy : GridItem
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    private Vector3 initialScale;
    private bool hasCachedScale = false;

    public override bool IsMovable => true;

    public override bool IsMatchable => true;

    private bool isBeingDestroyed = false;

    private void Awake()
    {
        // Register the normal scale once;
        CacheInitialScale();
    }

    // effect by default
    public virtual void ExecuteEffect(Vector2Int myPos)
    {
        grid.ClearMatchAt(myPos.x, myPos.y);
    }

    public virtual bool TriggerSpecialEffect(Candy swappedWith)
    {
        // Return false by default
        return false;
    }

    private void CacheInitialScale()
    {
        if (!hasCachedScale)
        {
            initialScale = transform.localScale;
            hasCachedScale = true;
        }
    }

    public void SetType(CandyItemData type)
    {
        candyType = type;

        if (spriteRenderer != null && candyType != null)
        {
            spriteRenderer.sprite = candyType.icon;
        }
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

    public override void OnDestroyItem()
    {
        Destroy();
    }
    public virtual void Destroy()
    {
        GameServiceLocator.Get<IEffectService>().PlayExplosion(transform.position);
        if (isBeingDestroyed) return; // Sécurité anti-double destruction
        isBeingDestroyed = true;

        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {              
                GetComponent<PoolMember>().ReturnToPool();
            });
    }

}