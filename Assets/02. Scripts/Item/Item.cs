public enum ItemType
{
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
    Ruby,
    Master
}

// Item은 굳이 생명주고 끌고 올 필요 없어보임
// 그냥 SO만 들고 있으면 될 것 같음
// 아이템끼리 상호작용 로직 넣기 위한 껍데기 정도로 생각 중

public class Item
{
    public int ID { get; private set; }
    public ItemSO ItemData { get; private set; }

    // 이거 생성자 Awake에서 호출하면 호출 순서 꼬이니깐 주의
    public Item(int id)
    {
        ID = id;
        Initialize();
    }

    private void Initialize()
    {
        ItemTable itemTable = TableManager.Instance.GetTable<ItemTable>();
        ItemData = itemTable.GetDataByID(ID);
    }

    public bool CanMerge(Item other)
    {
        return ID == other.ID;
    }
}


