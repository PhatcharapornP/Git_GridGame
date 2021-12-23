using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //TODO: Handling game starts / game over etc
    [SerializeField] private PlayerScore playerScore;
    public Tweaks gameTweak;
    public List<Color> ColorPool = new List<Color>();


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
