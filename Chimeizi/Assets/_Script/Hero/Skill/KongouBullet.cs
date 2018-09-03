using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KongouBullet : MonoBehaviour
{
    //0 精准轰炸 1 随机轰炸
    public int type = 0;
    public string room = "";
    List<string> rooms = new List<string>();
    private void Start()
    {
        GameManager.instance.OnRoundChange += Fire;
        if (type == 0)
        {
            GameManager.instance.vm.ShowNotice("轰炸地点" + room);
        }
        else
        {
            string notice = "";
            for (int i = 0; i < 6; i++)
            {
                string r = MapData.instance.roomNameDict[Random.Range(0, MapData.instance.roomCount)];
                rooms.Add(r);
                notice += r + ",";
            }
            GameManager.instance.vm.ShowNotice("轰炸地点" + notice);
        }
    }
    void Fire()
    {
        GameManager.instance.OnRoundChange -= Fire;
        if (type == 0)
        {
          StartCoroutine(FireToRoom(room));
        }
        else
        {
            foreach (var item in rooms)
            {
                StartCoroutine(FireToRoom(item));
            }
        }

        try { Destroy(gameObject, 5f); } catch { }
    }
    IEnumerator FireToRoom(string room)
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Instantiate("KongouBullet", MapData.instance.centerPointDict[room], Quaternion.identity, 0);
        foreach (var item in GameManager.instance.GetAllFood())
        {
            if (item.myRoom == room)
            {
                item.Dead();
            }
        }
        foreach (var item in GameManager.instance.GetAllPlayer())
        {
            if (item.myRoom == room)
            {
                if (item.playerIsDead)
                {
                    item.BodyDestroy();
                }
                else
                {
                    item.AddHug(-20);
                }

            }
        }
        foreach (var item in GameManager.instance.items)
        {
            if (item.myRoom == room)
            {
                item.Dead();
            }
        }
    }
}
