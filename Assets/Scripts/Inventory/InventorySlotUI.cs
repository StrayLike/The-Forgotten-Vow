using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI stackText;
    public InventorySlot currentSlot;
    public int slotIndex;
    public bool isHotbarSlot;
    [Header("Виділення")]
    public GameObject selectionPanel;
    public GameObject mouseSelectionPanel;
    public bool isChestSlot;

    public void AddItem(InventorySlot slot)
    {
        currentSlot = slot;
        if (icon == null) return;

        if (slot != null && slot.item != null)
        {
            icon.sprite = slot.item.icon;
            icon.enabled = true;
            if (stackText != null)
            {
                stackText.text = (slot.item.isStackable && slot.stackSize > 1) ? slot.stackSize.ToString() : "";
            }
        }
        else
        {
            ClearSlot();
        }
    }

    public void UpdateSelection()
    {
        if (InventoryInteraction.Instance == null) return;

        if (selectionPanel != null)
            selectionPanel.SetActive(InventoryInteraction.Instance.IsHotbarActive(slotIndex, isHotbarSlot));

        if (mouseSelectionPanel != null)
            mouseSelectionPanel.SetActive(InventoryInteraction.Instance.IsMouseSelected(slotIndex, isHotbarSlot, isChestSlot));
    }

    public void ClearSlot()
    {
        currentSlot = null;
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        if (stackText != null) stackText.text = "";
    }

    public void ClickOnSlot()
    {
        if (InventoryInteraction.Instance != null)
            InventoryInteraction.Instance.OnSlotClicked(slotIndex, isHotbarSlot, isChestSlot);

        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }
}