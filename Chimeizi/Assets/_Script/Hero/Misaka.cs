using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misaka : Player
{
    public bool isreadyFirst = true;
    public bool canUseTwo = true;
    public override void UseSkill()
    {
        
        if (mySkillSelectState == SkillSelectState.First)
        {
            if (isreadyFirst)
            {
                base.UseSkill();
                SkillFirstBtn();
                isreadyFirst = false;
            }
            
        }
        else
        if (mySkillSelectState == SkillSelectState.Second)
        {
            if (canUseTwo)
            {
                base.UseSkill();
                canUseTwo = false;
                StartCoroutine("SkillSecondFunc");
            }
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
   
   IEnumerator SkillSecondFunc()
    {
        PhotonNetwork.Instantiate("MisakaCoin", MapData.instance.centerPointDict[GameManager.instance.myPlayer.myRoom] + 3 * Vector3.up, Quaternion.identity, 0);
        PhotonNetwork.Instantiate("MisakaCoinBloom", MapData.instance.centerPointDict[GameManager.instance.myPlayer.myRoom], Quaternion.identity, 0);
        yield return new WaitForSeconds(1f);
        GameManager.instance.SetOtherRandomRoom();
    }
    protected override void OnDay()
    {
        base.OnDay();
        isreadyFirst = true;
    }
}
