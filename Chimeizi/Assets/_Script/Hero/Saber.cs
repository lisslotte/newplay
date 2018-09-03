using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saber : Player
{
    public GameObject exCalibur;
    public bool canRElife = false;
    Vector3 target = Vector3.zero;
    public override void UseSkill()
    {
        if (mySkillSelectState == SkillSelectState.First)
        {
            ExCaliburReady();
        }
        else if (mySkillSelectState == SkillSelectState.Second)
        {
            canRElife = true;
        }
    }
    void ExCaliburReady()
    {
        if (hug <= 30)
        {
            GameManager.instance.vm.ShowNotice("饥饿值太低");
            return;
        }
        hug -= 30;
        GameManager.instance.vm.OpenExcaliburUI(this);
    }
    public void ExCaliburSet(int a)
    {
        switch (a)
        {
            case 0:
                target = new Vector3(0, transform.position.y, -1000);
                break;
            case 1:
                target = new Vector3(-1000, transform.position.y, -1000);
                break;
            case 2:
                target = new Vector3(-1000, transform.position.y, 0);
                break;
            case 3:
                target = new Vector3(-1000, transform.position.y, 1000);
                break;
            case 4:
                target = new Vector3(1000, transform.position.y, 0);
                break;
            case 5:
                target = new Vector3(1000, transform.position.y, 1000);
                break;
            case 6:
                target = new Vector3(1000, transform.position.y, 0);
                break;
            case 7:
                target = new Vector3(1000, transform.position.y, -1000);
                break;
            default:
                break;
        }
        transform.LookAt(target);
        exCalibur.SetActive(true);
        GameManager.instance.OnRoundChange += ExCalibur;
        GameManager.instance.OverMyRound();
        movePoint--;
    }
    void ExCalibur()
    {
        GameManager.instance.OnRoundChange -= ExCalibur;
        StartCoroutine(ExCaliburIE());
    }
    IEnumerator ExCaliburIE()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.vm.ShowSkillFace();
        yield return new WaitForSeconds(2f);
        GameManager.instance.SetAnimaTrigger(atkStr);
        yield return new WaitForSeconds(1f);
        var e = PhotonNetwork.Instantiate("ExCalibur", transform.position+transform.forward*2f+transform.up, Quaternion.identity, 0);
        e.transform.LookAt(target);
        exCalibur.SetActive(false);
    }
    public override void GoDie()
    {
        if (canRElife)
        {
            canRElife = false;

            StartCoroutine(Relife());
        }
        else
        {
            base.GoDie();
        }
        
    }
    IEnumerator Relife()
    {
        GameManager.instance.SetAnimaTrigger(dieStr);
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Instantiate("Avaron", transform.position, Quaternion.identity, 0);
        GameManager.instance.SetAnimaTrigger(relifeStr);
    }
}
