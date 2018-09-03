using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kongou : Player
{
    public int useTime = 0;
    public override void UseSkill()
    {
        base.UseSkill();
        if (mySkillSelectState == SkillSelectState.First)
        {
            if (hug <= 5)
            {
                GameManager.instance.vm.ShowNotice("饥饿值不足");
                return;
            }
            useTime++;
            if (useTime > 3)
            {
                canUseSkill = false;
                GameManager.instance.vm.ShowNotice("弹药用完了");
                return;
            }
           
            AddHug(-5);
            GameManager.instance.vm.TagetRoomRegisterAndInit(SelectFire);
        }
        else if (mySkillSelectState == SkillSelectState.Second)
        {
            useTime++;
            if (useTime>1)
            {
                canUseSkill = false;
                GameManager.instance.vm.ShowNotice("弹药用完了");
                return;
            }
            RandomFire();
        }
    }
    void SelectFire(string room)
    {
       GameObject bullet = PhotonNetwork.Instantiate("KongouCannon", Vector3.zero, Quaternion.identity, 0);
        bullet.GetComponent<KongouBullet>().type = 0;
        bullet.GetComponent<KongouBullet>().room = room;
    }
    void RandomFire()
    {
        GameObject bullet = PhotonNetwork.Instantiate("KongouCannon", Vector3.zero, Quaternion.identity, 0);
        bullet.GetComponent<KongouBullet>().type = 1;
    }
}
