using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public SpriteRenderer handSpriteRenderer;

    void Start()
    {
        // Підписуємося на оновлення інвентарю, щоб рука змінювалася відразу
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateHandVisual;
        }
        UpdateHandVisual();
    }

    public void UpdateHandVisual()
    {
        if (InventoryInteraction.Instance == null || InventoryManager.Instance == null) return;

        int activeIndex = InventoryInteraction.Instance.activeHotbarIndex;
        var activeSlot = InventoryManager.Instance.hotbarSlots[activeIndex];

        if (activeSlot.item != null)
        {
            handSpriteRenderer.sprite = activeSlot.item.icon;
            handSpriteRenderer.enabled = true;
        }
        else
        {
            handSpriteRenderer.enabled = false;
        }
    }

    // Не забуваємо відписатися при знищенні об'єкта
    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback -= UpdateHandVisual;
        }
    }
}