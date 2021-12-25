using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class Piece : MonoBehaviour,IPiece,IPoolObject
{
    [SerializeField] private Button button;
    public Vector2Int Position { get;  private set; }
    public Color PieceColor { get; private set; }
    
    public void InitializePoolObj()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { OnClickPiece(); });
    }

    public void SetupPieceData(Vector2Int pos,Vector3 targetPos)
    {
        Position = pos;
        name = $"P_{Position.x},{Position.y}";
        PieceColor = GameManager.Instance.ColorPool[Random.Range(0, GameManager.Instance.ColorPool.Count)];
        button.image.color = PieceColor;
        DOTween.Kill(button.image);
        DOTween.To(() => transform.localPosition, x => transform.localPosition = x, targetPos, 1);
    }

    public void OnClickPiece()
    {
        if (GameManager.Instance.Board.CheckMatchesFromPiece(this))
        {
            // Debug.Log($"OnClickPiece: {name} and found matches: {GameManager.Instance.Board.CheckMatchesFromPiece(this)}".InColor(PieceColor),gameObject);
            Debug.Log($"OnClickPiece: {name} and found matches".InColor(PieceColor));
            // GameManager.Instance.Score.SetPlayerScore(GameManager.Instance.Board.CheckMatchesFromPiece(this));
        }
    }

    public void OnSelected()
    {
        //TODO: Do something snake!?

            DOTweenModuleUI.DOColor(button.image, Color.grey, .5f);
    }

   
}