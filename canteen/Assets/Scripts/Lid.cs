using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lid : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other)
    {
        if(other.transform.name == "bread box")
        {
            photonView.RPC("AddLidToBox", RpcTarget.All, this.GetComponent<PhotonView>().ViewID, other.GetComponent<PhotonView>().ViewID);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "bread box")
        {
            photonView.RPC("RemoveLidFromBox", RpcTarget.All, this.GetComponent<PhotonView>().ViewID, other.GetComponent<PhotonView>().ViewID);
        }
    }
    [PunRPC]
    void AddLidToBox(int lidID, int boxID)
    {
        GameObject lid = PhotonView.Find(lidID)?.gameObject;
        GameObject box = PhotonView.Find(boxID)?.gameObject;
        if (lid != null && box != null)
        {
            lid.transform.parent = box.transform;
        }
    }
    [PunRPC]
    void RemoveLidFromBox(int lidID, int boxID)
    {
        GameObject lid = PhotonView.Find(lidID)?.gameObject;
        if (lid != null)
        {
            lid.transform.parent = null;
        }
    }

}
