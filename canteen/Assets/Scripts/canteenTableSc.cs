using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using UnityEngine;

public class canteenTableSc : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool firstBenchDown = false;
    public bool secondBenchDown = false;

    public Transform bench1;
    public Transform bench2;

    public TMP_Text paperNumber;

    public float platesCount;
    public float glassesCount;

    public float childrenCount;

    public List<GameObject> plates = new List<GameObject>();
    public List<GameObject> glasses = new List<GameObject>();

    private void Start()
    {
        bench1 = this.transform.Find("firstBench");
        bench2 = this.transform.Find("secBench");

        if (photonView.IsMine)
        {
            SetPaperNumber(); // Только владелец объекта генерирует число
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            platesCount = plates.Count;
        }
    }

    private void OnTransformChildrenChanged()
    {
        if (!photonView.IsMine) return;

        // Обновляем списки объектов
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("plate") && !plates.Contains(child.gameObject))
            {
                plates.Add(child.gameObject);
                Debug.Log($"Добавлен объект: {child.gameObject.name}");
            }

            if (child.gameObject.CompareTag("glass") && !glasses.Contains(child.gameObject))
            {
                glasses.Add(child.gameObject);
                Debug.Log($"Добавлен объект: {child.gameObject.name}");
            }
        }

        plates.RemoveAll(plate => plate == null || plate.transform.parent != transform);
        glasses.RemoveAll(glass => glass == null || glass.transform.parent != transform);

        // Синхронизируем изменения для других игроков
        photonView.RPC("SyncChildObjects", RpcTarget.Others, plates.Count, glasses.Count);
    }

    [PunRPC]
    void SyncChildObjects(int newPlatesCount, int newGlassesCount)
    {
        platesCount = newPlatesCount;
        glassesCount = newGlassesCount;
    }

    void SetPaperNumber()
    {
        float randomFloat = Random.Range(6, 7);
        childrenCount = randomFloat;
        paperNumber.text = randomFloat.ToString();

        // Синхронизируем случайное число с другими игроками
        photonView.RPC("SyncPaperNumber", RpcTarget.Others, randomFloat);
    }

    [PunRPC]
    void SyncPaperNumber(float randomFloat)
    {
        childrenCount = randomFloat;
        paperNumber.text = randomFloat.ToString();
    }

    // Метод для синхронизации состояний через Photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Отправляем данные другим игрокам
            stream.SendNext(firstBenchDown);
            stream.SendNext(secondBenchDown);
            stream.SendNext(childrenCount);
        }
        else
        {
            // Получаем данные от других игроков
            firstBenchDown = (bool)stream.ReceiveNext();
            secondBenchDown = (bool)stream.ReceiveNext();
            childrenCount = (float)stream.ReceiveNext();
        }
    }
}
