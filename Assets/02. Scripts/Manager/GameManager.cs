using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public GridController gridController;
    [SerializeField] private Image bestScoreImage;
    private int _bestScore = -1;
    private PopupUIClickScaleTweenHandler _popupUIClickScaleTweenHandler;

    protected override void Awake()
    {
        base.Awake();
        _popupUIClickScaleTweenHandler = bestScoreImage.gameObject.GetComponent<PopupUIClickScaleTweenHandler>();
    }
    private void Start()
    {
        bestScoreImage.gameObject.SetActive(false);
    }

    private Vector2 GetPosition()
    {
        int row = gridController.Row;
        int column = gridController.Column;
        
        // 그리드가 워낙 작게 세팅이 될거라 O(N^2)에 돌려도 충분 할 듯
        // 만약 엄청 크게 만들어진다면 gridController에서 empty인 block과 그렇지 않은 블럭을 List에 때려넣고
        // LIFO처럼 empty인 블록 포지션을 리스트 앞에서부터 순서대로 꺼내오는 식으로 하면 랜덤(?) 포지션을 O(1)에 가져올 수 있긴 할거임
        // 근데 굳이??

        // for (int i = 0; i < row; i++)
        // {
        //     for (int j = 0; j < column; j++)
        //     {
        //         if (!gridController.GetBlockByPosition(i, j).IsBlockEmpty) continue;
        //         return new Vector2(i, j);
        //     }
        // }
        
        
        // 이렇게 하면 랜덤하게 할 수 있긴 할 듯
        var empties = new List<Vector2Int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (gridController.GetBlockByPosition(i, j).IsBlockEmpty)
                    empties.Add(new Vector2Int(i, j));
            }
        }
        

        if (empties.Count == 0) return new Vector2Int(-1, -1);
        return empties[Random.Range(0, empties.Count)];
        
        // return new  Vector2(-1, -1);
    }
    
    private int GenerateID(int maxID, int n = 4)
    {
        if (maxID <= 0) return 0;
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            sum += Random.value;
        }
        float average = sum / n;
        int id = Mathf.RoundToInt(average * maxID);
        return Mathf.Clamp(id, 0, maxID);
    }
    
    public void GenerateItem()
    {
        Vector2 randomPosition = GetPosition();
        Debug.Log(randomPosition);
        if ((int)randomPosition.x == -1 && (int)randomPosition.y == -1)
        {
            Debug.LogWarning("그리드 꽉 참!!");
            return;
        }
        int randomID = GenerateID(_bestScore);
    
        gridController.SetBlockItem((int)randomPosition.x, (int)randomPosition.y, randomID, true);

        RenewScore(randomID);
    }

    public void RenewScore(int newScore)
    {
        if (newScore > _bestScore)
        {
            if (!bestScoreImage.gameObject.activeSelf)
                bestScoreImage.gameObject.SetActive(true);
            _bestScore = newScore;
            bestScoreImage.sprite = ItemManager.Instance.GetItemImage(_bestScore);
            _popupUIClickScaleTweenHandler.Play();
        }
    }
}
