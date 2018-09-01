using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aqua : Player
{
    public int zombieNumber = 1;
    public bool skillReady = true;
    public override void UseSkill()
    {
        if (mySkillSelectState == SkillSelectState.First)
        {
            if (skillReady)
            {
                base.UseSkill();
                skillReady = false;
                GameManager.instance.vm.TagetPlayerRegisterAndInit(Blessing, true, true);
            }
        }
        else
        {

        }
    }
    protected override void OnDay()
    {
        base.OnDay();
        skillReady = true;
    }
    protected override void OnNight()
    {
        base.OnNight();
        if (mySkillSelectState == SkillSelectState.Second)
        {
            GameObject zombie = PhotonNetwork.Instantiate("AquaZombie", Vector3.zero, Quaternion.identity, 0);
            var az = zombie.GetComponent<AquaZombie>();
            az.myRoom = myRoom;
            az.attackList = GameManager.instance.GetRoomPlayer();
            az.attackList.Remove(az);
            az.zonbieNumber = zombieNumber;
            MapData.instance.SetCenterPoint(zombie.transform, GameManager.instance.myPlayer.myRoom);
            zombieNumber++;
        }
    }
    public void Blessing(string name)
    {
        GameObject skill = PhotonNetwork.Instantiate("AquaSkill", Vector3.zero, Quaternion.identity, 0);
        var aSkill = skill.GetComponent<AquaSkill>();
        aSkill.targetHero = name;
        aSkill.targetPlayer = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == name);
        aSkill.Init();
    }
}
