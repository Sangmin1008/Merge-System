using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO onBeginDragEvent;
    [SerializeField] private VoidEventChannelSO onEndDragEvent;
    [SerializeField] private Vector2EventChannelSO onDropEvent;
    [SerializeField] private VoidEventChannelSO onHideHighlights;
    [SerializeField] private CanvasGroup itemCanvasGroup;
    
    private bool _isDragging;
    private GridController _gridController;
    
    public Vector2 DragStartPosition { get; private set; }
    public Vector2 DropPosition { get; private set; }
    
    
    private void OnEnable()
    {
        onBeginDragEvent.RegisterListener(DetectBeingDrag);
        onEndDragEvent.RegisterListener(DetectEndDrag);
        onDropEvent.RegisterListener(DetectDrop);
    }

    private void OnDisable()
    {
        onBeginDragEvent.UnregisterListener(DetectBeingDrag);
        onEndDragEvent.UnregisterListener(DetectEndDrag);
        onDropEvent.UnregisterListener(DetectDrop);
    }
    
    private void Start()
    {
        _gridController = GameManager.Instance.gridController;
    }

    private void DetectBeingDrag(Vector2 position)
    {
        onHideHighlights.Raise();
        DragStartPosition = position;
        _isDragging = true;
        itemCanvasGroup.blocksRaycasts = false;
    }

    private void DetectEndDrag()
    {
        itemCanvasGroup.blocksRaycasts = true;
    }

    private void DetectDrop(Vector2 position)
    {
        if (!_isDragging)
            return;
        
        
        DropPosition = position;
        onHideHighlights.Raise();

        var block = GameManager.Instance.gridController.GetBlockByPosition((int)position.x, (int)position.y);
        block.ShowHighlights();

        TryMerge();
        
        _isDragging = false;
        itemCanvasGroup.blocksRaycasts = true;
    }

    private void TryMerge()
    {
        if (DragStartPosition == DropPosition)
        {
            Debug.Log("같은 위치라 merge 안 됨");
            return;
        }
        
        Block startBlock = _gridController.GetBlockByPosition((int)DragStartPosition.x, (int)DragStartPosition.y);
        Block dropBlock = _gridController.GetBlockByPosition((int)DropPosition.x, (int)DropPosition.y);

        if (startBlock == null || dropBlock == null || startBlock.IsBlockEmpty)
        {
            Debug.LogError("블록 검출이 안 됨");
            return;
        }
        
        if (dropBlock.IsBlockEmpty)
        {
            MoveItem(startBlock, dropBlock);
        }
        else if (startBlock.Item.ID != dropBlock.Item.ID)
        {
            SwapItems(startBlock, dropBlock);
        }
        else
        {
            if (startBlock.Item.ID >= TableManager.Instance.GetTable<ItemTable>().Count - 1)
            {
                Debug.Log("마지막 레벨 아이템임");
                return;
            }
            MergeItems(startBlock, dropBlock, startBlock.Item.ID);
        }
    }
    
    private void MoveItem(Block startBlock, Block dropBlock)
    {
        int itemID = startBlock.Item.ID;
        startBlock.EraseItem();
        _gridController.SetBlockItem((int)dropBlock.GridPosition.x, (int)dropBlock.GridPosition.y, itemID);
    }
    
    private void SwapItems(Block startBlock, Block dropBlock)
    {
        int idA = startBlock.Item.ID;
        Vector2 positionA = startBlock.GridPosition;
        
        int idB = dropBlock.Item.ID;
        Vector2 positionB = dropBlock.GridPosition;
        
        // startBlock.EraseItem();
        // dropBlock.EraseItem();
        //
        // _gridController.SetBlockItem((int)positionA.x, (int)positionA.y, idB);
        // _gridController.SetBlockItem((int)positionB.x, (int)positionB.y, idA);
        
        StartCoroutine(SmoothMovementCoroutine(startBlock, dropBlock, idA, idB, positionA, positionB));
    }

    private IEnumerator SmoothMovementCoroutine(Block blockA, Block blockB, int idA, int idB, Vector2 posA, Vector2 posB)
    {
        float duration = 0.15f;
        float time = 0f;
        Vector3 startPosition = blockB.CenterPosition;
        Vector3 targetPosition = blockA.CenterPosition;

        blockA.EraseItem();
        blockB.EraseItem();
        _gridController.SetBlockItem((int)posB.x, (int)posB.y, idA);
        _gridController.SetBlockItem((int)posA.x, (int)posA.y, idB);
        
        GameObject itemB = blockA.ItemPrefab;
        itemB.transform.position = startPosition;

        while (time < duration)
        {
            float t = time / duration;
            itemB.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            time += Time.deltaTime;
            yield return null;
        }
        
        itemB.transform.position = targetPosition;
    }

    private void MergeItems(Block startBlock, Block dropBlock, int id)
    {
        startBlock.EraseItem();
        dropBlock.EraseItem();
        
        _gridController.SetBlockItem((int)DropPosition.x, (int)DropPosition.y, id + 1);
        GameManager.Instance.RenewScore(id + 1);
    }
}
