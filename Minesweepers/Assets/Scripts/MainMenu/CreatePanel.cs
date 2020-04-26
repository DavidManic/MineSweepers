using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : MonoBehaviour
{
    [SerializeField]
    InputField roomName;
    [SerializeField]
    private Dropdown mod;
    [SerializeField]
    private Text hightText;
    [SerializeField]
    private Text widthText;
    [SerializeField]
    private Text playersText;
    [SerializeField]
    private Text percentText;
    [SerializeField]
    private Toggle firstSafe;
    [SerializeField]
    private Toggle endOnExpload;
    [SerializeField]
    private Toggle joinAfter;

    [SerializeField]
    NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {

        hightText.text = "5";
        widthText.text = "5";
        playersText.text = "2";
        percentText.text = "10";
    }

    public void OnHightChanged(float value)
    {
        hightText.text = ""+(int)value;
    }

    public void OnWidthChanged(float value)
    {

        widthText.text = "" + (int)value;
    }

    public void OnPlayersChanged(float value)
    {

        playersText.text = "" + (int)value;
    }

    public void OnPercentChanged(float value)
    {

        percentText.text = "" + (int)value;
    }

    public void OnCreateClick()
    {
        if(!string.IsNullOrEmpty(roomName.text))
            networkManager.CreateRoom(new GameOptions(roomName.text, mod.options[mod.value].text, int.Parse(hightText.text), int.Parse(widthText.text), int.Parse(playersText.text),int.Parse(percentText.text),
                firstSafe,endOnExpload,joinAfter));
    }

    public void OnCancelClicke()
    {
        gameObject.SetActive(false);
    }
}
