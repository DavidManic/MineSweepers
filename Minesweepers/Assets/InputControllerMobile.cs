using System;
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
    Vector3 start;
    Vector3 end;
    RaycastHit2D hit;
    bool flagging = false;
    bool zooming = false;
    float startZoom;
    private float displayHight { get { return (int)camera.orthographicSize * 2; } }
    private float displayWidth { get { return (int)camera.orthographicSize * 2 * Camera.main.aspect; } }
    private float HightDifference { get { return (board.Hight - displayHight + 2) / 2; } }
    private float WidthDifference { get { return (board.Width - displayWidth + 2) / 2; } }
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        flagButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount >= 2)
        {
            zooming = true;
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);
            Debug.Log("Distance; "+distance);
            Debug.Log(distance / Screen.width);
            camera.orthographicSize = Mathf.Clamp(distance / Screen.width * 15, 5, 15);
        }

        if (Input.touchCount > 0)
        {   if (Input.GetTouch(0).phase == TouchPhase.Began)
                start = Input.GetTouch(0).position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                end = Input.GetTouch(0).position;
                if(Vector3.Distance(start,end)> 50)
                {
                    end = start - Input.mousePosition;
                    end = new Vector3(end.x / Screen.width * displayWidth, end.y / Screen.height * displayHight, end.z);
                    MoveCamera(end);
                }else
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
    private float NormalizeWidth(float width)
    {
        return Mathf.Clamp(width, board.Width / 2 - WidthDifference - 1, board.Width / 2 + WidthDifference + 1);
    }

    private float NormalizeHight(float hight)
    {
        return Mathf.Clamp(hight, -board.Hight / 2 - HightDifference - 1, -board.Hight / 2 + HightDifference + 1);
    }
    private void Zoom()
    {
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - Input.mouseScrollDelta.y, 5, 15);

        if (displayHight < board.Hight || displayWidth < board.Width)
        {
            float WidthDifference = displayWidth - board.Width;
            float HightDifference = displayHight - board.Hight;
            WidthDifference = WidthDifference < 0 ? 0 : WidthDifference;
            HightDifference = HightDifference < 0 ? 0 : HightDifference;
            /*camera.transform.position = new Vector3(displayWidth < board.Width ? Mathf.Clamp(camera.transform.position.x, board.Width / 2 + 1 - WidthDifference, board.Width / 2 + 1 + WidthDifference) : camera.transform.position.x,
                                                      displayHight < board.Hight ? Mathf.Clamp(camera.transform.position.y, -board.Hight / 2 - HightDifference, -board.Hight / 2 + HightDifference) : camera.transform.position.y,
                                                      camera.transform.position.z);*/
            camera.transform.position = new Vector3(displayWidth < board.Width ? NormalizeWidth(camera.transform.position.x) : camera.transform.position.x,
                                                      displayHight < board.Hight ? NormalizeHight(camera.transform.position.y) : camera.transform.position.y,
                                                            camera.transform.position.z);
        }
    }

    private void MoveCamera(Vector3 move)
    {
        if (displayHight < board.Hight || displayWidth < board.Width)
        {
            camera.transform.position = new Vector3(displayWidth < board.Width ? NormalizeWidth(camera.transform.position.x + move.x) : camera.transform.position.x,
                                                      displayHight < board.Hight ? NormalizeHight(camera.transform.position.y + move.y) : camera.transform.position.y,
                                                            camera.transform.position.z);

            /*Debug.Log("minX: " + (board.Width / 2 + 1 - WidthDifference) + " maxX: " + (board.Width / 2 + 1 + WidthDifference));
            Debug.Log("minX: " + (-board.Hight / 2 - HightDifference) + " maxX: " + (-board.Hight / 2 + HightDifference));
            Debug.Log("move: " + move);*/
        }

    }
}
