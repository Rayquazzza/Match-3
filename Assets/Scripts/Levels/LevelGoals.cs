[System.Serializable]
public class LevelGoals
{
    public int scoreToFirstStar;
    public int scoreToSecondStar;
    public int scoreToThirdStar;

    public int GetStarsEarned(int currentScore)
    {
        if (currentScore >= scoreToThirdStar) return 3;
        if (currentScore >= scoreToSecondStar) return 2;
        if (currentScore >= scoreToFirstStar) return 1;
        return 0;
    }
}