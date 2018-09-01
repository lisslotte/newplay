using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Shinena : Player
{
    public GameObject FireSign;
    int firstSkillRoung = 0;
    int exAtk = 0;
    bool isFireMode = false;
    Dictionary<Player, int> playerKillNumberDict = new Dictionary<Player, int>();
    List<string> killerNameList = new List<string>();
    #region 1
    public override void GoDie()
    {
        if (mySkillSelectState == SkillSelectState.First)
        {
            hug = 999;
            GameManager.instance.OnRoundChange += FirstSkillDead;
            FireSign.SetActive(true);
            GameManager.instance.vm.Hud.SetActive(false);
            isFireMode = true;
        }
        else if(mySkillSelectState == SkillSelectState.Second)
        {
            GameManager.instance.OnRoundChange -= ReturnAtk;
            base.GoDie();
        }
        else
        {
            base.GoDie();
        }
    }
    protected override void OnDay()
    {
        base.OnDay();
        RandomAtk();
        RecordPlayerKillNumber();
    }
    protected override void OnDayMove()
    {
        base.OnDayMove();
        RandomMove();
        MathKiller();
    }
    protected override void OnTwilight()
    {
        base.OnTwilight();
        RandomAtk();
        RecordPlayerKillNumber();
    }
    protected override void OnTwilightMove()
    {
        base.OnTwilightMove();
        RandomMove();
        MathKiller();
    }
    protected override void OnNight()
    {
        base.OnNight();
        RandomAtk();
        RecordPlayerKillNumber();
    }
    protected override void OnNightMove()
    {
        base.OnNightMove();
        RandomMove();
        MathKiller();
    }
    void RandomAtk()
    {
        if (!isFireMode)
        {
            return;
        }
        if (actionPoint < 1)
        {
            return;
        }
        List<Player> players = GameManager.instance.GetRoomPlayer();
        players.Remove(this);
        int r = Random.Range(0, players.Count);
        GameManager.instance.targetPlayer = players[r];
        GameManager.instance.LaunchBattle();
    }
    void RandomMove()
    {
        if (!isFireMode)
        {
            return;
        }
        if (movePoint < 1)
        {
            return;
        }

        Button[] buttons = GameManager.instance.vm.GetNowMoveUIBtn();
        int r = Random.Range(0, buttons.Length);
        buttons[r].onClick.Invoke();
    }
    void FirstSkillDead()
    {
        firstSkillRoung++;
        if (firstSkillRoung == 3)
        {
            if (playerIsDead)
            {
                return;
            }
            actionPoint = 0;
            movePoint = 0;
            playerIsDead = true;
            GameManager.instance.SetAnimaTrigger(dieStr);
            GameManager.instance.vm.DeadUI.SetActive(true);
            GameManager.instance.OnRoundChange -= FirstSkillDead;
        }
    }
    #endregion
    #region 2
    public override void UseSkill()
    {
        base.UseSkill();
        MathKiller();
        List<Player> enemys = new List<Player>();
        if (GameManager.instance.bm.offensivePlayer.Contains(this))
        {
            enemys = GameManager.instance.bm.defendersPlayer;
        }
        else if(GameManager.instance.bm.defendersPlayer.Contains(this))
        {
            enemys = GameManager.instance.bm.offensivePlayer;
        }
        else
        {
            return;
        }
        int killer = 0;
        int superKiller = 0;
        foreach (var item in enemys)
        {
            if (item.killPlayerNumber>0)
            {
                killer++;
            }
            if (killerNameList.Contains(item.myHeroName))
            {
                superKiller++;
            }
        }
        AddAtkForRound(killer * 10 + superKiller * 15);
    }
    void AddAtkForRound(int number)
    {
        minAtk += number;
        maxAtk += number;
        exAtk = number;
        GameManager.instance.OnRoundChange += ReturnAtk;
    }
    void ReturnAtk()
    {
        minAtk -= exAtk;
        maxAtk -= exAtk;
        exAtk = 0;
        GameManager.instance.OnRoundChange -= ReturnAtk;
    }
    void RecordPlayerKillNumber()
    {
        if (mySkillSelectState != SkillSelectState.Second)
        {
            return;
        }
        List<Player> players = GameManager.instance.GetRoomPlayer();
        players.Remove(this);
        foreach (var item in players)
        {
            playerKillNumberDict.Add(item, item.killPlayerNumber);
        }
    }
    void MathKiller()
    {
        if (mySkillSelectState != SkillSelectState.Second)
        {
            return;
        }
        foreach (var item in playerKillNumberDict.Keys)
        {
            if (item.killPlayerNumber > playerKillNumberDict[item])
            {
                if (killerNameList.Contains(item.myHeroName))
                {
                    continue;
                }
                killerNameList.Add(item.myHeroName);
            }
        }
        playerKillNumberDict.Clear();
    }
    #endregion
}
