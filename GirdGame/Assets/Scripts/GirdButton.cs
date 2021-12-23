using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class GirdButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Vector2Int postion;

    public void Initialize(Vector2Int pos)
    {
        button = GetComponent<Button>();
        if (button == null)
            button = gameObject.AddComponent<Button>();

        postion = pos;
        name = $"GB_{postion.x},{postion.y}";
        button.image.color = GameManager.Instance.ColorPool[Random.Range(0, GameManager.Instance.ColorPool.Count)];
        button.onClick.AddListener(() => { OnGridButtonClick(); });
        
    }

    void OnGridButtonClick()
    {
        Debug.Log($"{name}",gameObject);
    }
}