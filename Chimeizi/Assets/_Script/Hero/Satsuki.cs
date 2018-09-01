using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satsuki : Player
{
    public int deadAddAtk = 0;
    public bool hasDeadBody = false;
    protected override void Start()
    {
        base.Start();
        GameManager.instance.OnRoundChange += GetBody;
    }
    public override void GoDie()
    {

    }
    public override void UseSkill()
    {
        base.UseSkill();
        if (mySkillSelectState == SkillSelectState.First)
        {
            if (deadAddAtk == 20)
            {
                GameManager.instance.vm.ShowNotice("次数用尽");
                return;
            }
            GameManager.instance.vm.TagetPlayerRegisterAndInit(Suck, true, false);
        }
        else if (mySkillSelectState == SkillSelectState.Second)
        {
            if (!hasDeadBody)
            {
                GameManager.instance.vm.ShowNotice("次数用尽");
                return;
            }

        }
    }
    void Suck(string name)
    {
        AddHug(5);
        photonView.RPC("AddHug", PhotonTargets.All, 5, name);
    }
    void AddAtkSkill()
    {

    }
    void GetBody()
    {
        if (mySkillSelectState != SkillSelectState.Second)
        {
            return;
        }
        List<Player> players = GameManager.instance.GetRoomPlayer();
        players.Remove(this);
        foreach (var item in players)
        {
            if (item.playerIsDead)
            {
                hasDeadBody = true;
                Destroy(item);
                break;
            }
        }
    }
}
