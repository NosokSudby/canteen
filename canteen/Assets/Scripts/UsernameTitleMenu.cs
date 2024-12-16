using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameTitleMenu : MonoBehaviour
{
    public TMP_Text username;
    
    void Update()
    {
        username.text = PhotonNetwork.NickName;
    }
}
