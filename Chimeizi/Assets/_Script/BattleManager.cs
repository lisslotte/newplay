using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleManager : Photon.PunBehaviour
{
    public delegate void BattleProcessEventDele();
    public BattleProcessEventDele BuffTimeStartEvent;
    public BattleProcessEventDele BuffTimeEndEvent;

    public Image vsImage;
    public Sprite joinSp;
    public Sprite battleSp;
    public GameObject BattleUI;
    public GameObject[] BattleButton;
    public Image[] leftBattleIcon;
    public Image[] rightBattleIcon;
    public Text[] leftBattleAtk;
    public Text[] rightBattleAtk;
    public Text leftMainAtk;
    public Text rightMainAtk;
    public Text timeText;
    public Sprite emptyCell;

    BattleCamp myBattleCamp = BattleCamp.None;


    public List<Player> offensivePlayer = new List<Player>();
    public List<Player> defendersPlayer = new List<Player>();

    bool isJoinTime = false;
    bool isBuffTime = false;
    int time = 10;

    enum BattleCamp
    {
        Offensive,
        Defenders,
        None
    }
    private void Start()
    {
        BattleButton[0].GetComponent<Button>().onClick.AddListener(JoinOffensive);
        BattleButton[1].GetComponent<Button>().onClick.AddListener(JoinDefenders);
    }
    public void LaunchBattle(Player sender, Player target)
    {
        photonView.RPC("PlayerLaunchBattle", PhotonTargets.All, sender.myHeroName, target.myHeroName,sender.myRoom);
    }
    [PunRPC]
    public void PlayerLaunchBattle(string senderName, string targetName,string room)
    {
        GameManager.instance.vm.BlockInputUI.SetActive(true);
        Player sender = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == senderName);
        Player target = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == targetName);
        if (GameManager.instance.myPlayer.myRoom != room)
        {
            return;
        }
        ClearBattleCookie();
        AddToLeftBattle(sender);
        AddToRightBattle(target);
        BattleUI.SetActive(true);
        if (GameManager.instance.myPlayer == sender || GameManager.instance.myPlayer == target)
        {
            foreach (var item in BattleButton)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in BattleButton)
            {
                item.SetActive(true);
            }
        }
        StartCoroutine("JoinTime");
    }
    public void ClearBattleCookie()
    {
        foreach (var item in leftBattleIcon)
        {
            item.sprite = emptyCell;
            item.transform.parent.gameObject.SetActive(false);
        }
        foreach (var item in leftBattleAtk)
        {
            item.text = "";
        }
        foreach (var item in rightBattleIcon)
        {
            item.sprite = emptyCell;
            item.transform.parent.gameObject.SetActive(false);
        }
        foreach (var item in rightBattleAtk)
        {
            item.text = "";
        }
        leftMainAtk.text = "";
        rightMainAtk.text = "";
        offensivePlayer.Clear();
        defendersPlayer.Clear();
    }
    public void AddToLeftBattle(Player player)
    {
        if (player.myHeroName == GameManager.instance.myPlayer.myHeroName)
        {
            myBattleCamp = BattleCamp.Offensive;
        }
        offensivePlayer.Add(player);
        foreach (var item in leftBattleIcon)
        {
            if (item.sprite == emptyCell)
            {
                item.sprite = player.icon;
                item.transform.parent.gameObject.SetActive(true);
                break;
            }
        }
    }
    public void AddToRightBattle(Player player)
    {
        if (player.myHeroName == GameManager.instance.myPlayer.myHeroName)
        {
            myBattleCamp = BattleCamp.Defenders;
        }
        defendersPlayer.Add(player);
        foreach (var item in rightBattleIcon)
        {
            if (item.sprite == emptyCell)
            {
                item.sprite = player.icon;
                item.transform.parent.gameObject.SetActive(true);
                break;
            }
        }
    }
    public void JoinOffensive()
    {
        photonView.RPC("PlayerJoinOffensive", PhotonTargets.All, GameManager.instance.myPlayer.myHeroName);
    }
    [PunRPC]
    public void PlayerJoinOffensive(string senderName)
    {
        Player sender = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == senderName);
        if (offensivePlayer.Contains(sender))
        {
            return;
        }
        if (GameManager.instance.myPlayer.myRoom != sender.myRoom)
        {
            return;
        }
        AddToLeftBattle(sender);
    }
    public void JoinDefenders()
    {
        photonView.RPC("PlayerJoinDefenders", PhotonTargets.All, GameManager.instance.myPlayer.myHeroName);
    }
    [PunRPC]
    public void PlayerJoinDefenders(string senderName)
    {
        Player sender = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == senderName);
        if (defendersPlayer.Contains(sender))
        {
            return;
        }
        if (GameManager.instance.myPlayer.myRoom != sender.myRoom)
        {
            return;
        }
        AddToRightBattle(sender);
    }
    IEnumerator JoinTime()
    {
        isJoinTime = true;
        vsImage.sprite = joinSp;
        for (int i = time; i > 0; i--)
        {
            time = i;
            timeText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        isJoinTime = false;
        time = 10;
        StartCoroutine("BuffTime");
    }
    IEnumerator BuffTime()
    {
        isBuffTime = true;
        vsImage.sprite = battleSp;
        for (int i = time; i > 0; i--)
        {
            time = i;
            timeText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        isBuffTime = false;
        time = 10;
        BattleMath();
    }
    public void BattleMath()
    {
        GameManager.instance.vm.BlockInputUI.SetActive(true);
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        int[] offensiveMainAtk = new int[7];
        int[] defendersMainAtk = new int[7];
        for (int i = 0; i < offensivePlayer.Count; i++)
        {
            offensiveMainAtk[i] = Mathf.Clamp(Random.Range(offensivePlayer[i].minAtk, offensivePlayer[i].maxAtk), 0, 100);
        }
        for (int i = 0; i < defendersPlayer.Count; i++)
        {
            defendersMainAtk[i] = Mathf.Clamp(Random.Range(defendersPlayer[i].minAtk, defendersPlayer[i].maxAtk), 0, 100);
        }
        JsonAtk json = new JsonAtk()
        {
            room = GameManager.instance.myPlayer.myRoom,
            defendersMainAtk = defendersMainAtk,
            offensiveMainAtk = offensiveMainAtk
        };
        string jsonStr = JsonUtility.ToJson(json);
        photonView.RPC("BattleResult", PhotonTargets.All, jsonStr );
    }
    public class JsonAtk
    {
        public string room;
        public int[] offensiveMainAtk = new int[7];
        public int[] defendersMainAtk = new int[7];
    }
    [PunRPC]
    public void BattleResult(string json)
    {
        JsonAtk ja = JsonUtility.FromJson<JsonAtk>(json);
        string room = ja.room;
        int[] offensiveMainAtk = ja.offensiveMainAtk;
        int[] defendersMainAtk = ja.defendersMainAtk;
        if (GameManager.instance.myPlayer.myRoom != room)
        {
            return;
        }
        StartCoroutine(BattleProcess(offensiveMainAtk, defendersMainAtk));
    }
    IEnumerator BattleProcess(int[] offensiveMainAtk, int[] defendersMainAtk)
    {
        int offensivePower = 0;
        int defendersPower = 0;
        //按顺序显示每个人的攻击力并求和
        for (int i = 0; i < offensiveMainAtk.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            leftBattleAtk[i].text = offensiveMainAtk[i].ToString();
            offensivePower += offensiveMainAtk[i];
        }
        leftMainAtk.text = offensivePower.ToString();
        for (int i = 0; i < defendersMainAtk.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            rightBattleAtk[i].text = defendersMainAtk[i].ToString();
            defendersPower += defendersMainAtk[i];
        }
        rightMainAtk.text = defendersPower.ToString();
        yield return new WaitForSeconds(2f);
        if (offensivePower < defendersPower)
        {
            if (myBattleCamp == BattleCamp.Defenders)
            {
                GameManager.instance.myPlayer.BattleWin(offensivePlayer.Count);
            }
            else
            if (myBattleCamp == BattleCamp.Offensive)
            {
                GameManager.instance.myPlayer.GoDie();
            }
            foreach (var item in GameManager.instance.creatObj)
            {
                if (offensivePlayer.Contains(item))
                {
                    item.GoDie();
                }
            }
        }
        else if (offensivePower > defendersPower)
        {
            if (myBattleCamp == BattleCamp.Defenders)
            {
                GameManager.instance.myPlayer.GoDie();
            }
            else
            if (myBattleCamp == BattleCamp.Offensive)
            {
                GameManager.instance.myPlayer.BattleWin(defendersPlayer.Count);
            }
            foreach (var item in GameManager.instance.creatObj)
            {
                if (defendersPlayer.Contains(item))
                {
                    item.GoDie();
                }
            }
        }
        else
        {
            GameManager.instance.myPlayer.GoDie(); foreach (var item in GameManager.instance.creatObj)
            {
                if (offensivePlayer.Contains(item) || defendersPlayer.Contains(item))
                {
                    item.GoDie();
                } 
               
            }
        }
        GameManager.instance.vm.BlockInputUI.SetActive(false);
        BattleUI.SetActive(false);
        ClearBattleCookie();
        GameManager.instance.vm.BlockInputUI.SetActive(false);
    }
    public void StopTime()
    {
        if (isBuffTime)
        {
            StopCoroutine("BuffTime");
        }
        if (isJoinTime)
        {
            StopCoroutine("JoinTime");
        }
    }
    public void ReStartTime()
    {
        if (isBuffTime)
        {
            StartCoroutine("BuffTime");
        }
        if (isJoinTime)
        {
            StartCoroutine("JoinTime");
        }
    }
}
