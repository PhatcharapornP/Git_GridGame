using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//https://youtu.be/CGsEJToeXmA

[ExecuteInEditMode]
public class BoardManager : MonoBehaviour
{
    [Tooltip("Row amount minimum is floored at 5")] 
    [SerializeField] [Range(5,16)]private int rows = 5;

    [Tooltip("Collum amount minimum is floored at 5")]
    [SerializeField] [Range(5,16)]private int columns = 5;

    [SerializeField] [Min(35)] private int gridSize = 10;
    [SerializeField] [Min(1.1f)] private float spacing = 1.1f;
    [SerializeField] private int totalColumnAvailable;
    [SerializeField] private int totalRowsAvailable;
    
    [SerializeField] private float widthDiff;
    [SerializeField] private float heightDiff;
    
    
    [SerializeField] private GirdButton girdButtonPrefab;
    [SerializeField] private List<GirdButton> spawnedGridButton = new List<GirdButton>();
    [SerializeField] private RectTransform _rectTransform;
    private Vector3 positionOffset;
    private GirdButton[,] gridButtons;
    private Vector3 center;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
        {
            GenerateBoard();
        }
    }

    private void CheckReference()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            columns = Random.Range(5, 17);
            rows = Random.Range(5, 17);
            GenerateBoard();
        }
    }

    private void GenerateBoard()
    {
        CheckReference();
        gridButtons = new GirdButton[columns, rows];
        GameManager.Instance.ColorPool.Clear();
        if (columns * rows == 25)
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
            {
                GameManager.Instance.ColorPool.Add(color);  
            }
            
        }
        else if (columns * rows >= 25 && columns * rows <= 100)
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
                GameManager.Instance.ColorPool.Add(color);
            
            foreach (var color in GameManager.Instance.gameTweak.levelTwo)
                GameManager.Instance.ColorPool.Add(color);  
        }
        else
        {
            foreach (var color in GameManager.Instance.gameTweak.levelOne)
                GameManager.Instance.ColorPool.Add(color);
            
            foreach (var color in GameManager.Instance.gameTweak.levelTwo)
                GameManager.Instance.ColorPool.Add(color);
            
            foreach (var color in GameManager.Instance.gameTweak.levelThree)
                GameManager.Instance.ColorPool.Add(color);  
        }
        
        DestroyAllSpawnObj();
        CalculateCellSize();
        SpawnGridButton();
    }

    private void OnEnable()
    {
        CheckReference();
        CalculateCellSize();
    }


    private void CalculateCellSize()
    {
        float parentWidth = _rectTransform.rect.width;
        float parentHeight = _rectTransform.rect.height;
        
        Debug.Log($"parentWidth: {parentWidth}");
        Debug.Log($"parentHeight: {parentHeight}");
        
       

        float cellWidth = parentWidth / columns - ((spacing / columns) * 2);
        float cellHeight = parentHeight / rows - ((spacing / rows) * 2);
        
        Debug.Log($"cellWidth: {cellWidth}");
        Debug.Log($"cellHeight: {cellHeight}");


        var calculatedWidthAmount = cellWidth * columns;
        var calculatedHeightAmount = cellHeight * rows;
        Debug.Log($"calculatedWidthAmount: {calculatedWidthAmount}");
        Debug.Log($"calculatedHeightAmount: {calculatedHeightAmount}");

        if (cellWidth < cellHeight)
            gridSize = Mathf.FloorToInt(cellWidth );
        else 
            gridSize = Mathf.FloorToInt(cellHeight) ;
        
        totalColumnAvailable = Mathf.FloorToInt(parentWidth / gridSize);
        totalRowsAvailable = Mathf.FloorToInt(parentHeight / gridSize);
        
        Debug.Log($"totalColumnAvailable: {totalColumnAvailable}");
        Debug.Log($"totalRowsAvailable: {totalRowsAvailable}");
        
        heightDiff = 0;
        heightDiff = totalRowsAvailable - rows;

        widthDiff = 0;
        widthDiff = totalColumnAvailable - columns;
    }
    
    private void SpawnGridButton()
    {
        // gridPosition = transform.position - new Vector3(columns * spacing / 2.0f, rows * spacing / 2.0f, 0);
        positionOffset = Vector3.zero;
        center = Vector3.zero;

        if (heightDiff > 0)
        {
            // center.x = columns * spacing / 2.0f;
            center.x = 0;
            // center.y = rows * gridSize / 2.0f;
            center.y = heightDiff * gridSize / 2.0f;
            Debug.Log($"center y: {center.y}");
            center.z = 0;
            center.y = center.y + ((gridSize - spacing) * heightDiff) / 2.0f;
            Debug.Log($"modded center y: {center.y}");
        }

        if (widthDiff > 0)
        {
            center.y = 0;
            center.x = widthDiff * gridSize / 2.0f;
            Debug.Log($"center x: {center.x}");
            center.z = 0;
            // center.x = center.x + ((gridSize - spacing) * widthDiff) / 2.0f;
            // Debug.Log($"modded center x: {center.x}");
        }

        positionOffset = _rectTransform.position - center;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GirdButton grid = Instantiate(girdButtonPrefab);
                grid.Initialize(new Vector2Int(column, row));
                spawnedGridButton.Add(grid);

                grid.transform.parent = transform;

                var posX = (gridSize * column) ;
                var posY = (gridSize * row) ;
                

                
                // grid.transform.localPosition = new Vector3(posX , posY, 0) - (new Vector3( _rectTransform.rect.center.x, _rectTransform.rect.center.y,0));
                // if (columns > rows || rows > columns)
                    grid.transform.localPosition = new Vector3(posX , posY, 0) - positionOffset;    
                // else
                    // grid.transform.localPosition = new Vector3(posX, posY, 0);    

                grid.transform.localScale = new Vector3(gridSize - spacing,gridSize - spacing ,1);
                gridButtons[column, row] = grid;
            }
        }
    }

    void DestroyAllSpawnObj()
    {
        spawnedGridButton.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}