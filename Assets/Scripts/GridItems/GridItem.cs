using DG.Tweening;
using UnityEngine;

public abstract class GridItem : MonoBehaviour
{
    protected GridController controller;
    protected GridData grid;
    public abstract bool IsMovable { get; }
    public abstract bool IsMatchable { get; }

    [SerializeField] protected CandyItemData candyType;

    public virtual void Init(GridController gridController)
    {
        controller = gridController;
        grid = controller.Grid;
    }

    public virtual void OnNearbyMatch()
    {
        // Empty by default
    }

    public virtual CandyItemData GetItemType()
    {
        return candyType;
    }

    public virtual void OnDestroyItem()
    {
        transform.DOKill();
        transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                GetComponent<PoolMember>().ReturnToPool();
            });
    }

    public virtual void BreakLayer()
    {
        // Empty by default
    }
}