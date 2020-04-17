using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject CreatePanel;
    [SerializeField]
    GameObject JoinPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnCreateOpen()
    {
        CreatePanel.SetActive(true);
    }

    public void OnJoinOpen()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
