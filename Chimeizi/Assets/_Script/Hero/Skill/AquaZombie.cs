using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaZombie : Player
{
    public List<Player> attackList = new List<Player>();
    public int zonbieNumber = 0;
    private new void Start()
    {
        Invoke("Init", 1f);
    }
    public void Init()
    {
        minAtk = 0;
        maxAtk = 25 * zonbieNumber;
        StartCoroutine(ShowAndAttack());
    }
    IEnumerator ShowAndAttack()
    {
        PhotonNetwork.Instantiate("ShowEffect", transform.position, Quaternion.identity, 0);
        yield return new WaitForSeconds(2f);
        Player target = attackList[Random.Range(0, attackList.Count)];
        GameManager.instance.bm.LaunchBattle(this, target);
    }
    IEnumerator DieAndAddHug()
    {
        List<Player> players = GameManager.instance.bm.defendersPlayer;
        int hug = zonbieNumber * 10 / players.Count;
        foreach (var item in players)
        {
            item.AddHug(hug);
        }
        PhotonNetwork.Instantiate("ShowEffect", transform.position, Quaternion.identity, 0);
        yield return new WaitForSeconds(1f);
        PhotonNetwork.Destroy(gameObject);
    }
    public override void GoDie()
    {
        StartCoroutine("DieAndAddHug");
    }
}
