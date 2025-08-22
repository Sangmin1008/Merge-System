using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemSO : ScriptableObject
{
    public int ID;
    public string Name;
    [TextArea(3, 5)] public string Description;
    public Sprite Icon;
    public ItemType Type;
    public int Level;
}
