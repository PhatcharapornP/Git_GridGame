using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameState : BaseState
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button randomButton;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        quitButton.onClick.AddListener(() =>
        {
            EndState();
            GameManager.Instance.State.GetStateViaType(typeof(GameOverState)).StartState();
        });
        
        randomButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Board.GenerateRandomizeBoard();
            randomButton.interactable = false;
            DOVirtual.DelayedCall(.5f, () =>
            {
                randomButton.interactable = true;
            });
        });
    }

    protected override void OnStartState()
    {
        base.OnInitialize();
        statePanel.gameObject.SetActive(true);
        GameManager.Instance.Score.SetPlayerScore(0);
        GameManager.Instance.Board.GenerateBoard();
    }

    protected override void OnEndState()
    {
        base.OnEndState();
        GameManager.Instance.Board.ClearBoard();
    }
}
