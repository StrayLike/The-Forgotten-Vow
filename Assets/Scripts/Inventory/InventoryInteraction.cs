using UnityEngine;
using System.Collections.Generic;

public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction Instance;

    [Header("Налаштування")]
    public float clickCooldown = 0.2f;
    public Transform playerTransform;
    public float throwForce = 4f;

    [Header("UI та Стан")]
    public GameObject chestPanel;
    public GameObject mainInventoryPanel;
    public int activeHotbarIndex = 0;

    [Header("Поточна скриня")]
    public Chest currentChest;
    private int selectedIndex = -1;
    private bool isFromHotbar = false;
    private bool isFromChest = false;
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

        // 2. Керування клавішею E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentChest != null)
            {
                CloseChest();
                if (mainInventoryPanel != null) mainInventoryPanel.SetActive(false);
            }
            else if (mainInventoryPanel != null)
            {
                mainInventoryPanel.SetActive(!mainInventoryPanel.activeSelf);
                if (mainInventoryPanel.activeSelf)
                {
                    InventoryManager.Instance.onInventoryChangedCallback.Invoke();
                }
            }
        }

        // 3. Автоматичне закриття скрині
        if (currentChest != null && playerTransform != null)
        {
            float dist = Vector2.Distance(playerTransform.position, currentChest.transform.position);
            if (dist > 2.5f)
            {
                CloseChest();
                if (mainInventoryPanel != null) mainInventoryPanel.SetActive(false);
            }
        }

        // 4. Логіка викидання на Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Якщо ми вибрали предмет мишкою (він підсвічений) - викидаємо його
            if (selectedIndex != -1)
            {
                DropSelectedItem();
            }
            else
            {
                // Якщо нічого не вибрано - викидаємо предмет з активного слота хотбару
                DropItemFromHotbar(activeHotbarIndex);
            }
        }
    }

    // --- НОВИЙ МЕТОД: Викидання вибраного мишкою предмета ---
    public void DropSelectedItem()
    {
        if (selectedIndex == -1) return;

        List<InventorySlot> selectedList = GetList(isFromHotbar, isFromChest);
        InventorySlot slot = selectedList[selectedIndex];

        if (slot.item != null)
        {
            InventoryManager.Instance.DropItem(slot);
            selectedIndex = -1; // Скидаємо виділення після викидання
            InventoryManager.Instance.onInventoryChangedCallback.Invoke();
        }
    }

    // --- ІСНУЮЧИЙ МЕТОД: Викидання з хотбару (для Q без виділення) ---
    public void DropItemFromHotbar(int index)
    {
        InventorySlot slot = InventoryManager.Instance.hotbarSlots[index];
        if (slot.item != null)
        {
            InventoryManager.Instance.DropItem(slot);
            InventoryManager.Instance.onInventoryChangedCallback.Invoke();
        }
    }

    // Решта методів без змін...
    public void OpenChest(Chest chest)
    {
        currentChest = chest;
        if (chestPanel != null) chestPanel.SetActive(true);
        if (mainInventoryPanel != null) mainInventoryPanel.SetActive(true);
        InventoryManager.Instance.onInventoryChangedCallback.Invoke();
    }

    public void CloseChest()
    {
        if (currentChest != null)
        {
            currentChest.CloseChestVisual(); // Це вимкне прапорець isOpen і поверне закритий спрайт
        }
        currentChest = null;
        if (chestPanel != null) chestPanel.SetActive(false);
        InventoryManager.Instance.onInventoryChangedCallback.Invoke(); // Оновлюємо UI
    }

    public void OnSlotClicked(int index, bool clickedHotbar, bool clickedChest)
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        var manager = InventoryManager.Instance;
        List<InventorySlot> clickedList = GetList(clickedHotbar, clickedChest);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (clickedList[index].item != null)
            {
                HandleShiftClick(clickedList[index], clickedChest);
                manager.onInventoryChangedCallback.Invoke();
                return;
            }
        }

        if (selectedIndex == -1)
        {
            if (clickedList[index].item != null)
            {
                selectedIndex = index;
                isFromHotbar = clickedHotbar;
                isFromChest = clickedChest;
            }
        }
        else
        {
            ExecuteAction(selectedIndex, isFromHotbar, isFromChest, index, clickedHotbar, clickedChest);
            selectedIndex = -1;
        }
        manager.onInventoryChangedCallback.Invoke();
        if (currentChest != null)
        {
            currentChest.UpdateChestVisual();
        }
    }

    private List<InventorySlot> GetList(bool hotbar, bool chest)
    {
        if (chest && currentChest != null) return currentChest.chestSlots;
        return hotbar ? InventoryManager.Instance.hotbarSlots : InventoryManager.Instance.inventorySlots;
    }

    void HandleShiftClick(InventorySlot slot, bool fromChest)
    {
        var manager = InventoryManager.Instance;
        if (fromChest)
            MoveToFirstFree(slot, manager.inventorySlots);
        else if (currentChest != null)
            MoveToFirstFree(slot, currentChest.chestSlots);
        else
            MoveToFirstFree(slot, isFromHotbar ? manager.inventorySlots : manager.hotbarSlots);
    }

    void MoveToFirstFree(InventorySlot source, List<InventorySlot> target)
    {
        foreach (var slot in target)
        {
            if (slot.item == source.item && slot.item.isStackable)
            {
                int canAdd = slot.item.maxStackSize - slot.stackSize;
                int toAdd = Mathf.Min(canAdd, source.stackSize);
                slot.stackSize += toAdd;
                source.stackSize -= toAdd;
                if (source.stackSize <= 0) { source.item = null; return; }
            }
        }
        foreach (var slot in target)
        {
            if (slot.item == null)
            {
                slot.item = source.item;
                slot.stackSize = source.stackSize;
                source.item = null;
                source.stackSize = 0;
                return;
            }
        }
    }

    void ExecuteAction(int idxA, bool hotA, bool chestA, int idxB, bool hotB, bool chestB)
    {
        List<InventorySlot> listA = GetList(hotA, chestA);
        List<InventorySlot> listB = GetList(hotB, chestB);

        InventorySlot slotA = listA[idxA];
        InventorySlot slotB = listB[idxB];

        if (slotB.item != null && slotA.item == slotB.item && slotB.item.isStackable)
        {
            int toAdd = Mathf.Min(slotB.item.maxStackSize - slotB.stackSize, slotA.stackSize);
            slotB.stackSize += toAdd;
            slotA.stackSize -= toAdd;
            if (slotA.stackSize <= 0) slotA.item = null;
        }
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

    public bool IsHotbarActive(int index, bool hotbar) => hotbar && index == activeHotbarIndex;
    public bool IsMouseSelected(int index, bool hotbar, bool chest) => index == selectedIndex && hotbar == isFromHotbar && chest == isFromChest;
}