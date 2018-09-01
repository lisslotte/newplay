using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Rika : Player
{
    public GameObject selectRoomPanel;
    public List<string> relifeRooms = new List<string>();
    int hasSelect = 0;

    protected override void Start()
    {
        base.Start();
        selectRoomPanel.SetActive(true);
    }
    public void SelectRelifeRoom(string name)
    {
        relifeRooms.Add(name);
        hasSelect++;
        if (hasSelect == 3)
        {
            selectRoomPanel.SetActive(false);
            if (mySkillSelectState == SkillSelectState.Second)
            {
                GameManager.instance.OnRoundChange += HanyuuMove;
            }
        }
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
        photonView.RPC("AddCrazy", PhotonTargets.Others, hanyuuRoom);
    }
}
