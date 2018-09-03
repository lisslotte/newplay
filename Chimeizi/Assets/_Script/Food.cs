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
        GameManager.instance.OnTwilightMoveEvent += Use;
        GameManager.instance.OnNightMoveEvent += Use;
    }
    public void SetHugNumber(int hug)
    {
        hugNumber = hug;
        text.text = (hugNumber*5).ToString();
    }
    public void Dead()
    {
        GameManager.instance.OnDayMoveEvent -= Use;
        GameManager.instance.OnTwilightMoveEvent -= Use;
        GameManager.instance.OnNightMoveEvent -= Use;
        try
        {
            PhotonNetwork.Destroy(gameObject);
        }
        catch { }
    }
    public void Use()
    {
        List<Player> player = GameManager.instance.GetRoomPlayer();
        if (player.Count == 0)
        {
            return;
        }
        int perPlayerHug = (int)(hugNumber*5 / player.Count);
        foreach (var item in player)
        {
            if (myRoom!=item.myRoom)
            {
                return;
            }
            Debug.Log(item.myHeroName + "增加" + perPlayerHug);
            item.AddHug(perPlayerHug);
        }
        
        try
        {
            PhotonNetwork.Instantiate("Hearts", transform.position, Quaternion.identity, 0);
            PhotonNetwork.Destroy(gameObject);
        }
        catch { }
    }
}
