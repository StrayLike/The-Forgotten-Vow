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

    public int AddItem(Item item, int amount = 1)
    {
        int remaining = amount;

        // 1. Якщо предмет стакається, спочатку заповнюємо існуючі слоти
        if (item.isStackable)
        {
            remaining = FillExistingStacks(item, remaining);
        }

        // 2. Якщо ще щось залишилося, шукаємо порожні слоти
        if (remaining > 0)
        {
            remaining = FillEmptySlots(item, remaining);
        }

        // Оновлюємо UI, якщо хоча б один предмет було додано
        if (remaining < amount && onInventoryChangedCallback != null)
        {
            onInventoryChangedCallback.Invoke();
        }

        return amount - remaining; // Повертаємо, скільки реально взяли
    }

    private int FillExistingStacks(Item item, int amount)
    {
        // Об'єднуємо всі слоти в один список для зручного пошуку
        List<InventorySlot> allSlots = new List<InventorySlot>(inventorySlots);
        allSlots.AddRange(hotbarSlots);

        foreach (var slot in allSlots)
        {
            if (slot.item == item && slot.stackSize < item.maxStackSize)
            {
                int canAdd = item.maxStackSize - slot.stackSize;
                int toAdd = Mathf.Min(canAdd, amount);

                slot.stackSize += toAdd;
                amount -= toAdd;

                if (amount <= 0) return 0;
            }
        }
        return amount;
    }

    private int FillEmptySlots(Item item, int amount)
    {
        List<InventorySlot> allSlots = new List<InventorySlot>(inventorySlots);
        allSlots.AddRange(hotbarSlots);

        foreach (var slot in allSlots)
        {
            if (slot.item == null)
            {
                int toAdd = Mathf.Min(item.maxStackSize, amount);
                slot.item = item;
                slot.stackSize = toAdd;
                amount -= toAdd;

                if (amount <= 0) return 0;
            }
        }
        return amount;
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