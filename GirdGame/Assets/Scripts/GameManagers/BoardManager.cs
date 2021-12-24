using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class BoardManager : MonoBehaviour
{
    [Tooltip("Row amount minimum is floored at 5")] 
    [SerializeField] [Range(8,16)]private int rows = 8;

    [Tooltip("Collum amount minimum is floored at 5")]
    [SerializeField] [Range(8,16)]private int columns = 8;

    [SerializeField] [Min(35)] private int pieceSize = 35;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private int totalColumnAvailable;
    [SerializeField] private int totalRowsAvailable;
    
    [SerializeField] private float widthDiff;
    [SerializeField] private float heightDiff;
    
    [SerializeField] private Piece piecePrefab;
    public Piece[,] pieces;
    [SerializeField] private List<Piece> spawnedPieces = new List<Piece>();
    
    private RectTransform _rectTransform;
    private Vector3 positionOffset;
    private Vector3 center;
    private float parentWidth;
    private float parentHeight;
    private float cellWidth;
    private float cellHeight;

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Space))
        {
            columns = Random.Range(8, 16);
            rows = Random.Range(8, 16);
            GenerateBoard();
        }
        #endif
    }
    
    private void CheckReference()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void GenerateBoard()
    {
        CheckReference();
        pieces = new Piece[columns, rows];
        GameManager.Instance.ColorPool.Clear();
        SetupColorPool();
        ClearBoard();
        CalculatePieceSize();
        SpawnPieces();
    }

    private void SetupColorPool()
    {
        if (columns * rows <= 64)
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
            {
                GameManager.Instance.ColorPool.Add(color);
            }
        }
        else if (columns * rows >= 65 && columns * rows <= 100)
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
                GameManager.Instance.ColorPool.Add(color);

            foreach (var color in GameManager.Instance.gameTweak.levelTwo)
                GameManager.Instance.ColorPool.Add(color);
        }
        else
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
                GameManager.Instance.ColorPool.Add(color);

            foreach (var color in GameManager.Instance.gameTweak.levelTwo)
                GameManager.Instance.ColorPool.Add(color);

            foreach (var color in GameManager.Instance.gameTweak.levelThree)
                GameManager.Instance.ColorPool.Add(color);
        }
    }

    private void CalculatePieceSize()
    {
        parentWidth = _rectTransform.rect.width;
        parentHeight = _rectTransform.rect.height;

        cellWidth = parentWidth / columns - ((spacing / columns) * 2);
        cellHeight = parentHeight / rows - ((spacing / rows) * 2);

        if (cellWidth < cellHeight)
            pieceSize = Mathf.FloorToInt(cellWidth );
        else 
            pieceSize = Mathf.FloorToInt(cellHeight) ;
        
        totalColumnAvailable = Mathf.FloorToInt(parentWidth / pieceSize);
        totalRowsAvailable = Mathf.FloorToInt(parentHeight / pieceSize);
        
        heightDiff = 0;
        heightDiff = totalRowsAvailable - rows;

        widthDiff = 0;
        widthDiff = totalColumnAvailable - columns;
    }
    
    private void SpawnPieces()
    {
        positionOffset = Vector3.zero;
        center = Vector3.zero;
        spawnedPieces.Clear();

        if (heightDiff > 0)
        {
            center.y = heightDiff * pieceSize / 2.0f;
            center.y = center.y + ((pieceSize - spacing) * heightDiff) / 2.0f;
        }

        if (widthDiff > 0)
            center.x = widthDiff * pieceSize / 2.0f;

        positionOffset = _rectTransform.position - center;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Piece piece = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.piece).GetComponent<Piece>();
                if (piece == null)
                {
                    Debug.LogError($"tried to get obj from pool with null Piece class!".InColor(Color.red),gameObject);
                    break;
                }
                piece.SetupPieceData(new Vector2Int(column, row));
                spawnedPieces.Add(piece);
                if (piece.transform.parent != transform)
                    piece.transform.SetParent(transform);

                var posX = (pieceSize * column) ;
                var posY = (pieceSize * row) ;
                piece.transform.localPosition = new Vector3(posX , posY, 0) - positionOffset;    
                piece.transform.localScale = new Vector3(pieceSize - spacing,pieceSize - spacing ,1);
                piece.gameObject.SetActive(true);
                pieces[column, row] = piece;
            }
        }
    }
    
    public bool CheckMatchesFromPiece(Piece targetPiece)
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();

        matchedPieces = FindColumnMatchFromPiece(targetPiece);
        matchedPieces.UnionWith(FindRowMatchFromPiece(targetPiece));

        List<Piece> matchedPieceList = new List<Piece>();
        foreach (var piece in matchedPieces)
        {
            matchedPieceList.Add(piece);
        }
        
        if (matchedPieceList.Count > 1)
        {
            foreach (var piece in matchedPieceList)
                piece.OnSelected();
            GameManager.Instance.Score.SetPlayerScore(matchedPieceList.Count);    
        }
        
        // Debug.Break();

        // foreach (var piece in matchedPieceList)
        // {
        //     matchedPieces.UnionWith(FindColumnMatchFromPiece(piece));
        //     matchedPieces.UnionWith(FindRowMatchFromPiece(piece));
        // }
        //
        // if (matchedPieceList.Count > 1)
        // {
        //     foreach (var piece in matchedPieceList)
        //         piece.OnSelected();
        //     GameManager.Instance.Score.SetPlayerScore(matchedPieceList.Count);    
        // }
        return matchedPieceList.Count > 0;
    }

    private HashSet<Piece> FindColumnMatchFromPiece(Piece targetPiece)
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();

        for (int i = targetPiece.Position.x; i < columns; i++)
        {
            Piece nextPiece = GetPieceAt(i, targetPiece.Position.y);
            if (targetPiece.PieceColor != nextPiece.PieceColor)
                break;
            matchedPieces.Add(nextPiece);
        }
        
        for (int i = targetPiece.Position.x; i >= 0; i--)
        {
            Piece nextPiece = GetPieceAt(i, targetPiece.Position.y);
            if (targetPiece.PieceColor != nextPiece.PieceColor)
            {
                break;
            }
            matchedPieces.Add(nextPiece);
        }

        return matchedPieces;
    }
    
    private HashSet<Piece> FindRowMatchFromPiece(Piece targetPiece)
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();

        for (int i = targetPiece.Position.y; i < rows; i++)
        {
            Piece nextPiece = GetPieceAt(targetPiece.Position.x,i);
            if (targetPiece.PieceColor != nextPiece.PieceColor)
            {
                break;
            }
            matchedPieces.Add(nextPiece);
        }
        
        for (int i = targetPiece.Position.y; i >= 0; i--)
        {
            Piece nextPiece = GetPieceAt(targetPiece.Position.x,i);
            if (targetPiece.PieceColor != nextPiece.PieceColor)
            {
                break;
            }
            matchedPieces.Add(nextPiece);
        }

        return matchedPieces;
    }

    public Piece GetPieceAt(int column,int row)
    {
        return pieces[column,row];
    }

    public void ClearBoard()
    {
        spawnedPieces.Clear();
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}