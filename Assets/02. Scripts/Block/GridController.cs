using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 이거 Grid에 달아줘야 grid layout에 블록 먹일 수 있음
// 블록 크기는 150 x 150, space는 30, 그러면 grid가 N x M일 때 전체 크기는
// height = N x (150 + 30) + 30, width = M x (150 + 30) + 30 으로 하면 될 듯
public class GridController : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int row;
    [SerializeField] private int column;
    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;
    private Block[,] _grid;
    private const int CELL_SIZE = 150;
    private const int SPACE_SIZE = 30;

    public int Row => row;
    public int Column => column;
    public Block GetBlockByPosition(int x, int y) => _grid[x, y];

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        
        InitializeGrid();
        GenerateGrid();
        ItemManager.Instance.poolSize = row * column + 1; // 혹시 모르니깐 일단 +1 해야 할 것 같음
    }

    private void InitializeGrid()
    {
        _gridLayoutGroup.cellSize = new Vector2(CELL_SIZE, CELL_SIZE);
        _gridLayoutGroup.spacing = new Vector2(SPACE_SIZE, SPACE_SIZE);
        _gridLayoutGroup.padding.left = SPACE_SIZE;
        _gridLayoutGroup.padding.right = SPACE_SIZE;
        _gridLayoutGroup.padding.top = SPACE_SIZE;
        _gridLayoutGroup.padding.bottom = SPACE_SIZE;
    }

    private void GenerateGrid()
    {
        _grid = new Block[row, column];
        _rectTransform.sizeDelta = new Vector2(column * (CELL_SIZE + SPACE_SIZE) + SPACE_SIZE, row * (CELL_SIZE + SPACE_SIZE) + SPACE_SIZE);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var blockObject = Instantiate(blockPrefab, transform);
                if (blockObject.TryGetComponent(out Block blockComponent))
                {
                    _grid[i, j] = blockComponent;
                    blockComponent.GridPosition = new Vector2(i, j);
                }
            }
        }
    }

    public void SetBlockItem(int r, int c, int id, bool doAnimation = false)
    {
        Debug.Log($"Row: {r}, Column: {c}, Id: {id}");
        Block targetBlock = _grid[r, c];
    
        targetBlock.SetItem(id);
        targetBlock.GridPosition = new Vector2(r, c);

        if (targetBlock.ItemPrefab != null && targetBlock.ItemPrefab.TryGetComponent(out ItemController itemController))
        {
            itemController.SetGridPosition(new Vector2(r, c));
            if (doAnimation)
                itemController.ShowPopUpAnimation();
        }
    }
}
