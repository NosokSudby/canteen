using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMat : MonoBehaviour
{
    public Material[] materials; // Массив материалов для смены
    RoomManager rm;
    private Renderer objectRenderer;
    private int currentMaterialIndex = 0;

    void Start()
    {
        rm = FindAnyObjectByType<RoomManager>();
        // Получаем Renderer объекта
        objectRenderer = GetComponent<Renderer>();

        if (materials.Length > 0 && objectRenderer != null)
        {
            // Устанавливаем первый материал
            objectRenderer.material = materials[currentMaterialIndex];
        }
        else
        {
            Debug.LogWarning("Материалы не назначены или Renderer не найден!");
        }
    }

    // Переключение на следующий материал
    public void NextMaterial()
    {
        if (materials.Length == 0 || objectRenderer == null) return;

        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
        objectRenderer.material = materials[currentMaterialIndex];
        rm.color = currentMaterialIndex;
    }

    // Переключение на предыдущий материал
    public void PreviousMaterial()
    {
        if (materials.Length == 0 || objectRenderer == null) return;

        currentMaterialIndex = (currentMaterialIndex - 1 + materials.Length) % materials.Length;
        objectRenderer.material = materials[currentMaterialIndex];
        rm.color = currentMaterialIndex;
    }
}
