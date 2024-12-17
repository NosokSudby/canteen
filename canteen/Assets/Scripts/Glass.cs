using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Glass : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("table"))
        {
            if (transform.parent == null)
            {
                photonView.RPC("Setp", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, other.gameObject.GetComponent<PhotonView>().ViewID);
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("table"))
        {
            if (transform.parent != null)
            {
                photonView.RPC("SetF", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, other.gameObject.GetComponent<PhotonView>().ViewID);
            }

        }
    }
    [PunRPC]
    void Setp(int objectID, int otherID)
    {
        Transform objectToSet = PhotonView.Find(objectID)?.transform;
        GameObject other = PhotonView.Find(otherID)?.gameObject;
        objectToSet.SetParent(other.transform.parent);
    }
    [PunRPC]
    void SetF(int objectID, int otherID)
    {
        Transform objectToSet = PhotonView.Find(objectID)?.transform;
        GameObject other = PhotonView.Find(otherID)?.gameObject;
        objectToSet.SetParent(null);
    }
}
