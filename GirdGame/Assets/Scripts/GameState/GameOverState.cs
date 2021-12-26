using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverState : BaseState
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highestText;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        quitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
            EndState();
            GameManager.Instance.State.GetStateViaType(typeof(MenuState)).StartState();
            #else
            Application.Quit();
            #endif
            
        });
        
        restartButton.onClick.AddListener(() =>
        {
            EndState();
            GameManager.Instance.State.GetStateViaType(typeof(GameState)).StartState();
        });
    }

    protected override void OnStartState()
    {
        base.OnStartState();
        scoreText.text = $"Your score is {GameManager.Instance.Score.Score} !";
    }

    protected override void OnEndState()
    {
        base.OnEndState();
        
    }
}
