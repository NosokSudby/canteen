using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    public void OnUsernameInputValueChange()
    {
        if(usernameInput != null)
        {
            PhotonNetwork.NickName = usernameInput.text;
            PlayerPrefs.SetString("username", usernameInput.text); 
        }
    }
    private void Update()
    {
        if(usernameInput.text == "")
        {
            PhotonNetwork.NickName = "Дежурный " + Random.Range(0, 1000).ToString("0000");
        }
    }
}
