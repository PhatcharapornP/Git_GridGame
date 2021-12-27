using UnityEngine;

public class DiscoPiece : BaseSpecialPiece
{
    protected override void OnClickedPiece()
    {
        IsSelected = GameManager.Instance.Board.CheckColorMatch(ColorIndex);
    }
}
