using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    NetworkController networkController;

    [SerializeField]
    Text status;
    [SerializeField]
    Transform playersPanel;
    [SerializeField]
    GameObject playerText;

    Dictionary<int, Text> texts = new Dictionary<int, Text>();

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        networkController.OnPlayerEnter += OnPlayerEnter;
        networkController.OnPlayerLeft += OnPlayerLeft;
        networkController.OnMatchBegin += OnMatchBegin;
        networkController.OnMatchEnd += OnMatchEnd;
    }

    private void OnMatchEnd(Dictionary<int, (Player player, int score)> leaderBoard)
    {
        foreach (Text t in texts.Values)
            Destroy(t.gameObject);
        texts.Clear();

        gameObject.SetActive(true);
        foreach(int place in leaderBoard.Keys)
        {
            Player player = leaderBoard[place].player;
            texts.Add(player.ActorNumber, Instantiate(playerText, playersPanel).GetComponent<Text>());
            texts[player.ActorNumber].text = player.NickName.Length > 0 ? player.NickName : "User_" + player.ActorNumber;
            
        }

    }

    private void OnMatchBegin()
    {
        gameObject.SetActive(false);
    }

    private void OnPlayerLeft(Player player)
    {
        Destroy(texts[player.ActorNumber].gameObject);
        texts.Remove(player.ActorNumber);
    }

    private void OnPlayerEnter(Player player)
    {
        texts.Add(player.ActorNumber, Instantiate(playerText, playersPanel).GetComponent<Text>());
        texts[player.ActorNumber].text = player.NickName.Length > 0 ? player.NickName : "User_" + player.ActorNumber;
    }

    void Setup()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            texts.Add(player.ActorNumber,Instantiate(playerText, playersPanel).GetComponent<Text>());
            texts[player.ActorNumber].text = player.NickName.Length > 0 ? player.NickName : "User_" + player.ActorNumber;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
