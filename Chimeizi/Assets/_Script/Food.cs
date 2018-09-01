using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Food : Photon.PunBehaviour
{
    public string myRoom;
    public int hugNumber = 1;
    public TextMesh text;
    private void Start()
    {
        GameManager.instance.OnDayMoveEvent += Use;
    }
    public void SetHugNumber(int hug)
    {
        hugNumber = hug;
        text.text = hugNumber.ToString();
    }
    public void Dead()
    {
        GameManager.instance.OnDayMoveEvent -= Use;
        PhotonNetwork.Destroy(gameObject);
    }
    public void Use()
    {
        List<Player> player = GameManager.instance.GetAllPlayer();
        List<Player> roomPlayer = new List<Player>();
        foreach (var item in player)
        {
            if (item.myRoom == myRoom)
            {
                roomPlayer.Add(item);
            }
        }
        if (roomPlayer.Count == 0)
        {
            return;
        }
        int perPlayerHug = (int)(hugNumber / roomPlayer.Count);
        foreach (var item in roomPlayer)
        {
            item.AddHug(perPlayerHug);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
