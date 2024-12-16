using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BreadBox : MonoBehaviourPun
{
    public float breadCounter;
    public List<GameObject> breads = new List<GameObject>();

    [SerializeField] Transform targetParent;
    [SerializeField] Transform sourceParent;

    private void Update()
    {
        int sourceChildCount = sourceParent.childCount;

        // Удаляем неактивные или удаленные ложки из списка
        breads.RemoveAll(bread => bread == null || !bread.activeInHierarchy);
        breadCounter = breads.Count; // Обновляем счетчик ложек
        photonView.RPC("ActivateNonMatchingChildren", RpcTarget.All, targetParent.GetComponent<PhotonView>().ViewID, sourceChildCount);
        photonView.RPC("DisableMatchingChildren", RpcTarget.All, targetParent.GetComponent<PhotonView>().ViewID, sourceChildCount);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("bread") && !breads.Contains(other.gameObject))
        {

            // Синхронизируем добавление ложки у всех игроков
            photonView.RPC("AddBreadToBox", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("bread") && breads.Contains(other.gameObject))
        {

            // Синхронизируем удаление ложки у всех игроков
            photonView.RPC("RemoveBreadFromBox", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    private void AddBreadToBox(int breadViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject bread = PhotonView.Find(breadViewID)?.gameObject;
        bread.GetComponent<Rigidbody>().isKinematic = true;
        if (bread != null)
        {
            // Делаем корзину родителем ложки
            bread.transform.parent = transform.Find("normalBread").transform;

            // Обновляем список ложек локально
            if (!breads.Contains(bread))
            {
                
                breads.Add(bread);
                breadCounter = breads.Count;
            }
        }
    }

    [PunRPC]
    private void RemoveBreadFromBox(int breadViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject bread = PhotonView.Find(breadViewID)?.gameObject;

        if (bread != null)
        {
            // Убираем родителя у ложки
            bread.transform.parent = null;
            // Удаляем ложку из списка локально
            breads.Remove(bread);
            breadCounter = breads.Count;
        }
    }
    [PunRPC]
    private void ActivateNonMatchingChildren(int parentID, int countToMatch)
    {
        Transform parent = PhotonView.Find(parentID)?.transform;
        if(parent != null)
        {
            int childCount = parent.childCount;
            // Перебираем всех дочерних объектов у targetParent
            for (int i = 0; i < childCount; i++)
            {
                // Активируем объект, если индекс больше или равен countToMatch
                parent.GetChild(i).gameObject.SetActive(i >= countToMatch);
            }
        }

        
    }
    [PunRPC]
    private void DisableMatchingChildren(int parentID, int countToMatch)
    {
        Transform parent = PhotonView.Find(parentID)?.transform;
        if (parent != null)
        {
            int childCount = parent.childCount;

            // Перебираем всех дочерних объектов у targetParent
            for (int i = 0; i < childCount; i++)
            {
                // Деактивируем объект, если индекс меньше countToMatch
                parent.GetChild(i).gameObject.SetActive(i < countToMatch);
            }
        }
        
    }
}
