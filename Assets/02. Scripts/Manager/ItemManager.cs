using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GridController에서 row와 col값 만큼 각 Item을 미리 생성해둬야 함
// poolSize < row * col이 되어버리면 풀이 텅텅 비어버림
public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] public int poolSize;
    [SerializeField] private Transform parent;
    
    private Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();
    private Queue<GameObject>[] _pool;
    private ItemTable _itemTable;
    private int _itemCount;
    
    private void Start()
    {
        _itemTable = TableManager.Instance.GetTable<ItemTable>();
        _itemCount = _itemTable.Count;
        _pool = new Queue<GameObject>[_itemCount];
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < _itemCount; i++)
        {
            _pool[i] = new Queue<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                var item = Instantiate(itemPrefab, parent);
                if (item.TryGetComponent(out ItemController controller))
                {
                    controller.Initialize(i);
                }
                _pool[i].Enqueue(item);
                item.SetActive(false);
            }
        }
    }

    public GameObject GetItem(int id)
    {
        GameObject item = _pool[id].Dequeue();
        item.SetActive(true);
        return item;
    }

    public void ReturnItem(GameObject item)
    {
        if (item.TryGetComponent(out ItemController itemController))
        {
            itemController.transform.localScale = Vector3.one;
            _pool[itemController.Item.ID].Enqueue(item);
            item.SetActive(false);
        }
    }

    public Sprite GetItemImage(int id)
    {
        var item = _itemTable.GetDataByID(id);
        return item.Icon;
    }
}
