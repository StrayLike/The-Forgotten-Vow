using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform hotbarParent;
    public GameObject inventoryPanel;

    private InventorySlotUI[] hotbarSlotsUI;
    private InventorySlotUI[] inventorySlotsUI;
    private InventoryManager inventoryManager;
    public Transform chestSlotsParent; // ПРИЗНАЧ ПАНЕЛЬ СКРИНІ
    private InventorySlotUI[] chestSlotsUI;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;

        if (hotbarParent != null)
            hotbarSlotsUI = hotbarParent.GetComponentsInChildren<InventorySlotUI>();

        if (inventoryPanel != null)
            inventorySlotsUI = inventoryPanel.GetComponentsInChildren<InventorySlotUI>();

        if (inventoryManager != null)
            inventoryManager.onInventoryChangedCallback += UpdateUI;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (chestSlotsParent != null)
            chestSlotsUI = chestSlotsParent.GetComponentsInChildren<InventorySlotUI>();

        UpdateUI();
    }

    // МЕТОД Update ВИДАЛЕНО, бо тепер керування на клавішу E в InventoryInteraction.cs

    public void UpdateUI()
    {
        if (inventoryManager == null) return;

        // 1. Оновлення Хотбару
        if (hotbarSlotsUI != null)
        {
            for (int i = 0; i < hotbarSlotsUI.Length; i++)
            {
                if (i < inventoryManager.hotbarSlots.Count)
                {
                    hotbarSlotsUI[i].AddItem(inventoryManager.hotbarSlots[i]);
                    hotbarSlotsUI[i].UpdateSelection();
                }
            }
        }

        // 2. Оновлення Скрині
        if (InventoryInteraction.Instance != null && InventoryInteraction.Instance.currentChest != null && chestSlotsUI != null)
        {
            var chestSlots = InventoryInteraction.Instance.currentChest.chestSlots;
            for (int j = 0; j < chestSlotsUI.Length; j++)
            {
                if (j < chestSlots.Count)
                {
                    chestSlotsUI[j].AddItem(chestSlots[j]);
                    chestSlotsUI[j].UpdateSelection();
                }
            }
        }

        // 3. Оновлення Інвентарю
        if (inventorySlotsUI != null)
        {
            for (int i = 0; i < inventorySlotsUI.Length; i++)
            {
                if (i < inventoryManager.inventorySlots.Count)
                {
                    inventorySlotsUI[i].AddItem(inventoryManager.inventorySlots[i]);
                    inventorySlotsUI[i].UpdateSelection();
                }
            }
        }
    }
}