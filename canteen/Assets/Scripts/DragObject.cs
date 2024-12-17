using UnityEngine;
using Photon.Pun;

public class DragObject : MonoBehaviourPun, IPunObservable
{
    private Camera mainCamera; // Камера для работы с координатами
    private Vector3 offset;    // Смещение между объектом и мышью
    private float zCoord;      // Z-координата объекта для глубины

    private Rigidbody rb;

    [SerializeField]
    private float weight = 1f; // Вес объекта, чем больше, тем труднее двигать

    [SerializeField]
    private float dragSpeed = 10f; // Скорость "тянущего" эффекта

    [SerializeField]
    private float scrollSpeed = 2f; // Скорость изменения расстояния по колесику

    [SerializeField]
    private float rotationSpeed = 100f; // Скорость вращения объекта

    private float distanceFromPlayer; // Расстояние от объекта до камеры

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private bool isDragging = false; // Флаг, указывающий, что объект перетаскивается

    public string SurfaceTag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Синхронизация позиции и вращения объекта для других игроков
            rb.position = Vector3.Lerp(rb.position, networkPosition, Time.deltaTime * 10);
            rb.rotation = Quaternion.Lerp(rb.rotation, networkRotation, Time.deltaTime * 10);
        }
        else
        {
            if(isDragging == true)
            {
                // Управление вращением объекта
                HandleRotationInput();
            }
        }
    }

    void OnMouseDown()
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        // Получаем PhotonView владельца объекта
        if (photonView.IsMine)
        {
            GameObject localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            if (localPlayer != null)
            {
                PlayerController localPlayerast = localPlayer.GetComponent<PlayerController>();
                localPlayerast.canMove = false;
            }
            
        }
        if (CompareTag("glass"))
        {
            photonView.RPC("SP", RpcTarget.All);
        }
        // Сохраняем текущую дистанцию до объекта
        distanceFromPlayer = Vector3.Distance(Camera.main.transform.position, transform.position);

        // Сохраняем смещение между объектом и курсором
        offset = transform.position - GetMouseWorldPosAtDistance();

        photonView.RPC("KinematicOn", RpcTarget.All);
        isDragging = true;
    }

    [PunRPC]
    void SP()
    {
        transform.SetParent(null);
    }
    [PunRPC]
    void KinematicOn()
    {
        rb.isKinematic = true;
    }

    private void OnMouseUp()
    {
        rb.isKinematic = false;
        isDragging = false; // Сбрасываем флаг перетаскиванияif (photonView.IsMine)
        {
            GameObject localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            if (localPlayer != null)
            {
                PlayerController localPlayerast = localPlayer.GetComponent<PlayerController>();
                localPlayerast.canMove = true;
            }

        }
    }

    void OnMouseDrag()
    {
        if (!photonView.IsMine) return;

        // Обновляем позицию объекта, учитывая дистанцию и положение мыши
        UpdateObjectPosition();

        // Обрабатываем прокрутку колесика мыши
        HandleScrollWheelInput();
    }

    private void UpdateObjectPosition()
    {
        // Получаем мировую позицию мыши, с учетом текущей дистанции
        Vector3 mouseWorldPosition = GetMouseWorldPosAtDistance() + offset;

        // Сглаживаем движение объекта
        transform.position = Vector3.Lerp(transform.position, mouseWorldPosition, Time.deltaTime * dragSpeed / weight);
    }

    private void HandleScrollWheelInput()
    {
        if (!isDragging) return; // Обработка только при перетаскивании

        // Получаем ввод от колесика мыши
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollDelta) > 0.01f) // Учитываем только значимые изменения
        {
            distanceFromPlayer += scrollDelta * scrollSpeed; // Изменяем дистанцию
            distanceFromPlayer = Mathf.Clamp(distanceFromPlayer, 0.1f, 20f); // Ограничиваем диапазон
        }
    }

    private void HandleRotationInput()
    {
        // Вращение влево (A) и вправо (D)
        if (Input.GetKey(KeyCode.A))
        {
            RotateObject(-rotationSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotateObject(rotationSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            RotateObjectDifferent(-rotationSpeed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            RotateObjectDifferent(rotationSpeed);
        }
    }

    private void RotateObject(float rotationDelta)
    {
        transform.Rotate(Vector3.back, rotationDelta * Time.deltaTime);
    }
    private void RotateObjectDifferent(float rotationDelta)
    {
        transform.Rotate(Vector3.left, rotationDelta * Time.deltaTime);
    }

    private Vector3 GetMouseWorldPosAtDistance()
    {
        // Получаем позицию мыши на экране
        Vector3 mousePoint = Input.mousePosition;

        // Устанавливаем расстояние до объекта
        mousePoint.z = distanceFromPlayer;

        // Конвертируем координаты в мировое пространство
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Отправляем данные текущему владельцу
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // Получаем данные от других игроков
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
