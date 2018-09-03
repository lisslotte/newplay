using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Rika : Player
{
    public List<string> relifeRooms = new List<string>();

    protected override void Start()
    {
        base.Start();
        SelectRelifeRoom();
    }
    public void SelectRelifeRoom()
    {
        var a = Random.Range(0, 3);
        var b = Random.Range(3, 5);
        var c = Random.Range(5, MapData.instance.roomCount);
        relifeRooms.Add(MapData.instance.roomNameDict[a]);
        relifeRooms.Add(MapData.instance.roomNameDict[b]);
        relifeRooms.Add(MapData.instance.roomNameDict[c]);
        GameManager.instance.vm.ShowNotice("羽入的徘徊地" + relifeRooms[0] + relifeRooms[1] + relifeRooms[2]);
    }
    public override void GoDie()
    {
        if (mySkillSelectState == SkillSelectState.First && relifeRooms.Count > 0)
        {
            string room = relifeRooms[0];
            relifeRooms.RemoveAt(0);
            MapData.instance.SetPlayerToRoom(PhotonNetwork.player.ID, transform, room);
            minAtk = Mathf.Clamp(minAtk + 10, minAtk, maxAtk);
            hug = 10;
            PhotonNetwork.Instantiate("ShowEffect", transform.position, Quaternion.identity, 0);
            GameManager.instance.vm.ShowNotice("你在" + room + "重生了");
        }
        else
        {
            GameManager.instance.OnRoundChange -= HanyuuMove;
            base.GoDie();
        }
    }
    public void HanyuuMove()
    {
        string hanyuuRoom = relifeRooms[Random.Range(0, relifeRooms.Count)];
        GameManager.instance.vm.ShowNotice("羽入在" + hanyuuRoom);
        GameManager.instance.AddCrazyToOther(hanyuuRoom);
    }
}
