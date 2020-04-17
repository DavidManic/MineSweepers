using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    [SerializeField]
    Text minesLeft;

    [SerializeField]
    Board board;
    [SerializeField]
    GameObject playerScore;
    [SerializeField]
    Transform scoresPanel;

    Dictionary<int, string> names;
    Dictionary<int, Text> scores;
    // Start is called before the first frame update
    void Start()
    {
        
        board.OnMinesLeftChanged += OnMinesLeftChanged;

        //minesLeft.text = "" + table.MineCount;
    }

    private void OnMinesLeftChanged(int current)
    {
        minesLeft.text = "Mines left: " + current;
    }

    public void Setup(Dictionary<int,int> startScores)
    {
        this.scores = new Dictionary<int, Text>();
        this.names = new Dictionary<int, string>();
        
        foreach(int key in startScores.Keys)
        {
            GameObject g = Instantiate(playerScore);
            g.transform.SetParent(scoresPanel);
            scores.Add(key, g.GetComponent<Text>());
            Player player = PhotonNetwork.CurrentRoom.Players.First(x => x.Key == key).Value;

            names.Add(key, player.NickName.Length > 0 ? player.NickName : "User_"+ player.ActorNumber);
            scores[key].text = names[key] + " : " + startScores[key];
        }
        
    }

    public void UpdateScore(int key,int value)
    {
        scores[key].text = names[key] + " : " + value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
