using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misaka : Player
{
    public override void UseSkill()
    {
        base.UseSkill();
        if (mySkillSelectState == SkillSelectState.First)
        {
            SkillFirstBtn();
        }
        else
        if (mySkillSelectState == SkillSelectState.Second)
        {

        }
    }
    public void SkillFirstBtn()
    {
        GameManager.instance.vm.TagetRoomRegisterAndInit(SkillFirstFunc);
    }
    public void SkillFirstFunc(string room)
    {
        List<Food> foods = GameManager.instance.GetAllFood();
        foreach (var item in foods)
        {
            if (item.myRoom == room)
            {
                item.Dead();
                PhotonNetwork.Instantiate("MisakaCoin", item.transform.position + 3 * Vector3.up, Quaternion.identity, 0);
                PhotonNetwork.Instantiate("MisakaCoinBloom", item.transform.position, Quaternion.identity, 0);
            }
        }
        PhotonNetwork.Instantiate("MisakaCoin", transform.position + Vector3.up * 1.8f, Quaternion.Euler(232, -147, 149), 0);
    }
    public void SkillSecondBtn()
    {
        if (mySkillSelectState != SkillSelectState.Second)
        {
            return;
        }
        GameManager.instance.vm.TagetPlayerRegisterAndInit(SkillSecondFunc, true, false);
    }
    public void SkillSecondFunc(string name)
    {
        photonView.RPC("SetRandomRoom", PhotonTargets.Others, name, true);
    }
}
