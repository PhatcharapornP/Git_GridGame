using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BoardManager : MonoBehaviour
{
    [SerializeField] private Transform borderParent;
    [SerializeField] private Vector3 positionOffset;

    [Tooltip("Row amount minimum is floored at 5")] [SerializeField] [Range(5, 16)]
    private int rows = 8;

    [Tooltip("Collum amount minimum is floored at 5")] [SerializeField] [Range(5, 16)]
    private int columns = 8;

    [SerializeField] [Min(35)] private int pieceSize = 35;
    [SerializeField] private int totalColumnAvailable;
    [SerializeField] private int totalRowsAvailable;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float widthDiff;
    [SerializeField] private float heightDiff;
    [SerializeField] private List<Piece> spawnedPieces = new List<Piece>();
    [SerializeField] private List<GameObject> spawnedBorder = new List<GameObject>();
    [SerializeField] private List<Piece> matchedPieces = new List<Piece>();
    private List<Vector2Int> searchedPos = new List<Vector2Int>();

    private Piece[,] pieces;
    private RectTransform _rectTransform;
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
            columns = 8;
            rows = 8;
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
                GameManager.Instance.ColorPool.Add(color);
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
            pieceSize = Mathf.FloorToInt(cellWidth);
        else
            pieceSize = Mathf.FloorToInt(cellHeight);

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


        for (int column = 0; column < columns; column++)
        for (int row = 0; row < rows; row++)
        {
            Piece piece = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.piece).GetComponent<Piece>();
            if (piece == null)
            {
                Debug.LogError($"tried to get obj from pool with null Piece class!".InColor(Color.red), gameObject);
                break;
            }

            spawnedPieces.Add(piece);
            if (piece.transform.parent != transform)
                piece.transform.SetParent(transform);

            var posX = (pieceSize * column);
            var posY = (pieceSize * row);

            piece.transform.localPosition = new Vector3(posX, posY + parentHeight, 0) - positionOffset;
            piece.transform.localScale = new Vector3(pieceSize - spacing, pieceSize - spacing, 1);
            piece.gameObject.SetActive(true);
            piece.SetupPieceData(new Vector2Int(column, row), new Vector3(posX, posY, 0) - positionOffset);
            pieces[column, row] = piece;

            SpawnBorderToPosition(posX, posY);
        }
    }

    private void SpawnBorderToPosition(int posX, int posY)
    {
        GameObject border = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.border);
        border.transform.SetParent(borderParent);
        border.transform.localPosition = new Vector3(posX, posY, 0) - positionOffset;
        border.transform.localScale = new Vector3(pieceSize - spacing, pieceSize, 1);
        border.gameObject.SetActive(true);
        spawnedBorder.Add(border);
    }

    public bool CheckMatchesFromPiece(Piece targetPiece)
    {
        searchedPos.Clear();
        matchedPieces.Clear();

        matchedPieces = GetMatchList(targetPiece);

        while (matchedPieces.Any(x => FindColumnMatchFromPiece(x).Count >= 1) ||
               matchedPieces.Any(x => FindRowMatchFromPiece(x).Count >= 1))
        {
            for (int i = 0; i < matchedPieces.Count; i++)
            {
                var temp = GetMatchList(matchedPieces[i]);
                if (temp == null)
                    continue;
                for (int j = 0; j < temp.Count; j++)
                {
                    if (matchedPieces.Contains(temp[j]) == false)
                        matchedPieces.Add(temp[j]);
                }
            }
        }

        if (matchedPieces.Count > 1)
        {
            foreach (var piece in matchedPieces)
                piece.OnSelected();
        }
        Debug.Log($"CheckMatches at: {targetPiece.Position} got matchedPiece: {matchedPieces.Count}".InColor(targetPiece.PieceColor),targetPiece.gameObject);

        return matchedPieces.Count > 1;
    }

    public void FillEmptyPositions()
    {
        for (int column = 0; column < columns; column++)
        for (int row = 0; row < rows; row++)
        {
            while (pieces[column, row].IsSelected)
            {
                Debug.Log($"------- Found empty at: {pieces[column, row].Position} apply to current -------".InColor(Color.red));
                Piece current = pieces[column, row];
                Piece next = current;
                Vector2Int tempPos = next.Position;
                // Debug.Break();
                for (int filler = row; filler < columns - 1; filler++)
                {
                    Debug.Log($"next was: {next.gameObject.name} and IsSelected: {next.IsSelected}".InColor(current.PieceColor), current.gameObject);
                    next = pieces[column, filler + 1];
                    Debug.Log($"next is: {next.gameObject.name} and IsSelected: {next.IsSelected} apply to current".InColor(next.PieceColor),current.gameObject);
                    current = next;
                    tempPos = next.Position;
                    
                    var posX = (pieceSize * column);
                    var posY = (pieceSize * filler);
                    current.MoveToTargetPos(new Vector3(posX, posY, 0) - positionOffset);
                    current.OverwritePos(new Vector2Int(column,filler));
                   
                    pieces[column, filler] = current;
                    
                }
                
                var newPiece = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.piece).GetComponent<Piece>();
                var tempX = (pieceSize * tempPos.x);
                var tempY = (pieceSize * tempPos.y);
                newPiece.transform.localPosition = new Vector3(tempX, tempY + parentHeight, 0) - positionOffset;
                newPiece.SetupPieceData(new Vector2Int(tempPos.x,tempPos.y),new Vector3(tempX, tempY, 0) - positionOffset,false);
                pieces[tempPos.x, tempPos.y] = newPiece;
                
            }
        }
        spawnedPieces.Clear();
        foreach (var piece in pieces)
            spawnedPieces.Add(piece);
    }

    private List<Piece> GetMatchList(Piece targetPiece)
    {
        //Detect if the piece has already been searched
        if (searchedPos.Contains(targetPiece.Position))
            return null;

        searchedPos.Add(targetPiece.Position);
        List<Piece> tempMatches = new List<Piece>();
        var horizontalMatches = FindColumnMatchFromPiece(targetPiece);
        var verticalMatches = FindRowMatchFromPiece(targetPiece);

        if (horizontalMatches.Count >= 1)
        {
            foreach (var piece in horizontalMatches)
            {
                if (tempMatches.Contains(piece) == false)
                    tempMatches.Add(piece);
            }
        }
        else
            Debug.LogWarning($"horizontalMatches from: {targetPiece.Position} is none",targetPiece);


        if (verticalMatches.Count >= 1)
        {
            foreach (var piece in verticalMatches)
                if (tempMatches.Contains(piece) == false)
                    tempMatches.Add(piece);
        }
        else
            Debug.LogWarning($"verticalMatches from: {targetPiece.Position} is none",targetPiece);
            
        return tempMatches;
    }

    private List<Piece> FindColumnMatchFromPiece(Piece targetPiece)
    {
        List<Piece> tempMatches = new List<Piece>();

        for (int i = targetPiece.Position.x; i < columns; i++)
        {
            Piece nextPiece = pieces[i, targetPiece.Position.y];
            if (targetPiece.PieceColor == nextPiece.PieceColor)
            {
                if (matchedPieces.Contains(nextPiece) == false)
                    tempMatches.Add(nextPiece);
            }
            else break;
        }

        for (int i = targetPiece.Position.x; i >= 0; i--)
        {
            Piece nextPiece = pieces[i, targetPiece.Position.y];
            if (targetPiece.PieceColor == nextPiece.PieceColor)
            {
                if (matchedPieces.Contains(nextPiece) == false)
                    tempMatches.Add(nextPiece);
            }
            else break;
        }

        return tempMatches;
    }

    private List<Piece> FindRowMatchFromPiece(Piece targetPiece)
    {
        List<Piece> tempMatches = new List<Piece>();

        for (int i = targetPiece.Position.y; i < rows; i++)
        {
            Piece nextPiece = pieces[targetPiece.Position.x, i];
            if (targetPiece.PieceColor == nextPiece.PieceColor)
            {
                if (matchedPieces.Contains(nextPiece) == false)
                    tempMatches.Add(nextPiece);
            }
            else break;
        }

        for (int i = targetPiece.Position.y; i >= 0; i--)
        {
            Piece nextPiece = pieces[targetPiece.Position.x, i];
            if (targetPiece.PieceColor == nextPiece.PieceColor)
            {
                if (matchedPieces.Contains(nextPiece) == false)
                    tempMatches.Add(nextPiece);
            }
            else break;
        }

        return tempMatches;
    }

    public void ClearBoard()
    {
        ClearSpawnedPieces();
        ClearSpawnedBorders();
    }

    private void ClearSpawnedBorders()
    {
        spawnedBorder.Clear();
        for (int i = 0; i < borderParent.childCount; i++)
            borderParent.GetChild(i).gameObject.SetActive(false);
    }

    private void ClearSpawnedPieces()
    {
        spawnedPieces.Clear();
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}