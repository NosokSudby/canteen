using UnityEngine;

public class PlaceAndReplaceObject : MonoBehaviour
{
    public GameObject objectToPlace; // Объект, который будет появляться при зажатии клавиши E
    public GameObject objectToReplace; // Объект, который будет появляться при отпускании клавиши E
    public float maxDistance = 10f; // Максимальная дистанция, на которой игрок может взаимодействовать с объектом

    private GameObject placedObject; // Ссылка на последний размещённый объект

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            TryPlaceObject();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            TryReplaceObject();
        }
    }

    void TryPlaceObject()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (placedObject == null)
            {
                placedObject = Instantiate(objectToPlace, hit.point, Quaternion.identity);
            }
        }
    }

    void TryReplaceObject()
    {
        if (placedObject != null)
        {
            Vector3 position = placedObject.transform.position;
            Destroy(placedObject);
            placedObject = Instantiate(objectToReplace, position, Quaternion.identity);
        }
    }
}
