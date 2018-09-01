using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Photon.PunBehaviour {
    public GameObject prograssPanel;
    public GameObject controlPanel;

    public Animator shakeAni;

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
            JoinGame();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
        InPrograss(true);
    }
    public override void OnConnectedToMaster()
    {
        JoinGame();
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
    void JoinGame()
    {
        PhotonNetwork.JoinOrCreateRoom("1", new RoomOptions { MaxPlayers = 20 }, null);
    }
    void InPrograss(bool b)
    {
        prograssPanel.SetActive(b);
        controlPanel.SetActive(!b);
    }
}
