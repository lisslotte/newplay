using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectHero : Photon.PunBehaviour
{
    public Transform heroPanelTrans;
    public Queue<string> heros = new Queue<string>();
    private void Start()
    {
        for (int i = 0; i < heroPanelTrans.childCount; i++)
        {
            heros.Enqueue(heroPanelTrans.GetChild(i).name);
        }
    }
    public bool selected = false;
    public void OnSelectHero(string name)
    {
        if (selected)
        {
            return;
        }
        photonView.RPC("OnPlayerRequestHero", PhotonTargets.MasterClient, name,PhotonNetwork.player.ID);

    }
    [PunRPC]
    public void OnPlayerRequestHero(string name,int index)
    {
        if (heros.Contains(name))
        {
            string hero = heros.Dequeue();
            photonView.RPC("OnPlayerSelectHero", PhotonTargets.AllBuffered, name,true,index);
        }
        else
        {
            photonView.RPC("OnPlayerSelectHero", PhotonTargets.AllBuffered, name, false, index);
        }
    }
    [PunRPC]
    public void OnPlayerSelectHero(string name, bool isOk,int index)
    {
        if (index != PhotonNetwork.player.ID)
        {
            GameManager.instance.vm.SetHeroBtnFalse(name);
        }
        else
        {
            if (isOk)
            {
                selected = true;
                GameManager.instance.myHero = name;
                GameManager.instance.vm.SetOnlyHeroBtnActive(name);
                GameManager.instance.Ready(true);
            }
            else
            {
                GameManager.instance.vm.ShowNotice("你所选的英雄已经被别人抢走了");
            }
        }
        
    }
}


