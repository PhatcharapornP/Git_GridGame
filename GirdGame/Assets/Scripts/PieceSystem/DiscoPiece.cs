using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoPiece : Piece
{
    public int customColorIndex;

    protected override void OnSetupPieceData(Vector2Int pos, Vector3 targetPos)
    {
        base.OnSetupPieceData(pos, targetPos);
        SetPieceColor(customColorIndex);
    }

    protected override void OnClickedPiece()
    {
        IsSelected = GameManager.Instance.Board.CheckColorMatch(ColorIndex);
    }
}
