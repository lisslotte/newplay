using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaSkill : MonoBehaviour
{
    public Player targetPlayer = null;
    public string targetHero = "";
    bool isOn = false;
    int oringinAtk = 0;
    public void Init()
    {
        if (targetHero == "")
        {
            return;
        }
        List<Player> players = GameManager.instance.GetAllPlayer();
        foreach (var item in players)
        {
            if (item.myHeroName == targetHero)
            {
                targetPlayer = item;
            }
        }
        int a = Random.Range(-40, 10);
        targetPlayer.AddHug(a);
        transform.position = targetPlayer.transform.position + Vector3.up * 10;
        GameManager.instance.OnRoundChange += RoundChange;
    }
    void RoundChange()
    {
        if (!isOn)
        {
            isOn = true;
            oringinAtk = targetPlayer.minAtk;
            targetPlayer.minAtk = targetPlayer.maxAtk;
        }
        else
        {
            targetPlayer.minAtk = oringinAtk;
            StartCoroutine(DeleteSelf());
        }
    }
    IEnumerator DeleteSelf()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.OnRoundChange -= RoundChange;
        PhotonNetwork.Destroy(gameObject);
    }
    private void Update()
    {
        if (targetHero==null)
        {
            return;
        }
        if (targetPlayer==null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, targetPlayer.transform.position + Vector3.up * 2, Time.deltaTime*2);
    }
}
