using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Змінна для посилання на наш предмет

    void OnTriggerEnter2D(Collider2D other)
    {
        // Перевіряємо, чи об'єкт, що увійшов в тригер, є гравцем
        if (other.CompareTag("Player"))
        {
            // Намагаємося додати предмет в інвентар
            if (InventoryManager.Instance.AddItem(item))
            {
                // Якщо предмет додано, знищуємо ігровий об'єкт підбору
                Destroy(gameObject);
            }
        }
    }
}