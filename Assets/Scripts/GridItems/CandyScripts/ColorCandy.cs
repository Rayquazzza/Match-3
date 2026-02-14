using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ColorCandy : Candy
{
    public override bool IsMatchable => false;

    public override IEnumerator TriggerSpecialEffect(Candy swappedWith)
    {
        CandyItemData targetData = swappedWith.GetItemType() as CandyItemData;
        List<Candy> targets = grid.GetCandiesOfColor(targetData);

        float travelTime = 0.5f;
        List<Vector3> targetPositions = targets.Select(t => t.transform.position).ToList();

        GameServiceLocator.Get<IMatchService>().NotifyColorBomb(transform.position, targetPositions, travelTime);

        //GameServiceLocator.Get<IMoveService>().PerformMatch(transform.position, 1);


        yield return new WaitForSeconds(travelTime);

        int colorCombo = 1;
        foreach (var target in targets)
        {
            Vector2Int pos = grid.GetPositionOf(target);
            if (pos.x != -1)
            {
                grid.ClearMatchAt(pos.x, pos.y, colorCombo);
            }
        }

        Vector2Int myPos = grid.GetPositionOf(this);
        Vector2Int swapPos = grid.GetPositionOf(swappedWith);

        grid.ClearMatchAt(swapPos.x, swapPos.y, 1);
        grid.ClearMatchAt(myPos.x, myPos.y, 1);
    }
}
