using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class canteenTableSc : MonoBehaviour
{
    public bool firstBenchDown = false;
    public bool secondBenchDown = false;

    public Transform bench1;
    public Transform bench2;

    public TMP_Text paperNumber;

    public float platesCount;

    public float childrenCount;

    public List<GameObject> plates = new List<GameObject>();

    private void Start()
    {
        bench1 = this.transform.Find("firstBench");
        bench2 = this.transform.Find("secBench");
        SetPaperNumber();
    }
    private void Update()
    {
        platesCount = plates.Count;
        
    }

    private void OnTransformChildrenChanged()
    {
        // Проходим по всем дочерним объектам
        foreach (Transform child in transform)
        {
            // Проверяем, есть ли объект с именем "plate" и отсутствует ли он уже в списке
            if (child.gameObject.CompareTag("plate") && !plates.Contains(child.gameObject))
            {
                // Добавляем в список
                plates.Add(child.gameObject);
                Debug.Log($"Добавлен объект: {child.gameObject.name}");
            }
        }

        // Удаляем из списка отсутствующие в иерархии объекты
        plates.RemoveAll(plate => plate == null || plate.transform.parent != transform);
    }

    void SetPaperNumber()
    {
        float randomFloat = Random.Range(6, 7);
        childrenCount= randomFloat;
        paperNumber.text = randomFloat.ToString();
    }
}
