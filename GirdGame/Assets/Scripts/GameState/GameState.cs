using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameState : BaseState
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button randomButton;
    private int startColumn;
    private int startRow;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        quitButton.onClick.AddListener(() => { OnQuitButton(); });
        
        randomButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Board.GenerateRandomizeBoard();
            randomButton.interactable = false;
            DOVirtual.DelayedCall(.5f, () =>
            {
                randomButton.interactable = true;
            });
        });

        startColumn = GameManager.Instance.gameTweak.startColumnSize;
        startRow = GameManager.Instance.gameTweak.startRowSize;
    }
    
    

    public void OnQuitButton()
    {
        EndState();
        GameManager.Instance.State.GetStateViaType(typeof(GameOverState)).StartState();
    }

    protected override void OnStartState()
    {
        base.OnInitialize();
        statePanel.gameObject.SetActive(true);
        GameManager.Instance.Timer.ResetTimer();
        GameManager.Instance.Score.ResetPlayerScore();
        GameManager.Instance.Board.SetBoardSize(startColumn,startRow);
        GameManager.Instance.Board.GenerateBoard();
        GameManager.Instance.Timer.StartTimer();
    }

    protected override void OnEndState()
    {
        base.OnEndState();
        GameManager.Instance.Board.ClearBoard();
    }
}
