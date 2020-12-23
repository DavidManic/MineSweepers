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
    public delegate void MatchEnd(List<(int, Player, int)> leaderBoard);
    public MatchBegin OnMatchBegin;
    public MatchEnd OnMatchEnd;
    public PlayerChanged OnPlayerLeft;
    public PlayerChanged OnPlayerEnter;

    public bool IsGameActive { get; protected set; }

    public enum Event { GameStart = 1, GameEnd = 2, StartTurn = 10, EndTurn = 11, Move = 12, ToggleFlag = 13, ReceiveMove = 20, ReceiveToggleFlag = 21, CurrentPlayerChanged = 30, scoreSet = 31, scoreUpdate = 32, SyncFields = 90, SyncFlags = 91 }
    
    /// <summary>
    /// Open tile
    /// </summary>
    /// <param name="y">colum</param>
    /// <param name="x">row</param>
    public void OpenTile(int y, int x)
    {

        Dictionary<byte, object> data = new Dictionary<byte, object>() { { 0, y }, { 1, x } };
        PhotonNetwork.RaiseEvent((byte)Event.Move, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }
    /// <summary>
    /// Toggle flag
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    public void ToggleFlag(int y, int x)
    {

        Dictionary<byte, object> data = new Dictionary<byte, object>() { { 0, y }, { 1, x } };
        PhotonNetwork.RaiseEvent((byte)Event.ToggleFlag, data, RaiseEventOptions.Default, SendOptions.SendReliable);
    }
    /// <summary>
    /// Recieve events from server
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("EVENT: " + (Event)photonEvent.Code+ " code: "+photonEvent.Code);
        Dictionary<byte, object> content = photonEvent.Parameters as Dictionary<byte, object>;
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
                OnScoreSet(content);
                break;
            case Event.GameEnd:
                OnGameEnd(content);
                break;
            case Event.SyncFields:
                OnSyncFields(content);
                break;
            case Event.SyncFlags:
                OnSyncFlags(content);
                break;
            default:
                break;
        }

    }
    /// <summary>
    /// Sync flags from given content
    /// </summary>
    /// <param name="content"></param>
    private void OnSyncFlags(Dictionary<byte, object> content)
    {
        byte i = 0;
        while (i < content.Count)
        {
            int y = (int)content[i++];
            int x = (int)content[i++];
            board.ToggleFlag(y, x, true);
        }
    }
    /// <summary>
    /// Sync fields from recieved content
    /// </summary>
    /// <param name="content"></param>
    private void OnSyncFields(Dictionary<byte, object> content)
    {
        byte i = 0;
        while (i < content.Count)
        {
            int y = (int)content[i++];
            int x = (int)content[i++];
            int val = (int)content[i++];
            board.SetTile(y, x, val);
        }
    }

    /// <summary>
    /// Update the score
    /// </summary>
    /// <param name="content"></param>
    private void OnScoreUpdate(Dictionary<byte, object> content)
    {
        int key = (int)content[0];
        int val = (int)content[1];
        statusPanel.UpdateScore(key,val);
    }

    /// <summary>
    /// Set scores from recived data
    /// </summary>
    /// <param name="content"></param>
    private void OnScoreSet(Dictionary<byte, object> content)
    {
        Dictionary<int, int> data = content[0] as Dictionary<int, int>;
        statusPanel.Setup(data);
    }

    /// <summary>
    /// Called on end of the game
    /// </summary>
    /// <param name="content"></param>
    private void OnGameEnd(Dictionary<byte, object> content)
    {
        List<(int,Player,int)> leaderBoard = new List<(int, Player, int)>();
        byte i = 0;
        int place = 1;
        while (i < content.Count)
        {
            leaderBoard.Add((place++,PhotonNetwork.CurrentRoom.GetPlayer((int)content[i++]), (int)content[i++]));
        }
        IsGameActive = false;
        OnMatchEnd?.Invoke(leaderBoard);
        Debug.Log("Game have ended");
        Invoke("LeaveRoom", 3f);
    }

    /// <summary>
    /// Called when game starts initialize the board
    /// </summary>
    /// <param name="content"></param>
    private void OnGameStart(Dictionary<byte, object> content)
    {
        IsGameActive = true;
        Debug.Log("OnGameStart");

        int hight = (int)content[0];
        int width = (int)content[1];
        int mineCount = (int)content[2];

        board.MakeBoard(hight, width,mineCount);
        OnMatchBegin?.Invoke();
    }

    /// <summary>
    /// Recive move (revealing the tile)
    /// </summary>
    /// <param name="content"></param>
    private void OnReciveMove(Dictionary<byte, object> content)
    {
        Debug.Log("Recive for opening: " + content.Count);
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

    /// <summary>
    /// Open the rest of tiles if recived more then one tile 
    /// </summary>
    /// <param name="content"></param>
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

    /// <summary>
    /// Recive flag toggle
    /// </summary>
    /// <param name="content"></param>
    private void OnReceiveToggleFlag(Dictionary<byte, object> content)
    {
        int y = (int)content[0];
        int x = (int)content[1];
        bool val = (bool)content[2];

        board.ToggleFlag(y, x, val);
    }

    /// <summary>
    /// On player entered the room
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        OnPlayerEnter?.Invoke(newPlayer);
    }

    /// <summary>
    /// On Player left the room
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        OnPlayerLeft?.Invoke(otherPlayer);
        statusPanel.RemovePlayer(otherPlayer.ActorNumber);
    }

    /// <summary>
    /// Leave the room and chage scene to launcher
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Launcher");
    }

}
