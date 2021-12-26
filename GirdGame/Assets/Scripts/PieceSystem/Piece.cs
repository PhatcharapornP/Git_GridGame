using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class Piece : MonoBehaviour, IPiece, IPoolObject
{
    [SerializeField] private Button button;
    public Vector2Int Position { get; private set; }
    public Color PieceColor { get; private set; }


    public bool IsSelected { get; private set; }

    [SerializeField] private Vector2Int debugPos;
    [SerializeField] private Color debugPiecColor;
    [SerializeField] private bool debugIsSelected;

    public void InitializePoolObj()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { OnClickPiece(); });
    }

    public void SetupPieceData(Vector2Int pos, Vector3 targetPos,bool autoMove = true)
    {
        IsSelected = false;
        debugIsSelected = IsSelected;
        Position = pos;
        debugPos = Position;
        PieceColor = GameManager.Instance.ColorPool[Random.Range(0, GameManager.Instance.ColorPool.Count)];
        button.image.color = PieceColor;
        debugPiecColor = PieceColor;
        MoveToTargetPos(targetPos);
        name = $"P_{Position.x},{Position.y}";
    }

    public void OverwritePieceData(Piece data)
    {
        IsSelected = data.IsSelected;
        debugIsSelected = IsSelected;
        var debugName = $"P_{Position.x},{Position.y}";
        Position = data.Position;
        debugPos = Position;
        debugName = $"{debugName}_OW_from_{Position.x},{Position.y}";
        Debug.Log($"{name} is being overwrite to {debugName} ".InColor(PieceColor),gameObject);
        PieceColor = data.PieceColor;
        debugPiecColor = PieceColor;
        button.image.color = PieceColor;
        name = debugName;
        Debug.Log($"Now {name} is Selected: {IsSelected}".InColor(PieceColor),gameObject);
        gameObject.SetActive(data.gameObject.activeInHierarchy);
        
    }

    public void OverwritePos(Vector2Int newPos)
    {
        Position = newPos;
        debugPos = Position;
        NameGameObj();
    }

    public void NameGameObj()
    {
        name = $"P_{Position.x},{Position.y}";
    }

    public void ForceDebugSelected(bool debug)
    {
        Debug.Log($"{name} Got forced selected to {debug}".InColor(Color.red),gameObject);
        IsSelected = debug;
        debugIsSelected = IsSelected;
        PieceColor = Color.grey;
        debugPiecColor = PieceColor;
        button.image.color = PieceColor;
    }

    public void MoveToTargetPos(Vector3 targetPos)
    {
        DOTween.Kill(button.image);
        DOTween.To(() => transform.localPosition, x => transform.localPosition = x, targetPos, .5f);
    }

    public void OnClickPiece()
    {
        if (GameManager.Instance.Board.CheckMatchesFromPiece(this))
        {
            // Debug.Log($"OnClickPiece: {name} and found matches: {GameManager.Instance.Board.CheckMatchesFromPiece(this)}".InColor(PieceColor),gameObject);
            Debug.Log($"OnClickPiece: {name} and found matches".InColor(PieceColor));
            // GameManager.Instance.Score.SetPlayerScore(GameManager.Instance.Board.CheckMatchesFromPiece(this));
            GameManager.Instance.Board.FillEmptyPositions();
        }
    }

    public void OnSelected()
    {
        //TODO: Do something snake!?

        IsSelected = true;
        DOTweenModuleUI.DOColor(button.image, Color.grey, 0);
        name = $"Selected_{Position.x},{Position.y}";
        gameObject.SetActive(false);
    }

    private void DisablePiece(TweenCallback callback)
    {
        gameObject.SetActive(false);
    }
}