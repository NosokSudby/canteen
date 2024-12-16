using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    RoomManager rm;
    float verticalLookRotation;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    Rigidbody rb;
    public Material[] mats;
    private Transform currentTarget;
    public canteenTableSc cts;

    public bool benching = false;

    Renderer objectRenderer;
    PhotonView PV;

    public Transform handTransform; // Позиция руки игрока для удержания предмета
    public Transform hand2Transform;
    public Transform handTransformToSync;
    public Transform hand2TransformToSync;
    public float pickUpRange = 3f; // Максимальная дистанция, на которой игрок может поднять предмет
    public LayerMask interactableLayer; // Слой объектов, которые можно поднимать

    private GameObject heldObject; // Ссылка на поднятый объект
    private GameObject held2Object; // Ссылка на второй поднятый объект

    private GameObject placedObject; // Ссылка на последний размещённый объект
    private GameObject previewObject; // Превью объекта перед размещением
    public LayerMask placeableLayer;
    float OGmouseSens;
    float OGwalkSpeed;
    float OGsprintSpeed;
    float OGjumpForce;

    public bool canMove = true;
    void Awake()
    {
        rm = FindAnyObjectByType<RoomManager>();
        objectRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Start()
    {
        if (PV.IsMine)
        {
            PV.RPC("SetMaterial", RpcTarget.AllBuffered, rm.color);
            OGmouseSens = mouseSensitivity;
            OGwalkSpeed = walkSpeed;
            OGsprintSpeed = sprintSpeed;
            OGjumpForce = jumpForce;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);

        }
    }
    public bool cursorLocked = true;
    private void Update()
    {
        if (!PV.IsMine)
            return;
        if (benching == false)
        {
            Look();
        }
        if(canMove == true)
        {
            Move();
            Jump();

        }
        Bench();
        if (benching == true)
            SwitchBenchCameras();
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (cursorLocked == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                mouseSensitivity = OGmouseSens;
                Camera.main.fieldOfView = 60f;
                cursorLocked = true;
                canMove = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (cursorLocked == true)
            {
                OGmouseSens = mouseSensitivity;
                Cursor.lockState = CursorLockMode.None;
                mouseSensitivity = 0;
                Camera.main.fieldOfView = 40f;
                cursorLocked = false;
                canMove= false;

            }
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (cursorLocked == true)
            {
                OGmouseSens = mouseSensitivity;
                Cursor.lockState = CursorLockMode.None;
                mouseSensitivity = 0;
                Camera.main.fieldOfView = 20f;
                cursorLocked = false;
                canMove= false;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                if(held2Object == null)
                {
                    TryPlaceObject();
                }
            }
            if (held2Object == null)
            {
                if(heldObject != null && heldObject.CompareTag("plate"))
                {
                    TryPickUpSecondObject();
                }
            }
            else
            {
                TryPlaceSecondObject();
            }
        }
        handTransformToSync.position = handTransform.position;
        hand2TransformToSync.position = hand2Transform.position;

        handTransformToSync.rotation = handTransform.rotation;
        hand2TransformToSync.rotation = hand2Transform.rotation;
    }
    private void TryPickUpObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange, interactableLayer))
        {
            PhotonView targetPV = hit.collider.GetComponent<PhotonView>();
            if (targetPV != null)
            {
                // Запрашиваем владение объектом перед поднятием
                targetPV.RequestOwnership();
                PV.RPC("PickUpObject", RpcTarget.All, targetPV.ViewID);
            }
        }
    }
    private void TryPickUpSecondObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, pickUpRange, interactableLayer))
        {
            if (hit.collider.CompareTag("plate"))
            {
                PhotonView targetPV = hit.collider.GetComponent<PhotonView>();
                if (targetPV != null)
                {
                    // Запрашиваем владение объектом перед поднятием
                    targetPV.RequestOwnership();
                    PV.RPC("PickUpSecondObject", RpcTarget.All, targetPV.ViewID);
                }
            }
        }
    }

    float OGfloat1;

    Vector3 OGsize;

    [PunRPC]
    private void PickUpObject(int viewID)
    {
        GameObject targetObject = PhotonView.Find(viewID).gameObject;
        heldObject = targetObject;
        OGsize = heldObject.transform.localScale;
        Rigidbody objectRb = heldObject.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
            objectRb.isKinematic = true;
            objectRb.constraints = RigidbodyConstraints.FreezeAll;
        }
        if (heldObject.CompareTag("drainer"))
        {
            GameObject spoonsToActivate = heldObject.transform.Find("pickedSpoons").gameObject;
            GameObject spoonsToDisable = heldObject.transform.Find("normalSpoons").gameObject;
            if (spoonsToActivate != null && spoonsToDisable != null)
            {
                spoonsToActivate.SetActive(true);
                spoonsToDisable.SetActive(false);
            }
        }
        if(heldObject.transform.name == "bread box")
        {
            GameObject breadsToActivate = heldObject.transform.Find("pickedBread").gameObject;
            GameObject breadsToDisable = heldObject.transform.Find("normalBread").gameObject;
            if (breadsToActivate != null && breadsToDisable != null)
            {
                breadsToActivate.SetActive(true);
                breadsToDisable.SetActive(false);
            }
            if (heldObject.transform.childCount != 0)
            {
                Transform lid = heldObject.transform.Find("lid");
                if (lid != null)
                {
                    lid.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            
        }

        if (PV.IsMine)
        {
            heldObject.transform.SetParent(handTransform);
        }
        else
        {
            heldObject.transform.SetParent(handTransformToSync);
        }
        heldObject.transform.localPosition = Vector3.zero;
        if (!heldObject.CompareTag("plate"))
        {
            heldObject.transform.localRotation = Quaternion.identity;
        }

        Collider[] colliders = heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }
    [PunRPC]
    private void PickUpSecondObject(int viewID)
    {
        GameObject targetObject = PhotonView.Find(viewID).gameObject;
        held2Object = targetObject;
        OGsize = held2Object.transform.localScale;
        Rigidbody objectRb = held2Object.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
            objectRb.isKinematic = true;
            objectRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (PV.IsMine)
        {
            held2Object.transform.SetParent(hand2Transform);
        }
        else
        {
            held2Object.transform.SetParent(hand2TransformToSync);
        }
        held2Object.transform.localPosition = Vector3.zero;
        
        Collider[] colliders = held2Object.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void TryPlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 6f, placeableLayer))
        {
            if(heldObject.transform.name == "bread box")
            {
                Vector3 d = new Vector3(hit.point.x, hit.point.y + 0.029669f, hit.point.z);
                PV.RPC("PlaceObject", RpcTarget.All, d);
            }
            else
            {
                PV.RPC("PlaceObject", RpcTarget.All, hit.point);
            }
        }
    }
    void TryPlaceSecondObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 6f, placeableLayer))
        {
            
            PV.RPC("PlaceSecondObject", RpcTarget.All, hit.point);
            
        }
    }


    [PunRPC]
    private void PlaceObject(Vector3 position)
    {
        if (heldObject == null) return;

        Rigidbody objectRb = heldObject.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
        }
        if (heldObject.CompareTag("drainer"))
        {
            GameObject spoonsToActivate = heldObject.transform.Find("normalSpoons").gameObject;

            GameObject spoonsToDisable = heldObject.transform.Find("pickedSpoons").gameObject;

            if (spoonsToActivate != null && spoonsToDisable != null)
            {
                spoonsToActivate.SetActive(true);
                spoonsToDisable.SetActive(false);
            }
        }
        if (heldObject.transform.name == "bread box")
        {
            GameObject breadsToActivate = heldObject.transform.Find("pickedBread").gameObject;
            GameObject breadsToDisable = heldObject.transform.Find("normalBread").gameObject;
            if (breadsToActivate != null && breadsToDisable != null)
            {
                breadsToActivate.SetActive(false);
                breadsToDisable.SetActive(true);
            }
            if (heldObject.transform.childCount != 0)
            {
                Transform lid = heldObject.transform.Find("lid");
                if (lid != null)
                {
                    lid.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
                
        }


        Vector3 rotate = transform.eulerAngles;
        if (!heldObject.CompareTag("plate"))
        {
            rotate.x = -90;
            heldObject.transform.rotation = Quaternion.Euler(rotate);
        }
        else
        {
            rotate.x = 0;
            heldObject.transform.rotation = Quaternion.Euler(rotate);
        }
        heldObject.transform.SetParent(null);
        heldObject.transform.position = position;

        Collider[] colliders = heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        Rigidbody[] components = heldObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody component in components)
        {
            component.constraints = RigidbodyConstraints.None;
        }
        heldObject.transform.localScale = OGsize;
        
        heldObject = null;
    }
    [PunRPC]
    private void PlaceSecondObject(Vector3 position)
    {
        if (held2Object == null) return;

        Rigidbody objectRb = held2Object.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
        }
        Vector3 rotate = transform.eulerAngles;
        rotate.x = 0;
        held2Object.transform.rotation = Quaternion.Euler(rotate);

        held2Object.transform.SetParent(null);
        held2Object.transform.position = position;

        Collider[] colliders = held2Object.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        Rigidbody[] components = held2Object.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody component in components)
        {
            component.constraints = RigidbodyConstraints.None;
        }
        held2Object.transform.localScale = OGsize;
        held2Object = null;
    }

    void SetPreviewAppearance(GameObject obj, bool isPreview)
    {
        foreach (var renderer in obj.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material.color = isPreview ? new Color(1, 1, 1, 0.5f) : Color.white;
        }

        foreach (var collider in obj.GetComponentsInChildren<Collider>())
        {
            collider.enabled = !isPreview;
        }
    }
    void DropItem()
    {
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; // Включаем физику обратно
            heldObject.transform.parent = null; // Убираем из дочерних объектов руки
            heldObject = null;
        }
    }

    [PunRPC]
    void SetMaterial(int col)
    {
        objectRenderer.material = mats[col];
    }

    public bool leftBenchCam = false;
    public bool rightBenchCam = false;

    Transform BenchCamLeft;
    Transform BenchCamRight;
    public BenchLift blToDisable;

    void Bench()
    {
        
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 2, LayerMask.GetMask("Ignore Surfaces")))
        {
            if (hit.collider.CompareTag("sec bench") && !benching)
            {
                BenchLift bl = hit.collider.GetComponent<BenchLift>();
                blToDisable = bl;

                if (Input.GetKey(KeyCode.F))
                {
                    ActivateBench(bl);
                }
            }
        }

        if (benching && Input.GetKey(KeyCode.Space))
        {
            DeactivateBench();
        }
    }

    void ActivateBench(BenchLift bl)
    {
        bl.enabled = true;

        // Сохраняем оригинальные значения, если необходимо
        jumpForce = 0;
        mouseSensitivity = 0;
        walkSpeed = 0;
        sprintSpeed = 0;

        Transform leftCamPos = bl.transform.Find("left cam pos");
        Camera.main.transform.position = leftCamPos.position;
        Camera.main.transform.rotation = leftCamPos.rotation;

        Cursor.lockState = CursorLockMode.None;
        leftBenchCam = true;
        benching = true;

        BenchCamLeft = leftCamPos;
        BenchCamRight = bl.transform.Find("right cam pos");
    }

    void DeactivateBench()
    {
        blToDisable.enabled = false;
        benching = false;

        Transform ogCamPos = cameraHolder.transform.Find("OGcamPos");
        Camera.main.transform.position = ogCamPos.position;
        Camera.main.transform.rotation = ogCamPos.rotation;

        Cursor.lockState = CursorLockMode.Locked;

        mouseSensitivity = OGmouseSens;
        walkSpeed = OGwalkSpeed;
        sprintSpeed = OGsprintSpeed;
        jumpForce = OGjumpForce;
    }

    void SwitchBenchCameras()
    {
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            ToggleCamera();
        }
    }

    void ToggleCamera()
    {
        if (leftBenchCam)
        {
            Camera.main.transform.position = BenchCamRight.position;
            Camera.main.transform.rotation = BenchCamRight.rotation;
            leftBenchCam = false;
            rightBenchCam = true;
        }
        else if (rightBenchCam)
        {
            Camera.main.transform.position = BenchCamLeft.position;
            Camera.main.transform.rotation = BenchCamLeft.rotation;
            leftBenchCam = true;
            rightBenchCam = false;
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.Find("Camera").localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }
    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        if (canMove)
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
        
    }
}