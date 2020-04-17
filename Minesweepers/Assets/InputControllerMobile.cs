using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputControllerMobile : MonoBehaviour
{
    [SerializeField]
    Board board;
    [SerializeField]
    NetworkController netowork;
    [SerializeField]
    GameObject flagButton;

    Camera camera;
    RaycastHit2D hit;
    bool flagging = false;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        flagButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {

                if (hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero))
                {
                    if (hit.transform.tag == "Tile")
                    {
                        Debug.Log(hit);
                        if (flagging)
                        {
                            netowork.ToggleFlag(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                            Debug.Log("Flagging" + int.Parse(hit.transform.name.Split('_')[1]) + " " + int.Parse(hit.transform.name.Split('_')[2]));
                        }
                        else
                        {
                            //table.OpenTile(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                            netowork.OpenTile(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                            Debug.Log("Request Opening Tile" + int.Parse(hit.transform.name.Split('_')[1]) + " " + int.Parse(hit.transform.name.Split('_')[2]));
                        }
                    }
                }
            }
        }
    }

    public void ToggleFlag()
    {
        flagging = !flagging;
        flagButton.GetComponent<Image>().color = flagging ? Color.green : Color.white;
    }
}
