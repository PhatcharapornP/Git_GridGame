using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //TODO: Handling game starts / game over etc
    [SerializeField] private PlayerScore playerScore;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private ObjectPoolManager objectPoolManager;

    public ObjectPoolManager Pool => objectPoolManager;
    public BoardManager Board => boardManager;
    public PlayerScore Score => playerScore;
    
    public Tweaks gameTweak;
    public List<Color> ColorPool = new List<Color>();


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Pool.Initialize(() =>
        {
            Debug.Log($"Pool Initailize callback");
            boardManager.GenerateBoard();
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
