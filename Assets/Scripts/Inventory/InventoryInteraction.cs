using UnityEngine;
using System.Collections.Generic;

public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction Instance;

    [Header("Налаштування")]
    public float clickCooldown = 0.5f;

    [Header("Посилання")]
    public Transform playerTransform;
    public float throwForce = 4f;

    [Header("Стан Хотбару")]
    public int activeHotbarIndex = 0;

    private int selectedIndex = -1;
    private bool isFromHotbar = false;
    private float lastClickTime;

    void Awake() => Instance = this;

    void Update()
    {
        // 1. Перемикання хотбару
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                activeHotbarIndex = i;
                InventoryManager.Instance.onInventoryChangedCallback.Invoke();
            }
        }

        // 2. Викидання на Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool dropAll = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            DropItemFromHotbar(activeHotbarIndex, dropAll);
        }
    }

    // --- МЕТОДИ ВИКИДАННЯ ---

    void DropItemFromHotbar(int index, bool all)
    {
        var manager = InventoryManager.Instance;
        InventorySlot slot = manager.hotbarSlots[index];
        if (slot.item != null) PerformDrop(slot, all);
    }

    public void DropSelectedItem()
    {
        if (selectedIndex == -1) return;
        var manager = InventoryManager.Instance;
        List<InventorySlot> currentList = isFromHotbar ? manager.hotbarSlots : manager.inventorySlots;
        InventorySlot slot = currentList[selectedIndex];

        if (slot.item != null)
        {
            PerformDrop(slot, true);
            selectedIndex = -1;
        }
    }

    void PerformDrop(InventorySlot slot, bool all)
    {
        int amountToDrop = all ? slot.stackSize : 1;
        SpawnItemWorld(slot.item, amountToDrop);

        slot.stackSize -= amountToDrop;
        if (slot.stackSize <= 0) slot.item = null;

        InventoryManager.Instance.onInventoryChangedCallback.Invoke();
    }

    void SpawnItemWorld(Item item, int amount)
    {
        Vector3 dropPos = playerTransform.position + playerTransform.up * 1.0f;
        GameObject droppedObj = Instantiate(item.itemPrefab, dropPos, Quaternion.identity);

        ItemPickup pickup = droppedObj.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            pickup.item = item;
            pickup.amount = amount;
            pickup.StartPickupDelay();
        }

        Rigidbody2D rb = droppedObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 throwDir = Random.insideUnitCircle.normalized;
            rb.AddForce(throwDir * throwForce, ForceMode2D.Impulse);
        }
    }

    // --- ЛОГІКА КЛІКІВ ТА ОБМІНУ ---

    public void OnSlotClicked(int index, bool clickedHotbar)
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        var manager = InventoryManager.Instance;

        if (selectedIndex == index && isFromHotbar == clickedHotbar)
        {
            selectedIndex = -1;
        }
        else if (selectedIndex == -1)
        {
            List<InventorySlot> currentList = clickedHotbar ? manager.hotbarSlots : manager.inventorySlots;
            if (currentList[index].item != null)
            {
                selectedIndex = index;
                isFromHotbar = clickedHotbar;
            }
        }
        else
        {
            // ВИКЛИК МЕТОДУ, ЯКИЙ БУВ ВІДСУТНІЙ:
            ExecuteAction(selectedIndex, isFromHotbar, index, clickedHotbar);
            selectedIndex = -1;
        }
        manager.onInventoryChangedCallback.Invoke();
    }

    // ТОЙ САМИЙ МЕТОД ExecuteAction
    void ExecuteAction(int idxA, bool wasHotbarA, int idxB, bool isHotbarB)
    {
        var manager = InventoryManager.Instance;
        List<InventorySlot> listA = wasHotbarA ? manager.hotbarSlots : manager.inventorySlots;
        List<InventorySlot> listB = isHotbarB ? manager.hotbarSlots : manager.inventorySlots;

        InventorySlot slotA = listA[idxA];
        InventorySlot slotB = listB[idxB];

        // 1. Логіка стакання
        if (slotB.item != null && slotA.item == slotB.item && slotB.item.isStackable)
        {
            int max = slotB.item.maxStackSize;
            int canAdd = max - slotB.stackSize;
            int toAdd = Mathf.Min(canAdd, slotA.stackSize);

            slotB.stackSize += toAdd;
            slotA.stackSize -= toAdd;

            if (slotA.stackSize <= 0) slotA.item = null;
        }
        // 2. Логіка звичайного обміну
        else
        {
            Item tempItem = slotA.item;
            int tempSize = slotA.stackSize;

            slotA.item = slotB.item;
            slotA.stackSize = slotB.stackSize;

            slotB.item = tempItem;
            slotB.stackSize = tempSize;
        }
    }

    // ПЕРЕВІРКИ ДЛЯ UI
    public bool IsHotbarActive(int index, bool isHotbar) => isHotbar && index == activeHotbarIndex;
    public bool IsMouseSelected(int index, bool isHotbar) => index == selectedIndex && isHotbar == isFromHotbar;
}