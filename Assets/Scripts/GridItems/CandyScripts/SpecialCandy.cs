using System.Collections;
using UnityEngine;

public class SpecialCandy : Candy
{
    public SpecialEffect Effect {get; private set; }

    public override void SetType(GridItemData data)
    {
        base.SetType(data);

        if (data is SpecialCandyData specialData)
        {
            Effect = specialData.effect;
        }
    }

    public override void OnDestroyItem(int multiplier = 1)
    {
        Vector2Int myPos = grid.GetPositionOf(this);

        if (Effect != null && myPos.x != -1)
        {
            Debug.Log($"Activation de l'effet : {Effect.name} en {myPos}");
            Effect.Execute(myPos.x, myPos.y, grid, controller);
        }

        base.OnDestroyItem();
    }

    public override IEnumerator TriggerSpecialEffect(Candy swappedWith)
    {
        if (swappedWith is ColorCandy) return null;

        if (swappedWith is SpecialCandy other)
        {
            if (Effect != null)
            {
                Effect.ExecuteCombo(this, other, grid, controller);
            }

            this.OnDestroyItem();
            other.OnDestroyItem();
            return null;
        }

        return null ;
    }
}