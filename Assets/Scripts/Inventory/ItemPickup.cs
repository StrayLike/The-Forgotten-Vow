using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public int amount = 1;

    [Header("Анімація")]
    public float rotationSpeed = 100f;

    private bool canBePickedUp = true;
    private float timer = 0f;
    private float pickupDelay = 2f;

    void Update()
    {
        // 1. Таймер затримки після викидання
        if (!canBePickedUp)
        {
            timer += Time.deltaTime;
            if (timer >= pickupDelay) canBePickedUp = true;
        }

        // 2. Красиве обертання (по вісі Y)
        // Якщо твій спрайт стає невидимим при повороті на 90 градусів, 
        // переконайся, що в Sprite Renderer стоїть правильний Material
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    public void StartPickupDelay()
    {
        canBePickedUp = false;
        timer = 0f;
    }

    // Реєструємо підбір
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) TryPickup();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) TryPickup();
    }

    private void TryPickup()
    {
        if (!canBePickedUp) return;

        // Питаємо менеджера, скільки він може взяти
        int added = InventoryManager.Instance.AddItem(item, amount);

        if (added >= amount)
        {
            // Забрали все - видаляємо об'єкт
            Destroy(gameObject);
        }
        else
        {
            // Забрали частину - оновлюємо залишок на землі
            amount -= added;
            Debug.Log($"[Pickup] Інвентар майже повний. Залишилося на землі: {amount}");
        }
    }
}