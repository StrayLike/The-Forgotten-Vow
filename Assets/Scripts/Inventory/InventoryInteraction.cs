using UnityEngine;
using System.Collections.Generic;

public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction Instance;

    [Header("Налаштування")]
    public float clickCooldown = 0.5f;

    [Header("Посилання")]
    public Transform playerTransform;   // Гравця все одно треба вказати
    public float throwForce = 4f;

    private int selectedIndex = -1;
    private bool isFromHotbar = false;
    private float lastClickTime;

    void Awake() => Instance = this;

    public void OnSlotClicked(int index, bool clickedHotbar)
    {
        if (Time.time - lastClickTime < clickCooldown) return;
        lastClickTime = Time.time;

        var manager = InventoryManager.Instance;
        List<InventorySlot> currentList = clickedHotbar ? manager.hotbarSlots : manager.inventorySlots;

        if (selectedIndex == index && isFromHotbar == clickedHotbar)
        {
            selectedIndex = -1;
            Debug.Log("<color=white>[Inventory]</color> ВИБІР СКАСОВАНО.");
            return;
        }

        if (selectedIndex == -1)
        {
            if (index >= 0 && index < currentList.Count && currentList[index].item != null)
            {
                selectedIndex = index;
                isFromHotbar = clickedHotbar;
                Debug.Log($"<color=yellow>[Inventory]</color> ВИБРАНО: {currentList[index].item.name}");
            }
        }
        else
        {
            ExecuteAction(selectedIndex, isFromHotbar, index, clickedHotbar);
            selectedIndex = -1;
        }
    }

    // ГОЛОВНИЙ МЕТОД ДЛЯ ВИКИДАННЯ
    public void DropSelectedItem()
    {
        if (selectedIndex == -1) return;

        var manager = InventoryManager.Instance;
        List<InventorySlot> currentList = isFromHotbar ? manager.hotbarSlots : manager.inventorySlots;
        InventorySlot slot = currentList[selectedIndex];

        if (slot.item != null)
        {
            // Перевіряємо, чи ми додали префаб у ScriptableObject
            if (slot.item.itemPrefab == null)
            {
                Debug.LogError($"У предмета {slot.item.name} не призначено Item Prefab!");
                return;
            }

            Debug.Log($"<color=red>[Inventory]</color> ВИКИНУТО: {slot.item.name}");

            // 1. Спавнимо саме той префаб, який належить предмету
            Vector3 dropPos = playerTransform.position + playerTransform.up * 1.0f;
            GameObject droppedObj = Instantiate(slot.item.itemPrefab, dropPos, Quaternion.identity);

            // 2. Налаштовуємо дані у скрипті підбору (щоб його можна було підняти знову)
            ItemPickup pickup = droppedObj.GetComponent<ItemPickup>();
            if (pickup != null) pickup.item = slot.item;

            // 3. Додаємо імпульс вильоту
            Rigidbody2D rb = droppedObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 throwDir = Random.insideUnitCircle.normalized;
                rb.AddForce(throwDir * throwForce, ForceMode2D.Impulse);
            }

            // 4. Очищуємо слот
            slot.item = null;
            slot.stackSize = 0;

            selectedIndex = -1;
            manager.onInventoryChangedCallback.Invoke();
        }
    }

    void ExecuteAction(int idxA, bool wasHotbarA, int idxB, bool isHotbarB)
    {
        var manager = InventoryManager.Instance;
        List<InventorySlot> listA = wasHotbarA ? manager.hotbarSlots : manager.inventorySlots;
        List<InventorySlot> listB = isHotbarB ? manager.hotbarSlots : manager.inventorySlots;

        InventorySlot slotA = listA[idxA];
        InventorySlot slotB = listB[idxB];

        if (slotB.item != null && slotA.item == slotB.item && slotB.item.isStackable)
        {
            slotB.stackSize += slotA.stackSize;
            slotA.item = null;
            slotA.stackSize = 0;
            Debug.Log("<color=purple>[Inventory]</color> СТАКАННЯ завершено!");
        }
        else
        {
            Item tempItem = slotA.item;
            int tempSize = slotA.stackSize;
            slotA.item = slotB.item;
            slotA.stackSize = slotB.stackSize;
            slotB.item = tempItem;
            slotB.stackSize = tempSize;
            Debug.Log("<color=cyan>[Inventory]</color> ПЕРЕМІЩЕННЯ завершено.");
        }

        manager.onInventoryChangedCallback.Invoke();
    }
}