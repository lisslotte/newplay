using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerName : MonoBehaviour {
    
    public void SetPlayerName(string value)
    {
        PhotonNetwork.playerName = value + " ";
    }
}
