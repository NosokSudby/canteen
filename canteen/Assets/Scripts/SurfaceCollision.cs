using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("spoon"))
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("spoon"))
        {
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
