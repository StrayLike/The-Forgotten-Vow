using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform hotbarParent;    
    public GameObject inventoryPanel; 

    private InventorySlotUI[] hotbarSlotsUI;
    private InventorySlotUI[] inventorySlotsUI;
    private InventoryManager inventoryManager;

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

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                if (inventoryPanel.activeSelf) UpdateUI();
            }
        }
    }

    public void UpdateUI()
{
    if (inventoryManager == null) return;

    // Оновлення Хотбару
    for (int i = 0; i < hotbarSlotsUI.Length; i++)
    {
        if (i < inventoryManager.hotbarSlots.Count)
        {
            // Ми передаємо саме об'єкт InventorySlot
            hotbarSlotsUI[i].AddItem(inventoryManager.hotbarSlots[i]);
        }
    }

    // Оновлення Інвентарю
    for (int i = 0; i < inventorySlotsUI.Length; i++)
    {
        if (i < inventoryManager.inventorySlots.Count)
        {
            inventorySlotsUI[i].AddItem(inventoryManager.inventorySlots[i]);
        }
    }
}
}