using UnityEngine;

public class InventoryInteraction : MonoBehaviour
{
    public static InventoryInteraction Instance;
    private int selectedIndex = -1;

    void Awake() => Instance = this;

    public void OnSlotClicked(int index)
    {
        var manager = InventoryManager.Instance;

        if (index < 0 || index >= manager.inventorySlots.Count) return;

        // 1. Якщо нічого не вибрано - вибираємо
        if (selectedIndex == -1)
        {
            if (manager.inventorySlots[index].item != null)
            {
                selectedIndex = index;
                Debug.Log($"<color=green>Вибрано слот {index}</color>");
            }
        }
        // 2. Якщо вже був вибраний слот - робимо обмін
        else
        {
            // Якщо клікнули на той самий слот - просто знімаємо виділення
            if (selectedIndex == index)
            {
                selectedIndex = -1;
                Debug.Log("Виділення знято");
                return;
            }

            Swap(selectedIndex, index);

            // КРИТИЧНО: Скидаємо індекс і ВИХОДИМО, щоб цей клік не став "першим" для нового вибору
            selectedIndex = -1;
        }
    }

    void Swap(int a, int b)
    {
        var slots = InventoryManager.Instance.inventorySlots;

        Item tempItem = slots[a].item;
        int tempSize = slots[a].stackSize;

        slots[a].item = slots[b].item;
        slots[a].stackSize = slots[b].stackSize;

        slots[b].item = tempItem;
        slots[b].stackSize = tempSize;

        InventoryManager.Instance.onInventoryChangedCallback.Invoke();
        Debug.Log($"<color=blue>Успішний обмін: {a} <-> {b}</color>");
    }
}