using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Іконки курсорів")]
    public Texture2D defaultCursor;   // Звичайна стрілка
    public Texture2D interactCursor;  // Для скрині та слотів

    [Header("Налаштування")]
    public Vector2 hotSpot = Vector2.zero;
    public float interactionDistance = 2.5f; // Та сама відстань, що в InventoryInteraction
    public Transform playerTransform;        // Перетягни сюди гравця в інспекторі

    void Awake()
    {
        Instance = this;
        SetDefaultCursor();
    }

    void Update()
    {
        // 1. Перевірка наведення на UI (Слоти завжди активні для курсору)
        if (EventSystem.current.IsPointerOverGameObject())
        {
            SetInteractCursor();
            return;
        }

        // 2. Перевірка наведення на об'єкти у світі (Скриня)
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null && playerTransform != null)
            {
                // ПЕРЕВІРКА ВІДСТАНІ: міняємо курсор тільки якщо гравець поруч
                float dist = Vector2.Distance(playerTransform.position, chest.transform.position);
                if (dist <= interactionDistance)
                {
                    SetInteractCursor();
                    return;
                }
            }
        }

        // Якщо занадто далеко або нікуди не наведено — звичайний курсор
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    public void SetInteractCursor()
    {
        Cursor.SetCursor(interactCursor, hotSpot, CursorMode.Auto);
    }
}