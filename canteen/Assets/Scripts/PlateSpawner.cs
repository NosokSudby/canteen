using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlateSpawner : MonoBehaviourPunCallbacks
{
    public GameObject objectToSpawn; // Префаб для спавна
    public Transform[] spawnPositions; // Массив позиций для спавна
    public float minSpawnDelay = 1f; // Минимальная задержка
    public float maxSpawnDelay = 3f; // Максимальная задержка
    public int maxObjects = 10; // Максимальное количество объектов

    public List<GameObject> spawnedObjects = new List<GameObject>(); // Список текущих объектов
    private HashSet<int> occupiedPositions = new HashSet<int>(); // Набор занятых позиций

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (spawnedObjects.Count < maxObjects)
            {
                SpawnObject();
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnObject()
    {
        if (spawnPositions.Length == 0)
        {
            Debug.LogWarning("Нет позиций для спавна!");
            return;
        }

        // Выбираем доступную позицию
        List<int> availablePositions = new List<int>();
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (!occupiedPositions.Contains(i))
            {
                availablePositions.Add(i);
            }
        }

        if (availablePositions.Count == 0)
        {
            Debug.LogWarning("Все позиции заняты, спавн невозможен!");
            return;
        }

        int randomIndex = availablePositions[Random.Range(0, availablePositions.Count)];
        Transform spawnPosition = spawnPositions[randomIndex];

        // Создаем объект через Photon
        GameObject newObject = PhotonNetwork.Instantiate(objectToSpawn.name, spawnPosition.position, spawnPosition.rotation);

        // Добавляем в список
        spawnedObjects.Add(newObject);
        occupiedPositions.Add(randomIndex);

        // Привязываем освобождение позиции к PlateSc
        PlateSc plateSc = newObject.GetComponent<PlateSc>();
        if (plateSc != null)
        {
            plateSc.OnPicked += () =>
            {
                if (spawnedObjects.Contains(newObject))
                {
                    spawnedObjects.Remove(newObject);
                    occupiedPositions.Remove(randomIndex);
                }
            };
        }
    }
}
