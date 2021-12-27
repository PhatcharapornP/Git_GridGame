using UnityEngine;

public class BaseSpecialPiece : Piece
{
    public int customColorIndex;
    protected override void OnSetupPieceData(Vector2Int pos, Vector3 targetPos)
    {
        base.OnSetupPieceData(pos, targetPos);
        SetPieceColor(customColorIndex);
    }
}
