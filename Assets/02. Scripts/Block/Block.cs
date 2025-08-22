using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IDropHandler
{
    [SerializeField] private Vector2EventChannelSO onDropEvent;
    [SerializeField] private VoidEventChannelSO onHideHighlights;
    [SerializeField] private GameObject highlight;
    public GameObject ItemPrefab { get; private set; } = null;
    public Vector2 CenterPosition => transform.position;
    public Vector2 GridPosition { get; set; }
    public Item Item { get; private set; } = null;
    public bool IsBlockEmpty { get; private set; } = true;
    
    
    private PopupUIClickScaleTweenHandler _popupUIClickScaleTweenHandler;

    private void Awake()
    {
        _popupUIClickScaleTweenHandler = highlight.GetComponent<PopupUIClickScaleTweenHandler>();
    }

    private void OnEnable()
    {
        onHideHighlights.RegisterListener(HideHighlights);
    }

    private void OnDisable()
    {
        onHideHighlights.UnregisterListener(HideHighlights);
    }

    public void OnDrop(PointerEventData eventData)
    {
        onDropEvent.Raise(GridPosition);
    }

    public void SetItem(int itemID)
    {
        if (itemID < 0 || itemID >= TableManager.Instance.GetTable<ItemTable>().Count)
            return;
        
        Debug.Log($"{itemID}가 세팅됨");
        
        ItemPrefab = ItemManager.Instance.GetItem(itemID);
        if (ItemPrefab.TryGetComponent(out ItemController itemController))
        {
            Item = itemController.Item;
        }
        ItemPrefab.transform.position = CenterPosition;
        IsBlockEmpty = false;
    }

    public void EraseItem()
    {
        ItemManager.Instance.ReturnItem(ItemPrefab);
        ClearBlock();
    }
    
    private void ClearBlock()
    {
        ItemPrefab = null;
        Item = null;
        IsBlockEmpty = true;
    }

    public void ShowHighlights()
    {
        highlight.SetActive(true);
        _popupUIClickScaleTweenHandler.Play();
    }
    
    public void HideHighlights()
    {
        highlight.SetActive(false);
    }
}
