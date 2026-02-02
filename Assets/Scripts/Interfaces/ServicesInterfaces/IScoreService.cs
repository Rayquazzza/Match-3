using System;

public interface IScoreService
{
    public event Action<int> OnScoreUpdated;

    public void AddScore(int scoreToAdd);
    void Init();
    public void Unregister();

}