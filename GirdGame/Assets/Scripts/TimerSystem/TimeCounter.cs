using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private bool startTween = false;
    public TextMeshProUGUI timerText;

    private void Start()
    {
        DOTween.defaultAutoKill = false;
    }

    public void ResetTimer()
    {
        timeLimit = GameManager.Instance.gameTweak.startTimer;
        timer = timeLimit;
        startTween = false;
        timerText.color = Color.white;
    }

    private Color tempColor;

    public void AddTimeLimit(float time, int color = -1)
    {
        timer += time;
        timerText.transform.DOShakeScale(.15f, .5f);

        if (color == -1)
            tempColor = Color.green;
        else
            tempColor = GameManager.Instance.ColorPool[color];
        timerText.DOColor(tempColor, .5f).OnStepComplete(()=> {timerText.color = Color.white;});
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
        
        if (timer < 10 && !startTween)
        {
            startTween = true;
            timerText.DOColor(Color.red, .5f).SetLoops(-1,LoopType.Yoyo);
            timerText.transform.DOShakeScale(.5f, .2f).SetLoops(-1,LoopType.Yoyo);
        }

        if (timer > 10 && startTween)
        {
            timerText.DOKill();
            timerText.transform.DOKill();
            timerText.color = Color.white;
            startTween = false;
        }
        
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
