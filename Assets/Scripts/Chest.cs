using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    [Header("Налаштування")]
    public int chestSize = 15;
    public List<InventorySlot> chestSlots = new List<InventorySlot>();

    [Header("Візуал (Спрайти)")]
    public SpriteRenderer spriteRenderer;
    public Sprite closedSprite;
    public Sprite openEmptySprite;
    public Sprite openFullSprite;

    private bool isOpen = false;

    void Awake()
    {
        for (int i = 0; i < chestSize; i++)
        {
            chestSlots.Add(new InventorySlot(null, 0));
        }
        if (spriteRenderer != null) spriteRenderer.sprite = closedSprite;
    }

    void Start()
    {
        // Підписуємося на подію оновлення інвентарю
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback += UpdateChestVisual;
        }
    }

    public void Interact()
    {
        isOpen = true;
        InventoryInteraction.Instance.OpenChest(this);
        UpdateChestVisual();
    }

    public void UpdateChestVisual()
    {
        // Оновлюємо спрайт тільки якщо скриня зараз відкрита
        if (spriteRenderer == null || !isOpen) return;

        bool hasItems = false;
        foreach (var slot in chestSlots)
        {
            if (slot.item != null)
            {
                hasItems = true;
                break;
            }
        }

        spriteRenderer.sprite = hasItems ? openFullSprite : openEmptySprite;
    }

    public void CloseChestVisual()
    {
        isOpen = false;
        if (spriteRenderer != null) spriteRenderer.sprite = closedSprite;
    }

    private void OnMouseDown()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= 2.5f)
        {
            Interact();
        }
    }

    void OnDestroy()
    {
        // Відписуємося при видаленні об'єкта, щоб не було помилок
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChangedCallback -= UpdateChestVisual;
        }
    }
}