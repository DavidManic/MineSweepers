using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviourPunCallbacks, IOnEventCallback,IInRoomCallbacks
{
    [SerializeField]
    Board board;
    [SerializeField]
    StatusPanel statusPanel;

    public delegate void PlayerChanged(Player player);
    public delegate void MatchBegin();
    public delegate void MatchEnd(Dictionary<int, (Player, int)> leaderBoard);
    public MatchBegin OnMatchBegin;
    public MatchEnd OnMatchEnd;
    public PlayerChanged OnPlayerLeft;
    public PlayerChanged OnPlayerEnter;

    public bool IsGameActive { get; protected set; }

    public enum Event { GameStart = 1, GameEnd = 2, StartTurn = 10, EndTurn = 11, Move = 12, ToggleFlag = 13, ReceiveMove = 20, ReceiveToggleFlag = 21, CurrentPlayerChanged = 30, scoreSet = 31, scoreUpdate = 32 }
    // Start is called before the first frame update
    void Start()
    {
       // PhotonNetwork.AddCallbackTarget(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenTile(int y, int x)
    {

        Dictionary<byte, object> data = new Dictionary<byte, object>() { { 0, y }, { 1, x } };
        PhotonNetwork.RaiseEvent((byte)Event.Move, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("EVENT: " + (Event)photonEvent.Code+ " code: "+photonEvent.Code);
        Dictionary<byte, object> content = photonEvent.Parameters as Dictionary<byte, object>;
        Debug.Log(content.Count);
        switch ((Event)photonEvent.Code)
        {
            case Event.GameStart:
                OnGameStart(content);
                break;
            case Event.ReceiveMove:
                OnReciveMove(content);
                break;
            case Event.ReceiveToggleFlag:
                OnReceiveToggleFlag(content);
                break;
            case Event.scoreUpdate:
                OnScoreUpdate(content);
                break;
            case Event.scoreSet:
                onScoreSet(content);
                break;
            case Event.GameEnd:
                OnGameEnd(content);
                break;
            default:
                break;
        }

    }

    private void OnScoreUpdate(Dictionary<byte, object> content)
    {
        int key = (int)content[0];
        int val = (int)content[1];
        statusPanel.UpdateScore(key,val);
    }

    private void onScoreSet(Dictionary<byte, object> content)
    {
        Debug.Log(content[0]);
        Debug.Log(content[0].GetType());
        Dictionary<int, int> data = content[0] as Dictionary<int, int>;
        Debug.Log(data);
        Debug.Log(data.Keys);
        statusPanel.Setup(data);
    }

    private void OnGameEnd(Dictionary<byte, object> content)
    {
        Dictionary<int,(Player,int)> leaderBoard = new Dictionary<int, (Player, int)>();
        byte i = 0;
        int place = 1;
        while (i < content.Count)
        {
            leaderBoard.Add(place++,(PhotonNetwork.CurrentRoom.GetPlayer((int)content[i++]), (int)content[i++]));
        }
        IsGameActive = false;
        OnMatchEnd?.Invoke(leaderBoard);
        Debug.Log("Game have ended");
        Invoke("LeaveRoom", 3f);
    }

    internal void ToggleFlag(int y, int x)
    {

        Dictionary<byte, object> data = new Dictionary<byte, object>() { { 0, y }, { 1, x } };
        PhotonNetwork.RaiseEvent((byte)Event.ToggleFlag, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    private void OnGameStart(Dictionary<byte, object> content)
    {
        IsGameActive = true;
        OnMatchBegin?.Invoke();
        Debug.Log("OnGameStart");

        int hight = (int)content[0];
        int width = (int)content[1];

        board.MakeBoard(hight, width);
    }

    private void OnReciveMove(Dictionary<byte, object> content)
    {
        int y = (int)content[0];
        int x = (int)content[1];
        int val = (int)content[2];
        board.SetTile(y, x, val);
        if (val == 10)
            GetComponent<AudioSource>().Play();

        Debug.Log("Recive Open:" + y + " : " + x + " value: " + val);
        if (content.Count > 3)
            OpenRest(content);
    }

    private void OpenRest(Dictionary<byte, object> content)
    {
        byte i = 3;
        while (i < content.Count)
        {
            int y = (int)content[i++];
            int x = (int)content[i++];
            int val = (int)content[i++];
            board.SetTile(y, x, val);
        }
    }

    private void OnReceiveToggleFlag(Dictionary<byte, object> content)
    {
        int y = (int)content[0];
        int x = (int)content[1];
        bool val = (bool)content[2];

        board.ToggleFlag(y, x, val);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        OnPlayerEnter?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OnPlayerLeft?.Invoke(otherPlayer);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Launcher");
    }

}
