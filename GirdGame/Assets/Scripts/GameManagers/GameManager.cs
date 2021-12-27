using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameStateManager gameStateManager;  
    [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private PlayerScore playerScore;
    [SerializeField] private BoardManager boardManager;
    public BoardManager Board => boardManager;
    public PlayerScore Score => playerScore;
    public ObjectPoolManager Pool => objectPoolManager;
    public GameStateManager State => gameStateManager;
    
    public Tweaks gameTweak;
    public List<Color> ColorPool = new List<Color>();

    void Awake()
    {
        Instance = this;
        playerScore.Initialize();
        Pool.Initialize(() => {gameStateManager.InitializeState();});
    }
}
