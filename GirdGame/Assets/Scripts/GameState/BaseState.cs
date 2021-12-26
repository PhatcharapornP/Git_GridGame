using UnityEngine;

public class BaseState : MonoBehaviour,IGameState
{
    [SerializeField] protected GameObject statePanel;
    
    public void Initialize() {OnInitialize();}

    public void StartState() {OnStartState();}

    public void EndState() {OnEndState();}

    protected virtual void OnInitialize() {statePanel.SetActive(false);}

    protected virtual void OnStartState() {statePanel.SetActive(true);}

    protected virtual void OnEndState() {statePanel.SetActive(false);}
}
