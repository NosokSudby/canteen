using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MetalTray : MonoBehaviourPun
{
    public float glassCounter;
    public List<GameObject> glasses = new List<GameObject>();
    [SerializeField] Transform targetParent;
    [SerializeField] Transform sourceParent;


    private void Update()
    {
        int sourceChildCount = sourceParent.childCount;
        photonView.RPC("ActivateNonMatchingChildren", RpcTarget.All, targetParent.GetComponent<PhotonView>().ViewID, sourceChildCount);
        photonView.RPC("DisableMatchingChildren", RpcTarget.All, targetParent.GetComponent<PhotonView>().ViewID, sourceChildCount);

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("glass"))
        {
            other.transform.SetParent(null);
            glasses.Add(other.gameObject);
            glassCounter = glasses.Count;

            // Синхронизируем удаление ложки у всех игроков
            //photonView.RPC("RemoveSpoonFromBasket", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("glass"))
        //{
        //    other.transform.SetParent(transform.Find("normalGlasses"));
        //    // Убираем ложку из списка локально
        //    glasses.Remove(other.gameObject);
        //    glassCounter = glasses.Count;
        //    // Синхронизируем добавление ложки у всех игроков
            //photonView.RPC("AddSpoonToBasket", RpcTarget.AllBuffered, other.gameObject.GetComponent<PhotonView>().ViewID);

        //}
    }
    [PunRPC]
    private void AddSpoonToBasket(int spoonViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject glass = PhotonView.Find(spoonViewID)?.gameObject;

        if (glass != null)
        {
            // Делаем корзину родителем ложки
            glass.transform.parent = transform;

            // Обновляем список ложек локально
            if (!glasses.Contains(glass))
            {
                glasses.Add(glass);
                glassCounter = glasses.Count;
            }
        }
    }

    [PunRPC]
    private void RemoveSpoonFromBasket(int spoonViewID)
    {
        // Получаем объект ложки по ViewID
        GameObject glass = PhotonView.Find(spoonViewID)?.gameObject;

        if (glass != null)
        {
            // Убираем родителя у ложки
            glass.transform.parent = null;

            // Удаляем ложку из списка локально
            glasses.Remove(glass);
            glassCounter = glasses.Count;
        }
    }
    [PunRPC]
    private void ActivateNonMatchingChildren(int parentID, int countToMatch)
    {
        Transform parent = PhotonView.Find(parentID)?.transform;
        if (parent != null)
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
