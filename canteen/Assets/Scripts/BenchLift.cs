using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchLift : MonoBehaviourPun, IPunObservable
{
    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 dragStartPoint;
    private Transform hingePoint;

    [SerializeField]
    private Transform leftSide;   // Левая сторона скамейки
    [SerializeField]
    private Transform rightSide;  // Правая сторона скамейки
    [SerializeField]
    private float liftForce = 10f;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public bool lifted = false;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Скамейка должна содержать Rigidbody!");
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Клиенты только получают данные, но не управляют физикой
            rb.isKinematic = true; // Отключает физику для клиента
            rb.position = Vector3.Lerp(rb.position, networkPosition, Time.deltaTime * 10);
            rb.rotation = Quaternion.Lerp(rb.rotation, networkRotation, Time.deltaTime * 10);
        }
        else
        {
            rb.isKinematic = false; // Физика активна только у владельца
        }
    }


    void OnMouseDown()
    {
        Debug.Log($"OnMouseDown: photonView.IsMine = {photonView.IsMine}");
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
            if (!photonView.IsMine)
            {
                Debug.LogWarning("Владение не было передано!");
                photonView.RequestOwnership();
                return;
            }
            Debug.Log($"Владение после запроса: photonView.IsMine = {photonView.IsMine}");
        }

        // Логика для определения ближайшей стороны
        Vector3 mouseWorldPos = GetMouseWorldPos();
        dragStartPoint = mouseWorldPos;

        float leftDistance = Vector3.Distance(mouseWorldPos, leftSide.position);
        float rightDistance = Vector3.Distance(mouseWorldPos, rightSide.position);

        hingePoint = leftDistance < rightDistance ? leftSide : rightSide;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!photonView.IsMine || !isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPos();
        Vector3 liftDirection = (mouseWorldPos - dragStartPoint).normalized;

        // Применяем силу к выбранной стороне
        rb.AddForceAtPosition(liftDirection * liftForce, hingePoint.position, ForceMode.Force);

        // Обновляем стартовую точку для плавного движения
        dragStartPoint = mouseWorldPos;

        // Синхронизация действий с другими игроками
        photonView.RPC("SyncHingePoint", RpcTarget.Others, hingePoint.position);
    }

    void OnMouseUp()
    {
        if (!photonView.IsMine) return;

        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    [PunRPC]
    private void SyncHingePoint(Vector3 hingePosition)
    {
        if (hingePoint != null)
        {
            hingePoint.position = hingePosition;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Отправляем текущие данные
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
    private void OnCollisionExit(Collision other)
    {
        
        if (other.gameObject.CompareTag("floor"))
        {
            lifted = true;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            lifted = false;
        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            lifted = false;
        }
    }
    private void fOnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("table") && other.transform == this.transform.parent.Find("table"))
        {
            Debug.Log("sad");
            this.transform.localPosition = new Vector3(0.00661411509f, 0.179000005f, -0.335999995f);
            Vector3 rotate = transform.eulerAngles;
            rotate.y = 90f;
            transform.rotation = Quaternion.Euler(rotate);
        }
    }
    // надо сделать так, шобы по возвращению камеры, она была в нормальной позиции
}
