using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    [SerializeField] private float timeLimit = 0;
    private float timer;
    private float minutes;
    private float secs;
    private bool counting;
    public TextMeshProUGUI timerText;

    public void ResetTimer()
    {
        timeLimit = GameManager.Instance.gameTweak.startTimer;
        timer = timeLimit;
    }

    public void AddTimeLimit(float time)
    {
        timer += time;
    }

    public void StartTimer(float time = 0)
    {
        counting = true;
        if (time != 0)
            timeLimit = time;

        timer = timeLimit;
        StartCoroutine(UpdateTimeText());
    }

    // Update is called once per frame
    void Update()
    {
        if (!counting)
            return;

        timer -= Time.deltaTime;
        minutes = Mathf.FloorToInt(timer / 60);
        secs = Mathf.FloorToInt(timer % 60);
        if (timer < 0)
        {
            timer = 0;
            counting = false;
            StopCoroutine(UpdateTimeText());
            GameManager.Instance.State.GetStateViaType(typeof(GameState)).EndState();
            GameManager.Instance.State.GetStateViaType(typeof(GameOverState)).StartState();
        }
    }

    IEnumerator UpdateTimeText()
    {
        while (counting)
        {
            timerText.text = "Time: "+string.Format("{0:0}:{1:00}", minutes,secs);
            yield return new WaitForSeconds (.2f);
        }
    }
}
