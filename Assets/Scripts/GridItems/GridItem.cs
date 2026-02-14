using DG.Tweening;
using UnityEngine;

public abstract class GridItem : MonoBehaviour
{
    protected GridController controller;
    protected GridData grid;
    protected GridItemData itemData;
    public abstract bool IsMovable { get; }
    public abstract bool IsMatchable { get; }

    protected CandyItemData candyType;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    public virtual void Init(GridController gridController)
    {
        controller = gridController;
        grid = controller.Grid;
    }

    public virtual void SetType(GridItemData data)
    {
        itemData = data;
        spriteRenderer.sprite = data.icon;

    }

    public virtual void OnNearbyMatch()
    {
        // Empty by default
    }

    public virtual GridItemData GetItemType()
    {
        return itemData;
    }

    public virtual void OnDestroyItem(int multiplier = 1)
    {
        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>{GetComponent<PoolMember>().ReturnToPool();});
    }

    public virtual void BreakLayer()
    {
        // Empty by default
    }
}