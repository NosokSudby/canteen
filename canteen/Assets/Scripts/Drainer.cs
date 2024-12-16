using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Drainer : MonoBehaviourPun
{
    public bool hasSpoons;

    [SerializeField] private Transform sourceParent; // Объект, от которого считаем количество дочерних объектов
    [SerializeField] private Transform targetParent; // Объект, дочерние объекты которого будем активировать


    void Start()
    {
        if (sourceParent == null || targetParent == null)
        {
            Debug.LogError("Source or Target Parent is not assigned!");
            return;
        }
    }

    private void Update()
    {
        // Считаем количество дочерних объектов у sourceParent
        int sourceChildCount = sourceParent.childCount;

        // Активируем дочерние объекты targetParent, не совпадающие по индексу с sourceParent
        ActivateNonMatchingChildren(targetParent, sourceChildCount);

        // Деактивируем дочерние объекты targetParent, совпадающие по индексу с sourceParent
        DisableMatchingChildren(targetParent, sourceChildCount);
    }

    private void ActivateNonMatchingChildren(Transform parent, int countToMatch)
    {
        int childCount = parent.childCount;

        // Перебираем всех дочерних объектов у targetParent
        for (int i = 0; i < childCount; i++)
        {
            // Активируем объект, если индекс больше или равен countToMatch
            parent.GetChild(i).gameObject.SetActive(i >= countToMatch);
        }
    }

    private void DisableMatchingChildren(Transform parent, int countToMatch)
    {
        int childCount = parent.childCount;

        // Перебираем всех дочерних объектов у targetParent
        for (int i = 0; i < childCount; i++)
        {
            // Деактивируем объект, если индекс меньше countToMatch
            parent.GetChild(i).gameObject.SetActive(i < countToMatch);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("spoon"))
        {

            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.SetParent(transform.Find("normalSpoons").transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("spoon"))
        {
            other.transform.SetParent(null);
        }
    }
}
