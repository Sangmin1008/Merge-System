using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTable", menuName = "ScriptableObjects/Tables/ItemTable")]
public class ItemTable : BaseTable<int, ItemSO>
{
    protected override string[] DataPath => new[] { "Assets/10. Tables/ScriptableObject/Item" };

    public override void CreateTable()
    {
        Type = GetType();
        foreach (var item in dataList)
        {
            DataDic[item.ID] = item;
        }
    }
}
