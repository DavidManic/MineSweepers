
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    [SerializeField]
    GameObject Tile;
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Dictionary<string, Sprite> tiles = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
    void Awake()
    {
        tiles.Clear();
        foreach (Sprite s in sprites)
            tiles.Add(s.name.Split('_')[1], s);

    }

    public GameObject MakeTile(int number,Vector3 position)
    {
        Sprite s;
        if(tiles.TryGetValue(number.ToString(),out s))
        {
            GameObject g = Instantiate(Tile, position, Quaternion.identity);
            g.GetComponent<SpriteRenderer>().sprite = s;
            return g;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }

    public GameObject MakeBlank(Vector3 position)
    {
        Sprite s;
        if (tiles.TryGetValue("Normal", out s))
        {
            GameObject g = Instantiate(Tile, position, Quaternion.identity);
            g.GetComponent<SpriteRenderer>().sprite = s;
            return g;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }

    public GameObject SetFlag(SpriteRenderer tile)
    {
        Sprite s;
        if (tiles.TryGetValue("Flag", out s))
        {
            tile.sprite = s;
            return tile.gameObject;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }

    public GameObject SetNormal(SpriteRenderer tile)
    {
        Sprite s;
        if (tiles.TryGetValue("Normal", out s))
        {
            tile.sprite = s;
            return tile.gameObject;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }

    public GameObject SetMine(SpriteRenderer tile)
    {
        Sprite s;
        if (tiles.TryGetValue("Mine", out s))
        {
            tile.sprite = s;
            return tile.gameObject;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }

    public GameObject ChangeTile(SpriteRenderer tile, int number)
    {
        Sprite s;
        if (tiles.TryGetValue(number.ToString(), out s))
        {
            tile.sprite = s;
            return tile.gameObject;
        }
        else
        {
            Debug.Log("Can't find card");
        }

        return null;
    }
}
