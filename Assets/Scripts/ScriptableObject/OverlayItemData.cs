using UnityEngine;

[CreateAssetMenu(menuName = "Match3/Items/OverlayItem")]
public class OverlayItemData : ScriptableObject
{
    public string overlayName;
    public Sprite icon;
    public GameObject prefab;
    public int health = 1;
    public bool blocksMovement;
}
