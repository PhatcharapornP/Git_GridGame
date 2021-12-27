using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
[Serializable]
public class Piece : MonoBehaviour, IPiece, IPoolObject
{
    [SerializeField] private Button button;
    public Vector2Int Position { get; private set; }
    public Color PieceColor { get; private set; }
    public int ColorIndex { get; protected set; }

    public bool IsSelected { get; protected set; }

    public void InitializePoolObj()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { OnClickPiece(); });
    }

    public void SetupPieceData(Vector2Int pos, Vector3 targetPos) {OnSetupPieceData(pos, targetPos);}

    public void OnClickPiece() {OnClickedPiece();}

    public void OnSelected() {OnPieceSelected();}

    protected virtual void OnSetupPieceData(Vector2Int pos, Vector3 targetPos)
    {
        IsSelected = false;
        OverwritePos(pos);
        SetPieceColor(Random.Range(0, GameManager.Instance.ColorPool.Count));
        MoveToTargetPos(targetPos);
    }

    protected void SetPieceColor(int index)
    {
        ColorIndex = index;
        PieceColor = GameManager.Instance.ColorPool[ColorIndex];
        button.image.color = PieceColor;
    }

    public void OverwritePos(Vector2Int newPos)
    {
        Position = newPos;
        name = $"P_{Position.x},{Position.y}";
    }
    
    public void MoveToTargetPos(Vector3 targetPos)
    {
        DOTween.Kill(button.image);
        DOTween.To(() => transform.localPosition, x => transform.localPosition = x, targetPos, .5f);
    }

    protected virtual void OnClickedPiece()
    {
        GameManager.Instance.Board.CheckMatchesFromPiece(this);
    }

    protected virtual void OnPieceSelected()
    {
        IsSelected = true;
        gameObject.SetActive(false);
    }
}