using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Space))
            GenerateRandomizeBoard();
#endif
    }

    private void CheckReference()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void GenerateRandomizeBoard()
    {
        columns = Random.Range(5,16);
        rows = Random.Range(5,16);
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        CheckReference();
        pieces = new Piece[columns, rows];
        GameManager.Instance.ColorPool.Clear();
        SetupColorPoolFromBoardSize();
        ClearBoard();
        CalculatePieceSize();
        PopulateBoard();
    }

    private void SetupColorPoolFromBoardSize()
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
        var cellWidth = _rectTransform.rect.width / columns - ((spacing / columns) * 2);
        var cellHeight = _rectTransform.rect.height / rows - ((spacing / rows) * 2);

        if (cellWidth < cellHeight)
            pieceSize = Mathf.FloorToInt(cellWidth);
        else
            pieceSize = Mathf.FloorToInt(cellHeight);

        var totalColumnAvailable = Mathf.FloorToInt(_rectTransform.rect.width / pieceSize);
        var totalRowsAvailable = Mathf.FloorToInt(_rectTransform.rect.height / pieceSize);

        heightDiff = 0;
        heightDiff = totalRowsAvailable - rows;

        widthDiff = 0;
        widthDiff = totalColumnAvailable - columns;
    }

    private void PopulateBoard()
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
            SetupPieceTransform(piece, posX, posY, column, row);
            SpawnBorder(posX, posY);
        }
    }

    private void SetupPieceTransform(Piece piece, int posX, int posY, int column, int row)
    {
        piece.transform.localPosition = new Vector3(posX, posY + _rectTransform.rect.height, 0) - positionOffset;
        piece.transform.localScale = new Vector3(pieceSize - spacing, pieceSize - spacing, 1);
        piece.gameObject.SetActive(true);
        piece.SetupPieceData(new Vector2Int(column, row), new Vector3(posX, posY, 0) - positionOffset);
        pieces[column, row] = piece;
    }
    
    private void SpawnDiscoPiece(DiscoPiece piece, int posX, int posY, int column, int row,int colorIndex)
    {
        piece.transform.localPosition = new Vector3(posX, posY + _rectTransform.rect.height, 0) - positionOffset;
        piece.transform.localScale = new Vector3(pieceSize - spacing, pieceSize - spacing, 1);
        piece.customColorIndex = colorIndex;
        piece.SetupPieceData(new Vector2Int(column, row), new Vector3(posX, posY, 0) - positionOffset);
        pieces[column, row] = piece;
        piece.gameObject.SetActive(true);
    }

    private void SpawnBorder(int posX, int posY)
    {
        GameObject border = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.border);
        border.transform.SetParent(borderParent);
        border.transform.localPosition = new Vector3(posX, posY, 0) - positionOffset;
        border.transform.localScale = new Vector3(pieceSize - spacing, pieceSize, 1);
        border.gameObject.SetActive(true);
        spawnedBorder.Add(border);
    }

    private bool discoUnlocked = false;
    private int discoColorIndex = -1;
    private Vector2Int discoPos;

    public void CheckMatchesFromPiece(Piece targetPiece)
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

            if (matchedPieces.Count > 6 && matchedPieces.Count < 10)
            {
                //TODO: Bomb piece
            }
            
            if (matchedPieces.Count > 10)
            {
                discoUnlocked = true;
                discoColorIndex = targetPiece.ColorIndex;
                discoPos = targetPiece.Position;
                
                Debug.Log($"Unlock disco piece!: {targetPiece.Position}".InColor(targetPiece.PieceColor),targetPiece.gameObject);
            }
            
            GameManager.Instance.Score.SetPlayerScore(matchedPieces.Count);
            GameManager.Instance.Board.FillEmptyPositions();
        }
    }

    public bool CheckColorMatch(int colorIndex)
    {
        var score = 0;
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                if (pieces[c, r].ColorIndex == colorIndex)
                {
                    score += 1;
                    pieces[c,r].OnSelected();
                }
            }
        }

        if (score > 1)
        {
            GameManager.Instance.Score.SetPlayerScore(matchedPieces.Count);
            GameManager.Instance.Board.FillEmptyPositions();
        }

        return score > 1;
    }

    public bool CheckMatchPossibility()
    {
        HashSet<Piece> matchedPieces = new HashSet<Piece>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var horizontalMatches = FindColumnMatchFromPiece(pieces[column,row]);
                var verticalMatches = FindRowMatchFromPiece(pieces[column,row]);
                matchedPieces.UnionWith(horizontalMatches);
                matchedPieces.UnionWith(verticalMatches);
            }
        }
        return matchedPieces.Count > 0;
    }

    public void FillEmptyPositions()
    {
        for (int column = 0; column < columns; column++)
            for (int row = 0; row < rows; row++)
        {
            while (pieces[column, row].IsSelected)
            {
                Piece current = pieces[column, row];
                Piece next = current;
                Vector2Int tempPos = next.Position;
                
                for (int filler = row; filler < rows - 1; filler++)
                {
                    next = pieces[column, filler + 1];
                    current = next;
                    tempPos = next.Position;
                    var posX = (pieceSize * column);
                    var posY = (pieceSize * filler);
                    
                    //Move down upper piece to fill downward
                    current.MoveToTargetPos(new Vector3(posX, posY, 0) - positionOffset);
                    current.OverwritePos(new Vector2Int(column,filler));
                   
                    //Update piece array
                    pieces[column, filler] = current;
                    
                }
                
                //Fill up empty position on board
                var newPiece = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.piece).GetComponent<Piece>();
                var tempX = (pieceSize * tempPos.x);
                var tempY = (pieceSize * tempPos.y);
                SetupPieceTransform(newPiece, tempX, tempY, tempPos.x,tempPos.y);
                
            }

            if (discoUnlocked)
            {
                if (pieces[column, row].Position == discoPos)
                {
                    var temp = pieces[column, row];
                    temp.OnSelected();
                    var newPiece = GameManager.Instance.Pool.PickFromPool(Globals.PoolTag.disco).GetComponent<DiscoPiece>();
                    var tempX = (pieceSize * discoPos.x);
                    var tempY = (pieceSize * discoPos.y);
                    newPiece.customColorIndex = discoColorIndex;
                    SetupPieceTransform(newPiece, tempX, tempY, discoPos.x,discoPos.y);
                    pieces[column, row] = newPiece;
                    discoUnlocked = false;
                }
            }
        }
        
        if (CheckMatchPossibility()== false)
        {
            GameManager.Instance.State.GetStateViaType(typeof(GameState)).EndState();
            GameManager.Instance.State.GetStateViaType(typeof(GameOverState)).StartState();
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
            foreach (var piece in horizontalMatches)
                if (tempMatches.Contains(piece) == false)
                    tempMatches.Add(piece);

        if (verticalMatches.Count >= 1)
            foreach (var piece in verticalMatches)
                if (tempMatches.Contains(piece) == false)
                    tempMatches.Add(piece);
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