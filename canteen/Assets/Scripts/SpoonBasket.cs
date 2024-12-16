using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpoonBasket : MonoBehaviourPun
{
    public float spoonCounter;
    public List<GameObject> spoons = new List<GameObject>();

    private void Update()
    {
        // Удаляем неактивные или удаленные ложки из списка
        spoons.RemoveAll(spoon => spoon == null || !spoon.activeInHierarchy);
        spoonCounter = spoons.Count; // Обновляем счетчик ложек
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("spoon") && !spoons.Contains(other.gameObject))
        {
            // Добавляем ложку в список локально
            spoons.Add(other.gameObject);
            spoonCounter = spoons.Count;

            // Синхронизируем добавление ложки у всех игроков
            photonView.RPC("AddSpoonToBasket", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("spoon") && spoons.Contains(other.gameObject))
        {
            // Убираем ложку из списка локально
            spoons.Remove(other.gameObject);
            spoonCounter = spoons.Count;

            // Синхронизируем удаление ложки у всех игроков
            photonView.RPC("RemoveSpoonFromBasket", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void AddSpoonToBasket(int spoonViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject spoon = PhotonView.Find(spoonViewID)?.gameObject;

        if (spoon != null)
        {
            // Делаем корзину родителем ложки
            spoon.transform.parent = transform;

            // Обновляем список ложек локально
            if (!spoons.Contains(spoon))
            {
                spoons.Add(spoon);
                spoonCounter = spoons.Count;
            }
        }
    }

    [PunRPC]
    private void RemoveSpoonFromBasket(int spoonViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject spoon = PhotonView.Find(spoonViewID)?.gameObject;

        if (spoon != null)
        {
            // Убираем родителя у ложки
            spoon.transform.parent = null;

            // Удаляем ложку из списка локально
            spoons.Remove(spoon);
            spoonCounter = spoons.Count;
        }
    }
}
