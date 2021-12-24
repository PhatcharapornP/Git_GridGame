using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public int Score { get; private set; }

    public void SetPlayerScore(int targetScore)
    {
        Score = targetScore;
        scoreText.text = $"Score: {Score}";
    }
}
