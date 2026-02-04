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
        // Якщо скрипт взаємодії ще не прокинувся, виходимо
        if (InventoryInteraction.Instance == null) return;

        // Перевіряємо рамку активного хотбару (1-5)
        if (selectionPanel != null)
        {
            bool isActive = InventoryInteraction.Instance.IsHotbarActive(slotIndex, isHotbarSlot);
            selectionPanel.SetActive(isActive);
        }

        // Перевіряємо рамку вибору мишкою
        if (mouseSelectionPanel != null)
        {
            bool isSelected = InventoryInteraction.Instance.IsMouseSelected(slotIndex, isHotbarSlot);
            mouseSelectionPanel.SetActive(isSelected);
        }
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
        {
            InventoryInteraction.Instance.OnSlotClicked(slotIndex, isHotbarSlot);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}