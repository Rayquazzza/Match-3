using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform container; 
    [SerializeField] private List<LevelData> allLevels;

    private void OnEnable()
    {
        GenerateMenu();
    }

    private void GenerateMenu()
    {
        foreach (Transform child in container) Destroy(child.gameObject);

        List<LevelData> sortedLevels = allLevels.OrderBy(l => l.levelID).ToList();

        bool previousLevelPassed = true; 

        foreach (LevelData level in sortedLevels)
        {
            GameObject btnObj = Instantiate(buttonPrefab, container);
            LevelButton btnScript = btnObj.GetComponent<LevelButton>();

            bool isUnlocked = (level.levelID == 1) || previousLevelPassed;

            btnScript.Setup(level, isUnlocked);

            int stars = PlayerPrefs.GetInt($"Level_{level.levelID}_Stars", 0);
            previousLevelPassed = (stars > 0);
        }
    }
}
