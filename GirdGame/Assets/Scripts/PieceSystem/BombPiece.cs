public class BombPiece : BaseSpecialPiece
{
    protected override void OnClickedPiece()
    {
        IsSelected = GameManager.Instance.Board.DestroySameDimension(Position);
    }
}
