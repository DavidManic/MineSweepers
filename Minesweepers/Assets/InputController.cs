using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    RaycastHit2D hit;
    [SerializeField]
    Board board;
    [SerializeField]
    NetworkController netowork;
    [SerializeField]
    GameObject escapeMenu;


    Camera camera;

    Vector3 start;
    Vector3 end;

    public bool forceMobile = true;

    private float displayHight { get { return (int)camera.orthographicSize * 2; } }
    private float displayWidth { get { return (int)camera.orthographicSize * 2*Camera.main.aspect; } }
    private float HightDifference { get { return (board.Hight - displayHight + 2) / 2; } }
    private float WidthDifference { get { return (board.Width - displayWidth + 2) / 2; } }
    private Vector2 boardCentar = Vector2.zero;
    public void Start()
    {
    #if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isRemoteConnected)
        {
            forceMobile = true;
        }
    #endif
        camera = Camera.main;
        if (forceMobile || Application.platform == RuntimePlatform.Android)
        {
            GetComponent<InputControllerMobile>().enabled = true;
            this.enabled = false;
        }

        netowork.OnMatchBegin += OnMatchBegin;
    }

    private void OnMatchBegin()
    {
        Debug.Log("width: " + board.Width + " hight: " + board.Hight);
        boardCentar = new Vector2(((float)board.Width - 1f) / 2, -((float)board.Hight - 1f) / 2);
        //Debug.Log(boardCentar);
        camera.transform.position = new Vector3( boardCentar.x , boardCentar.y,camera.transform.position.z);
    }

    private float NormalizeWidth(float width)
    {
        //return Mathf.Clamp(width, board.Width / 2 -2 - WidthDifference, board.Width / 2 +2+ WidthDifference);
        //return (displayWidth / 2 - 1)> (board.Width - displayWidth + 1)?board.Width/2: Mathf.Clamp(width, displayWidth/2-2, board.Width - displayWidth/2+2);
        // return Mathf.Clamp(width,boardCentar.x-WidthDifference, boardCentar.x+WidthDifference);
        
        //float widthDifference = WidthDifference < 0 ? 0 : WidthDifference;
        return Mathf.Clamp(width, board.Width / 2 - WidthDifference - 1, board.Width / 2 + WidthDifference + 1);
    }

    private float NormalizeHight(float hight)
    {
        return Mathf.Clamp(hight, -board.Hight / 2 - HightDifference-1, -board.Hight / 2 + HightDifference+1);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            escapeMenu.SetActive(!escapeMenu.activeSelf);
        if (Input.GetMouseButtonDown(0))
            start = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            if (Vector3.Distance(start, Input.mousePosition) > 50)
            {
                end = start - Input.mousePosition;
                end = new Vector3(end.x / Screen.width * displayWidth, end.y / Screen.height * displayHight, end.z);
                MoveCamera(end);
               

            }
            else
            if (hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {
                if (hit.transform.tag == "Tile")
                {
                    Debug.Log(hit);
                    netowork.OpenTile(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                    Debug.Log("Request Opening Tile" + int.Parse(hit.transform.name.Split('_')[1]) + " " + int.Parse(hit.transform.name.Split('_')[2]));
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
            if (hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
            {
                if (hit.transform.tag == "Tile")
                {
                    netowork.ToggleFlag(int.Parse(hit.transform.name.Split('_')[1]), int.Parse(hit.transform.name.Split('_')[2]));
                }
            }

        //Debug.Log(Input.mouseScrollDelta.y);

        if (Input.mouseScrollDelta.y != 0)
        {
            Zoom();
            
        }

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
