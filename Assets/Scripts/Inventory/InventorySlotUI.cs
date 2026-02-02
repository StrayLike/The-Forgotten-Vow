using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI stackText;
    public InventorySlot currentSlot;

    // ДОДАЙ ЦЕ: номер слота в інвентарі (0, 1, 2...)
    public int slotIndex;

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
        else { ClearSlot(); }
    }

    public void ClearSlot()
    {
        currentSlot = null;
        if (icon != null) { icon.sprite = null; icon.enabled = false; }
        if (stackText != null) stackText.text = "";
    }
   
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Прямий клік по об'єкту: " + gameObject.name);
        ClickOnSlot();
    }

    // ДОДАЙ ЦЕ: метод для обробки кліку
    public void ClickOnSlot()
    {
        if (InventoryInteraction.Instance != null)
        {
            InventoryInteraction.Instance.OnSlotClicked(slotIndex);
        }
    }
}