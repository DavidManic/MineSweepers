using Assets;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{

    /// <summary>
    /// List of all game rooms.
    /// </summary>
    public static Dictionary<string, RoomInfo> Rooms { get; protected set; }
    public delegate void RoomsChanged();
    public RoomsChanged OnRoomsChanged;

    public static NetworkManager Instance { get; protected set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            throw new System.Exception("An instance of this singleton already exists.");
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
        Rooms = new Dictionary<string, RoomInfo>();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby("Main", LobbyType.SqlLobby));

    }

    public override void OnJoinedLobby()
    {
        /* RoomOptions roomOptions = new RoomOptions();
         roomOptions.IsVisible = true;
         roomOptions.IsOpen = true;
         roomOptions.MaxPlayers = 4;
         roomOptions.Plugins = new string[] { "MineSweepersPlugin" };
         //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "HeroCount", heroCount }, { "EnemieCount", enemieCount } };

         TypedLobby lobby = new TypedLobby("Main", LobbyType.SqlLobby);
         PhotonNetwork.CreateRoom("aaa", roomOptions, lobby);*/
    }
    public void CreateRoom(GameOptions options)
    {
        //if (!Rooms.ContainsKey(name))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = 4;
            roomOptions.Plugins = new string[] { "MineSweepers"+options.plugin };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {
                { "plugin", "MineSweepersVersus" }, { "hight", options.hight }, { "width", options.width },
                { "maxPlayers", options.numOfPlayers }, { "mineRate", options.mineRate },
                { "firstSafe", options.firstSafe },{ "endOnExpload",options.endOnExpload },
                { "joinAfter",options.JoinAfterStart } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "plugin", "hight", "width", "mineRate" };

            TypedLobby lobby = new TypedLobby("Main", LobbyType.SqlLobby);
            PhotonNetwork.CreateRoom(name, roomOptions, lobby);
        }
    }
    public void CreateRoom(string name,int hight,int width,int maxPlayers)
    {
        //if (!Rooms.ContainsKey(name))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = 4;
            roomOptions.Plugins = new string[] { "MineSweepersVersus" };
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "plugin", "MineSweepersVersus" }, { "hight", hight }, { "width", width },{ "maxPlayers", maxPlayers } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "plugin", "hight", "width", "mineRate" };

            TypedLobby lobby = new TypedLobby("Main", LobbyType.SqlLobby);
            PhotonNetwork.CreateRoom(name, roomOptions, lobby);
        }
    }
    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("MultiPlayer");
    }
    public void CreateRoom()
    {
        if (Rooms.ContainsKey("aaa"))
            PhotonNetwork.JoinRoom("aaa");
        else
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            roomOptions.MaxPlayers = 4;
            roomOptions.Plugins = new string[] { "MineSweepersVersus" };
            //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "HeroCount", heroCount }, { "EnemieCount", enemieCount } };

            TypedLobby lobby = new TypedLobby("Main", LobbyType.SqlLobby);
            PhotonNetwork.CreateRoom("aaa", roomOptions, lobby);
        }
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
        GUI.Label(new Rect(Screen.width/2-50, 10, 100, 50),"Online players:"+ PhotonNetwork.CountOfPlayers.ToString());
    }

}
