using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelProgress : MonoBehaviour
{
    [SerializeField] private Slider scoreBar;
    [SerializeField] private Image[] starIcons;

    private bool[] starsUnlocked = new bool[3];

    private ILevelService levelService;


    void Start()
    {
        levelService = GameServiceLocator.Get<ILevelService>();

        levelService.OnLoadLevelData += InitializeFromLevel;

        GameServiceLocator.Get<IScoreService>().OnScoreUpdated += UpdateScoreUI;

        if (levelService.CurrentLevelData != null)
        {
            InitializeFromLevel(levelService.CurrentLevelData);
        }
    }

    private void InitializeFromLevel(LevelData data)
    {
        if (data == null || data.goals == null) return;

        LevelGoals goals = data.goals;
        scoreBar.maxValue = goals.scoreToThirdStar;
        scoreBar.value = 0;

        float barWidth = scoreBar.GetComponent<RectTransform>().rect.width;

        int[] thresholds = { goals.scoreToFirstStar, goals.scoreToSecondStar, goals.scoreToThirdStar };

        for (int i = 0; i < starIcons.Length; i++)
        {
            float ratio = (float)thresholds[i] / goals.scoreToThirdStar;

            RectTransform starRect = starIcons[i].GetComponent<RectTransform>();

            float newX = (ratio * barWidth) - (barWidth / 2f);
            starRect.anchoredPosition = new Vector2(newX, starRect.anchoredPosition.y);

            starIcons[i].color = Color.gray;
        }

        starsUnlocked = new bool[starIcons.Length];
        for (int i = 0; i < starIcons.Length; i++)
        {
            starIcons[i].color = Color.gray;
            starIcons[i].transform.localScale = Vector3.one;
        }
    }

    private void UpdateScoreUI(int newScore)
    {
        if (levelService.CurrentLevelData == null) return;

        scoreBar.DOValue(newScore, 0.5f);

        int starsEarned = levelService.CurrentLevelData.goals.GetStarsEarned(newScore);

        for (int i = 0; i < starIcons.Length; i++)
        {
            if (i < starsEarned && !starsUnlocked[i])
            {
                starsUnlocked[i] = true;
                starIcons[i].color = Color.white;
                starIcons[i].transform.DOKill(true);
                starIcons[i].transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 5, 0.5f);
            }

            else if (i < starsEarned && starsUnlocked[i])
            {
                starIcons[i].color = Color.white;
            }
            else
            {
                starsUnlocked[i] = false;
                starIcons[i].color = Color.gray;
            }
        }
    }

    private void OnDestroy()
    {
        if (levelService != null) levelService.OnLoadLevelData -= InitializeFromLevel;
        GameServiceLocator.Get<IScoreService>().OnScoreUpdated -= UpdateScoreUI;
    }
}