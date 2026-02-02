using UnityEngine;

[CreateAssetMenu(fileName = "Match Pattern", menuName = "Match3/MatchPattern")]
public class MatchPattern : ScriptableObject
{
    public int minCount;
    public int priority = 1;
    public GameObject bonusPrefab; 
}