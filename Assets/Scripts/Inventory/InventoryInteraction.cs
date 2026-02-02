using UnityEngine;
using System.Collections.Generic;

public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction Instance;

    [Header("Налаштування")]
    public float clickCooldown = 0.5f; // Час перепочинку між кліками

    private int selectedIndex = -1;
    private bool isFromHotbar = false;
    private float lastClickTime;

    void Awake() => Instance = this;

    public void OnSlotClicked(int index, bool clickedHotbar)
    {
        // 0. Перевірка на "час перепочинку"
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        var manager = InventoryManager.Instance;
        List<InventorySlot> currentList = clickedHotbar ? manager.hotbarSlots : manager.inventorySlots;

        // 1. ВІДМІНА (Клікнув на той самий слот ще раз)
        if (selectedIndex == index && isFromHotbar == clickedHotbar)
        {
            selectedIndex = -1;
            Debug.Log("<color=white>[Inventory]</color> ВИБІР СКАСОВАНО (повторний клік).");
            return;
        }

        // 2. ВИБІР (Якщо ще нічого не взяли)
        if (selectedIndex == -1)
        {
            if (index >= 0 && index < currentList.Count && currentList[index].item != null)
            {
                selectedIndex = index;
                isFromHotbar = clickedHotbar;
                Debug.Log($"<color=yellow>[Inventory]</color> ВИБРАНО: Слот {index} ({(clickedHotbar ? "Хотбар" : "Інвентар")}). Предмет: {currentList[index].item.name}");
            }
            else
            {
                Debug.Log("<color=gray>[Inventory]</color> Клік по порожньому слоту. Виберіть предмет.");
            }
        }
        // 3. ДІЯ (ОБМІН АБО ПЕРЕМІЩЕННЯ)
        else
        {
            ExecuteAction(selectedIndex, isFromHotbar, index, clickedHotbar);
            selectedIndex = -1; // Скидаємо вибір після завершення дії
        }
    }

    void ExecuteAction(int idxA, bool wasHotbarA, int idxB, bool isHotbarB)
    {
        var manager = InventoryManager.Instance;
        List<InventorySlot> listA = wasHotbarA ? manager.hotbarSlots : manager.inventorySlots;
        List<InventorySlot> listB = isHotbarB ? manager.hotbarSlots : manager.inventorySlots;

        InventorySlot slotA = listA[idxA];
        InventorySlot slotB = listB[idxB];

        // Перевірка на Переміщення чи Обмін
        if (slotB.item != null)
        {
            Debug.Log($"<color=cyan>[Inventory]</color> ОБМІН: {slotA.item.name} <-> {slotB.item.name}");
        }
        else
        {
            Debug.Log($"<color=green>[Inventory]</color> ПЕРЕМІЩЕННЯ: {slotA.item.name} у пустий слот {idxB}");
        }

        // ЛОГІКА ОБМІНУ ДАНИМИ
        Item tempItem = slotA.item;
        int tempSize = slotA.stackSize;

        slotA.item = slotB.item;
        slotA.stackSize = slotB.stackSize;

        slotB.item = tempItem;
        slotB.stackSize = tempSize;

        // Оновлюємо UI через твій менеджер
        if (manager.onInventoryChangedCallback != null)
        {
            manager.onInventoryChangedCallback.Invoke();
        }
    }
}