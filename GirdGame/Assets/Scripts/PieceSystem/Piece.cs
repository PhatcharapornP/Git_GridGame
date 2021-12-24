using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class Piece : MonoBehaviour,IPiece,IPoolObject
{
    [SerializeField] private Button button;
    public Vector2Int Position { get;  private set; }
    public Color PieceColor { get; private set; }
    
    public void InitializePoolObj()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { OnClickPiece(); });
    }

    public void SetupPieceData(Vector2Int pos)
    {
        Position = pos;
        name = $"P_{Position.x},{Position.y}";
        PieceColor = GameManager.Instance.ColorPool[Random.Range(0, GameManager.Instance.ColorPool.Count)];
        button.image.color = PieceColor;
    }

    public void OnClickPiece()
    {
        if (GameManager.Instance.Board.CheckMatchesFromPiece(this))
            Debug.Log($"OnClickPiece: {name} and found matches",gameObject);
    }

    public void OnSelected()
    {
        //TODO: Do something snake!?
        
        button.image.color = Color.grey;
    }

   
}