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

    bool gameHasEnded = false;

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

    private void OnMatchEnd(List<(int, Player, int)> leaderBoard)
    {
        status.text = "Game won by: " + (leaderBoard[0].Item2.NickName.Length > 0 ? leaderBoard[0].Item2.NickName : "User_" + leaderBoard[0].Item2.ActorNumber);
        gameHasEnded = true;
        foreach (Text t in texts.Values)
            Destroy(t.gameObject);
        texts.Clear();

        gameObject.SetActive(true);
        foreach((int, Player, int) x in leaderBoard)
        {
            Player player = x.Item2;
            texts.Add(player.ActorNumber, Instantiate(playerText, playersPanel).GetComponent<Text>());
            texts[player.ActorNumber].text = (player.NickName.Length > 0 ? player.NickName : "User_" + player.ActorNumber) +" "+ x.Item3;
            
        }

    }

    private void OnMatchBegin()
    {
        gameObject.SetActive(false);
        status.text = "Match in progress";
    }

    private void OnPlayerLeft(Player player)
    {
        if (!gameHasEnded)
        {
            Destroy(texts[player.ActorNumber].gameObject);
            texts.Remove(player.ActorNumber);
        }
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
