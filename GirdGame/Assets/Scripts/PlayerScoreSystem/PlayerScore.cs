using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public int HighestScore { get; private set; }
    
    public int Score { get; private set;}

    public void Initialize()
    {
        HighestScore = PlayerPrefs.GetInt(Globals.HighestScore);
    }

    public void SetPlayerScore(int targetScore)
    {
        Score += targetScore;
        CheckHighestScore();
        scoreText.text = "Score: "+Score;
    }

    public void ResetPlayerScore()
    {
        Score = 0;
        scoreText.text = "Score: "+Score;
    }

    public void CheckHighestScore()
    {
        if (Score > HighestScore)
            HighestScore = Score;
        
        PlayerPrefs.SetInt(Globals.HighestScore,HighestScore);
    }

    public void ResetHighestScore(UnityAction onReset)
    {
        HighestScore = 0;
        PlayerPrefs.SetInt(Globals.HighestScore,HighestScore);
        onReset?.Invoke();
    }
}
