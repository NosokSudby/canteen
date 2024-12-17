using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Glass : MonoBehaviourPun
{
    public bool setted = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("table"))
        {
            if (transform.parent == null)
            {
                photonView.RPC("Setp", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID, other.gameObject.GetComponent<PhotonView>().ViewID);
                setted = true;
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(setted == true)
        {
            if (other.gameObject.CompareTag("table"))
            {
                photonView.RPC("SetF", RpcTarget.All, transform.GetComponent<PhotonView>().ViewID);
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
    void SetF(int objectID)
    {
        Transform objectToSet = PhotonView.Find(objectID)?.transform;
        objectToSet.SetParent(null);
    }
}
