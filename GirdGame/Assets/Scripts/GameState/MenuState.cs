using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuState : BaseState
{
    [SerializeField] private TextMeshProUGUI HighesScore;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resetButton;
    
    protected override void OnInitialize()
    {
        base.OnInitialize();
        startGameButton.onClick.AddListener(() =>
        {
            EndState();
            GameManager.Instance.State.GetStateViaType(typeof(GameState)).StartState();
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        resetButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Score.ResetHighestScore(() => { SetHighestScoreText(); });   
        });
    }

    protected override void OnStartState()
    {
        base.OnStartState();
        SetHighestScoreText();
    }
    
    private void SetHighestScoreText()
    {
        HighesScore.text = $"Highest score: {GameManager.Instance.Score.HighestScore}";
    }
}
