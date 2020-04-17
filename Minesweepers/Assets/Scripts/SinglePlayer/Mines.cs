using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mines
{
    int[,] mines;
    int width;
    int hight;
    int mineCount;

    public int MineCount { get { return mineCount; } }

    public Mines(int width, int hight, int mineCount)
    {
        this.width = width;
        this.hight = hight;
        this.mineCount = mineCount;
        mines = new int[hight, width];
        placeMines();
        calculateTiles();
    }
    public int GetTile(int y,int x)
    {
        return mines[y, x];
    }
    public void SetTile(int y, int x, int val)
    {
        mines[y, x] = val;

        int startY = (y - 1) < 0 ? 0 : y - 1;
        int startX = (x - 1) < 0 ? 0 : x - 1;
        int endY = (y + 1) >= hight ? hight - 1 : y + 1;
        int endX = (x + 1) >= width ? width - 1 : x + 1;
        for (int i = startY; i <= endY; i++)
            for (int j = startX; j <= endX; j++)
                if (mines[i, j] != 10)
                    mines[i, j] = NumOfMines(i, j);


    }

    private void calculateTiles()
    {
        for (int i = 0; i < hight; i++)
            for (int j = 0; j < width; j++)
            {
                if (mines[i, j] != 10)
                    mines[i, j] = NumOfMines(i, j);
            }
    }

    private int NumOfMines(int y, int x)
    {
        int count = 0;
        int startY = (y - 1) < 0 ? 0 : y - 1;
        int startX = (x - 1) < 0 ? 0 : x - 1;
        int endY = (y + 1) >= hight ? hight - 1 : y + 1;
        int endX = (x + 1) >= width ? width - 1 : x + 1;
        for (int i = startY; i <= endY; i++)
            for (int j = startX; j <= endX; j++)
            {

                if (mines[i, j] == 10) count++;
            }
        return count;
    }

    private void placeMines()
    {
        int count = 0;
        Random r = new Random();
        while (count < mineCount)
        {
            int x = r.Next(width);
            int y = r.Next(hight);
            if (mines[y, x] != 10)
            {
                mines[y, x] = 10;
                count++;
            }
        }
    }
}
