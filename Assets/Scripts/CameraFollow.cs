using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Об'єкт, за яким буде слідувати камера (твій гравець)
    public Transform target;

    // Плавність руху камери
    public float smoothSpeed = 0.125f;

    // Відстань камери від гравця (по осі Z)
    public float zOffset = -10f;

    void LateUpdate()
    {
        // Перевіряємо, чи існує об'єкт-ціль
        if (target == null)
        {
            Debug.LogWarning("Target not assigned for CameraFollow script!");
            return;
        }

        // Визначаємо бажану позицію камери
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, zOffset);

        // Плавно переміщуємо камеру до бажаної позиції
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Встановлюємо нову позицію для камери
        transform.position = smoothedPosition;
    }
}