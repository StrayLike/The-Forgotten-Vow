using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // ������ ����� ��� ��������� �� �������
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    public int inventorySize = 20;
    public int hotbarSize = 5;

    // ����, ��� ���� �������� UI ��� ���� � ��������
    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // ����������� ����� ��������� ���������
            for (int i = 0; i < inventorySize; i++)
            {
                inventorySlots.Add(new InventorySlot(null, 0));
            }

            // ����������� ����� �������
            for (int i = 0; i < hotbarSize; i++)
            {
                hotbarSlots.Add(new InventorySlot(null, 0));
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ����� ��� ��������� �������� � ��������
    public bool AddItem(Item itemToAdd)
    {
        // 1. �������� ����������, �� ����� ��������� �������
        if (itemToAdd.isStackable)
        {
            // ������ ���� � �������� ��� ����������
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.item == itemToAdd)
                {
                    slot.AddToStack(1);
                    Debug.Log($"������� {itemToAdd.itemName} ������ � ���� � ��������.");
                    NotifyUI();
                    return true;
                }
            }

            // ������ ���� � ������ ��� ����������
            foreach (InventorySlot slot in hotbarSlots)
            {
                if (slot.item == itemToAdd)
                {
                    slot.AddToStack(1);
                    Debug.Log($"������� {itemToAdd.itemName} ������ � ���� � ������.");
                    NotifyUI();
                    return true;
                }
            }
        }

        // 2. ���� �� ����� ��������� ��� ���� ���� � �����, ������ ������ ����

        // ������ �������� ���� � ��������� ��������
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].item == null)
            {
                inventorySlots[i].item = itemToAdd;
                inventorySlots[i].stackSize = 1;
                Debug.Log($"������� {itemToAdd.itemName} ������ � ��������.");
                NotifyUI();
                return true;
            }
        }

        // ������ �������� ���� � ������ (���� �������� ������)
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].item == null)
            {
                hotbarSlots[i].item = itemToAdd;
                hotbarSlots[i].stackSize = 1;
                Debug.Log($"������� {itemToAdd.itemName} ������ � ������ (�������� ��� ������).");
                NotifyUI();
                return true;
            }
        }

        // ���� � ��������, � ������ �����
        Debug.Log("�������� � ������ �����!");
        return false;
    }

    // �����, ��� ��������� ��������� UI
    void NotifyUI()
    {
        if (onInventoryChangedCallback != null)
        {
            onInventoryChangedCallback.Invoke();
        }
    }

    // Метод для викидання предмета
    public void DropItem(InventorySlot slot)
{
    if (slot.item == null) return;

    // Шукаємо гравця
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        // Створюємо предмет перед гравцем
        GameObject dropped = Instantiate(slot.item.itemPrefab, player.transform.position + Vector3.up, Quaternion.identity);
        
        // Визначаємо напрямок (куди дивиться гравець)
        float lookDir = player.transform.localScale.x > 0 ? 1f : -1f;
      
    }

    slot.item = null;
    slot.stackSize = 0;
    NotifyUI();
}
}