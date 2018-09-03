using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Launcher : Photon.PunBehaviour
{
    public GameObject prograssPanel;
    public GameObject controlPanel;
    public GameObject lobbyPanel;
    public Animator shakeAni;
    public string selectRoom;

    List<GameObject> rooms = new List<GameObject>();
    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }
    private void Start()
    {
        InPrograss(false);
    }
    public void Connect()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            shakeAni.SetTrigger("shake");
            return;
        }
        if (PhotonNetwork.connected)
        {
            JoinLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
        InPrograss(true);
    }
    public override void OnConnectedToMaster()
    {
        JoinLobby();
    }
    public override void OnDisconnectedFromPhoton()
    {
        InPrograss(false);
    }
    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("joined room,player count " + PhotonNetwork.countOfPlayers);
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevel("Map_01");
    }
    public override void OnJoinedLobby()
    {
        InPrograss(false);
        lobbyPanel.SetActive(true);
        controlPanel.SetActive(false);
        InvokeRepeating("RefreshRoomList", 0.5f, 0.5f);
    }
    public void RefreshRoomList()
    {
        var list = PhotonNetwork.GetRoomList();
        var panel = lobbyPanel.transform.Find("Image");
        var room = panel.Find("Button");
        foreach (var item in list)
        {
            foreach (var ro in rooms)
            {
                if (item.Name == ro.name)
                {
                    return;
                }
            }
            var r = Instantiate(room.gameObject, panel);
            r.name = item.Name;
            rooms.Add(r);
            r.SetActive(true);
            r.GetComponentInChildren<Text>().text = item.Name;
        }
    }
    public void OnSelectRoom(Transform item)
    {
        selectRoom =item.Find("Text").GetComponent<Text>().text;
        foreach (var t in rooms)
        {
            t.transform.Find("Image").gameObject.SetActive(false);
        }
        item.Find("Image").gameObject.SetActive(true);
    }
    void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
        //  PhotonNetwork.JoinOrCreateRoom("1", new RoomOptions { MaxPlayers = 20 }, null);
    }
    void InPrograss(bool b)
    {
        prograssPanel.SetActive(b);
        controlPanel.SetActive(!b);
    }
    public void CreatRoom()
    {
        InPrograss(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.CreateRoom(PhotonNetwork.player.NickName + "'s room",new RoomOptions { MaxPlayers = 7 }, null);
    }
    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(selectRoom))
        {
            return;
        }
        PhotonNetwork.JoinRoom(selectRoom);
    }
}
