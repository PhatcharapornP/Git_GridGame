using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int Score { get; private set; }

    public void SetPlayerScore(int targetScore)
    {
        Score = targetScore;
    }
}
