using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPiece
{
    Vector2Int Position { get; }
    Color PieceColor { get; }
    void SetupPieceData(Vector2Int pos);
    void OnClickPiece();
    void OnSelected();
}
