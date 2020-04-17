using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    float size = 0.64f;
    float cameraSizeIndex = 1.7f;
    public int width = 5;
    public int hight = 5;
    public float minePercent = 0.1f;
    public TileFactory factory;
    GameObject[,] tiles;
    bool[,] reveald;
    int revealCount = 0;
    bool[,] flags;
    int flagCount = 0;
    Mines mines;
    public int MineCount
    {
        get
        {
            return mines.MineCount;
        }
    }

    public delegate void FlagsCountChanged(int current);
    public event FlagsCountChanged OnFlagsCountChanged;

    // Start is called before the first frame update
    void Start()
    {


        tiles = new GameObject[hight, width];
        reveald = new bool[hight, width];
        flags = new bool[hight, width];

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < hight; i++)
        {
            for (int j = 0; j < width; j++)
            {
                pos.x = pos.x + size;
                tiles[i, j] = factory.MakeBlank(pos);
                tiles[i, j].name = "Tile_" + i + "_" + j;
                tiles[i, j].transform.SetParent(transform);
            }
            pos.y = pos.y - size;
            pos.x = pos.x - size * width;
        }
        pos.z = -5;
        pos.x = size * (width + 1) / 2;
        pos.y = -size * hight / 2;
        Camera.main.transform.position = pos;
        Camera.main.orthographicSize = (hight / 5) * cameraSizeIndex;

        mines = new Mines(width, hight, Mathf.FloorToInt(width * hight * minePercent));

        OnFlagsCountChanged?.Invoke(flagCount);
    }
    

    public void Restart()
    {
        reveald = new bool[hight, width];
        flags = new bool[hight, width];
        for (int i = 0; i < hight; i++)
            for (int j = 0; j < width; j++)
                factory.SetNormal(tiles[i, j].GetComponent<SpriteRenderer>());

                mines = new Mines(width, hight, Mathf.FloorToInt(width * hight * 0.25f));

        OnFlagsCountChanged?.Invoke(flagCount);

    }

    public void SetFlag(int y, int x)
    {
        if (!reveald[y, x])
        {

            if (flags[y, x])
                factory.SetNormal(tiles[y, x].GetComponent<SpriteRenderer>());
            else
                factory.SetFlag(tiles[y, x].GetComponent<SpriteRenderer>());

            flagCount += flags[y, x] ? -1 : 1;
            flags[y, x] = !flags[y, x];

            OnFlagsCountChanged?.Invoke(flagCount);
        }
    }
    public void SetTile(int y, int x,int value)
    {
        mines.SetTile(y, x, value);
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
    public void OpenTile(int y, int x)
    {
        if (flags[y, x]) return;
        if (reveald[y, x])
            TryOpenRest(y, x);
        else
        {

            reveald[y, x] = true;
            revealCount++;
            int value = mines.GetTile(y, x);
            if (value == 10)
            {
                Debug.Log("MINE!|!");
                factory.SetMine(tiles[y, x].GetComponent<SpriteRenderer>());
                tiles[y, x].GetComponent<SpriteRenderer>().color = new Color(.5f, 0, 0);
                revealMines();
            }
            else
            {

                Debug.Log(width*hight - revealCount);
                factory.ChangeTile(tiles[y, x].GetComponent<SpriteRenderer>(), value);
            }

            if (value == 0)
                expand(y, x);

            if (MineCount == width * hight - revealCount)
                CheckGameEnd();
        }

    }

    private void CheckGameEnd()
    {
        Debug.Log("GameEnd");
    }

    private void revealMines()
    {
        for(int i = 0; i < hight; i ++)
            for(int j = 0; j < width; j ++)
            {
                if (mines.GetTile(i, j) == 10 && !flags[i, j])
                    factory.SetMine(tiles[i, j].GetComponent<SpriteRenderer>());
            }
    }

    private void TryOpenRest(int y, int x)
    {
        if(mines.GetTile(y,x) == NumOfFlags(y, x))
        {
            int startY = (y - 1) < 0 ? 0 : y - 1;
            int startX = (x - 1) < 0 ? 0 : x - 1;
            int endY = (y + 1) >= hight ? hight - 1 : y + 1;
            int endX = (x + 1) >= width ? width - 1 : x + 1;
            for (int i = startY; i <= endY; i++)
                for (int j = startX; j <= endX; j++)
                    if (!reveald[i, j])
                        OpenTile(i, j);
        }
    }

    private int NumOfFlags(int y, int x)
    {
        int count = 0;
        int startY = (y - 1) < 0 ? 0 : y - 1;
        int startX = (x - 1) < 0 ? 0 : x - 1;
        int endY = (y + 1) >= hight ? hight - 1 : y + 1;
        int endX = (x + 1) >= width ? width - 1 : x + 1;
        for (int i = startY; i <= endY; i++)
            for (int j = startX; j <= endX; j++)
                if (flags[i, j])
                    count++;
        return count;
    }

    private void expand(int y,int x)
    {
        int startY = (y - 1) < 0 ? 0 : y - 1;
        int startX = (x - 1) < 0 ? 0 : x - 1;
        int endY = (y + 1) >= hight ? hight - 1 : y + 1;
        int endX = (x + 1) >= width ? width - 1 : x + 1;
        for (int i = startY; i <= endY; i++)
            for (int j = startX; j <= endX; j++)
                if (!reveald[i, j])
                    OpenTile(i, j);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
