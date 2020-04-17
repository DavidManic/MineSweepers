using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerEntry : MonoBehaviour
{
    /// <summary>
    /// Room information about this server.
    /// </summary>
    private RoomInfo roomInfo;
    /// <summary>
    /// Game info about this game. used when player is joining to same game. but with different scene.
    /// </summary>

    public GameObject Setup(RoomInfo roomInfo, Transform parent)
    {
        this.roomInfo = roomInfo;
        transform.GetChild(0).GetComponent<Text>().text = roomInfo.Name;
        transform.GetChild(1).GetComponent<Text>().text = roomInfo.CustomProperties["plugin"].ToString();
        transform.GetChild(2).GetComponent<Text>().text = roomInfo.CustomProperties["hight"].ToString();
        transform.GetChild(3).GetComponent<Text>().text = roomInfo.CustomProperties["width"].ToString();
        transform.GetChild(4).GetComponent<Text>().text = ((int)roomInfo.CustomProperties["mineRate"]/100).ToString();
        transform.SetParent(parent);
        return gameObject;
    }

    public void Join()
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);

    }
}
