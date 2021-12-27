using UnityEngine;

public interface IPiece
{
    Vector2Int Position { get; }
    Color PieceColor { get; }
    int ColorIndex { get; }
    void SetupPieceData(Vector2Int pos, Vector3 targetPos);
    void OnClickPiece();
    void OnSelected();
}
