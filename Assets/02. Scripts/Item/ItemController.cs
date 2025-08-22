using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 아이템 컨트롤러는 아이템 드래그 / 이동 만 처리해주고 merge 등 로직은 외부에서 하는게 나을 듯

public class ItemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private Vector2EventChannelSO beginDragEvent;
    [SerializeField] private VoidEventChannelSO endDragEvent;
    [SerializeField] private VoidEventChannelSO onHideHighlights;
    private PopupUIClickScaleTweenHandler _popupUIClickScaleTweenHandler;
    private Vector2 _originPosition;
    private Vector2 _gridPosition;
    private Image _image;
    public Item Item { get; private set; }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _popupUIClickScaleTweenHandler = GetComponent<PopupUIClickScaleTweenHandler>();
    }

    public void Initialize(int itemID)
    {
        Item = new Item(itemID);
        _image.sprite = Item.ItemData.Icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginDragEvent.Raise(_gridPosition);
        _originPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endDragEvent.Raise();
        transform.position = _originPosition;
    }

    public void SetGridPosition(Vector2 gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public Vector2 GetGridPosition()
    {
        return _gridPosition;
    }

    public void ShowPopUpAnimation()
    {
        _popupUIClickScaleTweenHandler.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var gridController = GameManager.Instance.gridController;
        var block = gridController.GetBlockByPosition((int)_gridPosition.x, (int)_gridPosition.y);
        
        onHideHighlights.Raise();
        block.ShowHighlights();
    }
}
