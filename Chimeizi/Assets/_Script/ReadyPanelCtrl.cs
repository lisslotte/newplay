using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReadyPanelCtrl : MonoBehaviour
{
    public Text[] playerNameCell;
    public Text buttonText;
    public bool isReady = false;
    public int playerCount = 0;
    private void Start()
    {
        foreach (var item in playerNameCell)
        {
            item.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (PhotonNetwork.playerList.Length != playerCount)
        {
            playerCount = PhotonNetwork.playerList.Length;
            UpdatePlayerList();
        }
    }

    void UpdatePlayerList()
    {
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            playerNameCell[i].text = PhotonNetwork.playerList[i].NickName;
            playerNameCell[i].gameObject.SetActive(true);
        }
    }
    public void Ready()
    {
        if (isReady)
        {
            isReady = false;
            buttonText.text = "准备";
        }
        else
        {
            isReady = true;
            buttonText.text = "取消准备";
        }
        GameManager.instance.Ready(isReady);
    }
}
