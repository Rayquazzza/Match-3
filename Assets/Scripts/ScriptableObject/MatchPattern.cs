using UnityEngine;

[CreateAssetMenu(fileName = "Match Pattern", menuName = "Match3/MatchPattern")]
public class MatchPattern : ScriptableObject
{
    public MatchShape shape;
    public int minCount;
    public int priority = 1;
    public GridItemData patternData; 
}