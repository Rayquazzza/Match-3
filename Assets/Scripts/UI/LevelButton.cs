using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CanvasGroup))]
public class LevelButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    private int levelID;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.black;

    [SerializeField] private TextMeshProUGUI levelNumberText;

    private Button button;

    [SerializeField] private Image[] starIcons;

    private void OnEnable()
    {
        
    }
    public void Setup(LevelData levelData, bool isUnlocked)
    {

        this.levelData = levelData;
        levelID = levelData.levelID;
        levelNumberText.text = levelData.levelID.ToString();

        UpdateStarUI();

        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.interactable = isUnlocked;
        GetComponent<CanvasGroup>().alpha = isUnlocked ? 1f : 0.5f;
        button.onClick.AddListener(() => LoadData(levelData));
        
    }

    private void UpdateStarUI()
    {
        int starsEarned = PlayerPrefs.GetInt($"Level_{levelData.levelID}_Stars", 0);

        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].color = (i < starsEarned) ? unlockedColor : lockedColor;
        }
    }

    private void LoadData(LevelData lvlData)
    {
        if (levelData !=null)
        {
            GameServiceLocator.Get<ILevelService>().LoadLevelData(lvlData, E_GameState.IN_GAME);          
        }

    }
}
