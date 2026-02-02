using UnityEngine;

// [CreateAssetMenu] �������� ��� ���������� ��� �������� ����� � Unity
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public bool isStackable = true;
    public GameObject itemPrefab;
}

// ������ ���� ��������
public enum ItemType
{
    MeleeWeapon,
    RangedWeapon,
    Utility,
    Shield,
    Talisman,
    Consumable,
    Resource
}