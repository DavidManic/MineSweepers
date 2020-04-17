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

    private int displayHight { get { return (int)camera.orthographicSize * 2; } }
    private int displayWidth { get { return (int)camera.orthographicSize * 4; } }
    private float HightDifference { get { return (board.Hight - displayHight + 1) / 2; } }
    private float WidthDifference { get { return (board.Width - displayWidth + 1) / 2; } }

    public void Start()
    {
        camera = Camera.main;
        if (forceMobile || Application.platform == RuntimePlatform.Android)
        {
            GetComponent<InputControllerMobile>().enabled = true;
            this.enabled = false;
        }
    }

    private float NormalizeWidth(float width)
    {
        //return Mathf.Clamp(width, board.Width / 2 -2 - WidthDifference, board.Width / 2 +2+ WidthDifference);
        return (displayWidth / 2 - 1)> (board.Width - displayWidth + 1)?board.Width/2: Mathf.Clamp(width, displayWidth/2-2, board.Width - displayWidth/2+2);
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
                end = new Vector3(end.x / Screen.width * displayWidth, end.y / Screen.height * displayHight, end.y);
                if (displayHight < board.Hight || displayWidth < board.Width)
                {
                    camera.transform.position = new Vector3(displayWidth < board.Width ? NormalizeWidth(camera.transform.position.x + end.x) : camera.transform.position.x,
                                                              displayHight < board.Hight ? NormalizeHight(camera.transform.position.y + end.y) : camera.transform.position.y,
                                                                    camera.transform.position.z);

                    Debug.Log("minX: " + (board.Width / 2 + 1 - WidthDifference) + " maxX: " + (board.Width / 2 + 1 + WidthDifference));
                    Debug.Log("minX: " + (-board.Hight / 2 - HightDifference) + " maxX: " + (-board.Hight / 2 + HightDifference));
                    Debug.Log("move: " + end);
                }


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

        Debug.Log(Input.mouseScrollDelta.y);

        if (Input.mouseScrollDelta.y != 0)
        {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - Input.mouseScrollDelta.y, 5, 15);

            int displayWidth = (int)camera.orthographicSize * 4;
            int displayHight = (int)camera.orthographicSize * 2;
            if (displayHight < board.Hight || displayWidth < board.Width)
            {
                int WidthDifference = displayWidth - board.Width;
                int HightDifference = displayHight - board.Hight;
                WidthDifference = WidthDifference < 0 ? 0 : WidthDifference;
                HightDifference = HightDifference < 0 ? 0 : HightDifference;
                camera.transform.position = new Vector3(displayWidth < board.Width ? Mathf.Clamp(camera.transform.position.x, board.Width / 2 + 1 - WidthDifference, board.Width / 2 + 1 + WidthDifference) : camera.transform.position.x,
                                                          displayHight < board.Hight ? Mathf.Clamp(camera.transform.position.y, -board.Hight / 2 - HightDifference, -board.Hight / 2 + HightDifference) : camera.transform.position.y,
                                                                camera.transform.position.z);
            }
        }

    }
}
