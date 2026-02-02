using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => LoadData(levelData));
    }

    private void LoadData(LevelData lvlData)
    {
        if (levelData !=null)
        {
            GameServiceLocator.Get<ILevelService>().LoadLevelData(lvlData, E_GameState.IN_GAME);          
        }

    }
}
