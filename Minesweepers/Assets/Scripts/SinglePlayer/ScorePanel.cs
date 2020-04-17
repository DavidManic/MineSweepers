using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [SerializeField]
    Text flagedMines;
    [SerializeField]
    Text MinesLeft;
    [SerializeField]
    Table table;
    // Start is called before the first frame update
    void Start()
    {
        table.OnFlagsCountChanged += OnFlagCountChange;

        MinesLeft.text = "" + table.MineCount;
    }

    private void OnFlagCountChange(int current)
    {
        flagedMines.text = "" + current;
        MinesLeft.text = "" + (table.MineCount - current);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
