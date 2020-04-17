using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class InputManager : MonoBehaviour
{
    RaycastHit2D hit;
    [SerializeField]
    Table table;
    [SerializeField]
    NetworkController netowork;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {
                if (hit.transform.tag == "Tile")
                {
                    Debug.Log(hit);
                    //table.OpenTile(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                    netowork.OpenTile(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
            if (hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {
                if (hit.transform.tag == "Tile")
                {
                    table.SetFlag(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                }
            }
    }
}