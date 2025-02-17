﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    float size = 1f;
    float cameraSizeIndex = 1.7f;

    public TileFactory factory;

    GameObject[,] tiles;
    bool[,] reveald;
    bool[,] flags;
    public int FlagCount { get; protected set; }
    public int ExplodedCount { get; protected set; }

    public int Hight { get; protected set; }
    public int Width { get; protected set; }
    public int MineCount { get; protected set; }


    public delegate void MinesLeftChanged(int current);
    public event MinesLeftChanged OnMinesLeftChanged;

    /// <summary>
    /// Make empty board
    /// </summary>
    /// <param name="hight"></param>
    /// <param name="width"></param>
    /// <param name="mineCount"></param>
    public void MakeBoard(int hight, int width,int mineCount)
    {
        Hight = hight;
        Width = width;
        MineCount = mineCount;
        OnMinesLeftChanged?.Invoke(MineCount);

        tiles = new GameObject[hight, width];
        reveald = new bool[hight, width];
        flags = new bool[hight, width];

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < hight; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tiles[i, j] = factory.MakeBlank(pos);
                tiles[i, j].name = "Tile_" + i + "_" + j;
                tiles[i, j].transform.SetParent(transform);
                pos.x = pos.x + size;
            }
            pos.y = pos.y - size;
            pos.x = pos.x - size * width;
        }

        pos.z = -5;
        pos.x = size * (width + 2) / 2;
        pos.y = -size * hight / 2;
        Camera.main.transform.position = pos;

        Camera.main.orthographicSize = (hight>width?hight:width / 5) * cameraSizeIndex;
        

    }
    /// <summary>
    /// Set a tile vlaue
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="value"></param>
    public void SetTile(int y, int x, int value)
    {
        if (value == 10)
        {

            factory.SetMine(tiles[y, x].GetComponent<SpriteRenderer>());
            tiles[y, x].GetComponent<SpriteRenderer>().color = new Color(.5f, 0, 0);
            MineCount--;
            OnMinesLeftChanged?.Invoke(MineCount);
        }
        else
        {

            factory.ChangeTile(tiles[y, x].GetComponent<SpriteRenderer>(), value);
        }
    }

    /// <summary>
    /// Toggle the flag
    /// </summary>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <param name="IsFlag"></param>
    public void ToggleFlag(int y, int x, bool IsFlag)
    {
        if (IsFlag)
            factory.SetFlag(tiles[y, x].GetComponent<SpriteRenderer>());
        else
            factory.SetNormal(tiles[y,x].GetComponent<SpriteRenderer>());

        MineCount += IsFlag ? -1 : 1;
        OnMinesLeftChanged?.Invoke(MineCount);
    } 

}
