using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuState : BaseState
{
    [SerializeField] private TextMeshProUGUI HighesScore;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitButton;
    
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
    }
}
