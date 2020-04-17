using System;
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
    public int FlagCount { get; private set; }
    public int ExplodedCount { get; private set; }

    public int Hight { get; private set; }
    public int Width { get; private set; }


    public delegate void MinesLeftChanged(int current);
    public event MinesLeftChanged OnMinesLeftChanged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MakeBoard(int hight, int width)
    {
        Hight = hight;
        Width = width;

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

    public void SetTile(int y, int x, int value)
    {
        if (value == 10)
        {

            factory.SetMine(tiles[y, x].GetComponent<SpriteRenderer>());
            tiles[y, x].GetComponent<SpriteRenderer>().color = new Color(.5f, 0, 0);
        }
        else
        {

            factory.ChangeTile(tiles[y, x].GetComponent<SpriteRenderer>(), value);
        }
    }

    public void ToggleFlag(int y, int x, bool IsFlag)
    {
        if (IsFlag)
            factory.SetFlag(tiles[y, x].GetComponent<SpriteRenderer>());
        else
            factory.SetNormal(tiles[y,x].GetComponent<SpriteRenderer>());
    } 

}
