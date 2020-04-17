using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServersPanel : MonoBehaviour
{
    private List<GameObject> serverEntries = new List<GameObject>();
    public GameObject serverView;

    private void Start()
    {
        NetworkManager.Instance.OnRoomsChanged += OnRoomsChanged;
    }

    private void OnRoomsChanged()
    {
        if (gameObject.activeSelf)
            Show("");
    }

    public void Show(string name)
    {
        Debug.Log("Number of rooms: " + NetworkManager.Rooms.Count);
        gameObject.SetActive(true);
        serverEntries.ForEach((a) => { DestroyImmediate(a); });//TODO pooling!!!

        foreach (RoomInfo room in NetworkManager.Rooms.Values)
            if (name.Equals("") || name.Equals(room.CustomProperties["Plugin"].ToString()))
            {
                MakeServerEntry(room);
            }
    }
    

    void MakeServerEntry(RoomInfo info)
    {
        serverEntries.Add(Instantiate(serverView).GetComponent<ServerEntry>().Setup(info, transform));
    }
}
