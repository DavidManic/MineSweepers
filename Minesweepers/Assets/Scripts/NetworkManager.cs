using Assets;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{

    /// <summary>
    /// List of all game rooms.
    /// </summary>
    public Dictionary<string, RoomInfo> Rooms { get; protected set; } = new Dictionary<string, RoomInfo>();
    public delegate void RoomsChanged();
    public RoomsChanged OnRoomsChanged;
    private bool reconnecting = false;

    public static NetworkManager Instance { get; protected set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby("Main", LobbyType.SqlLobby));

    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        if(!reconnecting)
            StartCoroutine(Reconnect());
    }

    private IEnumerator Reconnect()
    {
        reconnecting = true;
        while (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Reconnecting");

            if (!PhotonNetwork.ReconnectAndRejoin())
                PhotonNetwork.ConnectUsingSettings();

            yield return new WaitForSeconds(3f);
        }
        reconnecting = false;
    }
    public void CreateRoom(GameOptions options)
    {
        if (!Rooms.ContainsKey(options.name))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = 4;
            roomOptions.Plugins = new string[] { "MineSweepers"+options.plugin };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                { "plugin", "MineSweepers"+options.plugin }, { "hight", options.hight }, { "width", options.width },
                { "maxPlayers", options.numOfPlayers }, { "mineRate", options.mineRate },
                { "firstSafe", options.firstSafe },{ "endOnExpload",options.endOnExpload },
                { "joinAfter",options.JoinAfterStart } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "plugin", "hight", "width", "mineRate", "maxPlayers" };

            if (!options.JoinAfterStart) roomOptions.MaxPlayers = (byte)options.numOfPlayers;

            TypedLobby lobby = new TypedLobby("Main", LobbyType.SqlLobby);
            PhotonNetwork.CreateRoom(options.name, roomOptions, lobby);
        }
    }
    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("MultiPlayer");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(message);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo info in roomList)
            if (!info.IsVisible || info.RemovedFromList)
            {
                if (Rooms.ContainsKey(info.Name))
                {
                    Rooms.Remove(info.Name);
                }
            }
            else if (Rooms.ContainsKey(info.Name))
            {
                Rooms[info.Name] = info;
            }
            else
            {
                Rooms.Add(info.Name, info);
            }

        OnRoomsChanged?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
    }

    public void SetUsername(string username)
    {
        PhotonNetwork.LocalPlayer.NickName = username;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(5, 500, 100, 50), PhotonNetwork.NetworkClientState.ToString());
        if(PhotonNetwork.IsConnected)
            GUI.Label(new Rect(Screen.width/2-50, 10, 100, 50),"Online players:"+ PhotonNetwork.CountOfPlayers.ToString());
        else
            GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 50), "Disconnected" );
    }

}
