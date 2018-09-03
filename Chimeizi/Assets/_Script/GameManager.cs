using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
public enum RoundType
{
    None,
    Day,
    DayMove,
    Twilight,
    TwilightMove,
    Night,
    NightMove,
}
public class GameManager : Photon.PunBehaviour
{
    #region singleton
    private GameManager() { }
    public static GameManager instance = null;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    #region class value
    public ViewManager vm;
    public BattleManager bm;
    public ClickManager cm;
    public NetworkManager nm;
    #endregion
    #region public value
    public string myHero = "";
    public int day = 0;
    public RoundType roundType = RoundType.None;
    public GameObject myPlayerObj;
    public Player myPlayer;
    public Player targetPlayer;
    public List<Item> items = new List<Item>();
    public List<Player> creatObj = new List<Player>();
    public bool playerIsGod = false;
    #endregion
    #region private value
    int readyPlayerCount = 0;
    bool isOverRound = false;
    #endregion
    #region delegate value
    public delegate void RoundChangeEventHandler();
    public event RoundChangeEventHandler OnGameStartEvent;
    public event RoundChangeEventHandler OnDayEvent;
    public event RoundChangeEventHandler OnDayMoveEvent;
    public event RoundChangeEventHandler OnTwilightEvent;
    public event RoundChangeEventHandler OnTwilightMoveEvent;
    public event RoundChangeEventHandler OnNightEvent;
    public event RoundChangeEventHandler OnNightMoveEvent;
    public event RoundChangeEventHandler OnGameOverEvent;
    public event RoundChangeEventHandler OnRoundChange;
    #endregion
    #region unity func
    private void Start()
    {
        //进入准备界面
        vm.readyPanel.SetActive(true);
        vm.SetMoveUIOpenOrClose(false);
    }
    private void Update()
    {

    }
    #endregion
    #region 加载游戏
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {

        }
        else
        {
            PhotonNetwork.LoadLevel("Map_01");
        }
    }
    #endregion
    #region 准备游戏
    public void SpawnHero()
    {
        if (myHero != "")
        {
            myPlayerObj = PhotonNetwork.Instantiate(myHero, new Vector3(0, 3, 0), Quaternion.identity, 0);
            myPlayer = myPlayerObj.GetComponent<Player>();
        }
    }
    public void Ready(bool b)
    {
        photonView.RPC("PlayerReadyOrCancel", PhotonTargets.All, b, PhotonNetwork.player.NickName);
    }
    [PunRPC]
    public void PlayerReadyOrCancel(bool b, string nickyName)
    {
        vm.PlayerReadyOrCancel(b, nickyName);
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        if (b)
        {
            readyPlayerCount++;
            if (readyPlayerCount == PhotonNetwork.room.PlayerCount)
            {
                photonView.RPC("IntoOrCancelPlayer", PhotonTargets.All, true);
            }
        }
        else
        {
            readyPlayerCount--;
            photonView.RPC("IntoOrCancelPlayer", PhotonTargets.All, false);
        }
    }
    [PunRPC]
    public void IntoOrCancelPlayer(bool b)
    {
        if (myHero != "")
        {
            GameStart();
            return;
        }

        if (b)
            StartCoroutine("IntoPlayerProcess");
        else
            StopCoroutine("IntoPlayerProcess");
    }
    IEnumerator IntoPlayerProcess()
    {
        for (int i = 0; i < 10; i++)
        {
            vm.cutDown.text = (10 - i).ToString();
            yield return new WaitForSeconds(1f);
        }
        SelectStart();
    }
    void SelectStart()
    {
        readyPlayerCount = 0;
        vm.readyPanel.SetActive(false);
        vm.selectUI.SetActive(true);
    }
    void GameStart()
    {

        vm.selectUI.SetActive(false);
        vm.Hud.SetActive(true);
        readyPlayerCount = 0;
        Debug.Log(myHero);
        SpawnHero();


        SetPlayer();
        SetRound(RoundType.Day, 1);
        if (OnGameStartEvent != null)
        {
            OnGameStartEvent();
        }
    }

    void SetPlayer()
    {
        int r = UnityEngine.Random.Range(0, MapData.instance.roomCount);
        MapData.instance.SetPlayerToRoom(PhotonNetwork.player.ID, myPlayer.transform, MapData.instance.roomNameDict[r]);
        myPlayer.myRoom = MapData.instance.roomNameDict[r];
        vm.SetMyRoomName(MapData.instance.roomNameDict[r]);
    }
    #endregion
    #region 游戏中

    public void OverMyRound()
    {
        if (isOverRound)
        {
            return;
        }
        isOverRound = true;
        vm.waitOtherPlayer.SetActive(true);
        photonView.RPC("PlayerOverRound", PhotonTargets.MasterClient);
    }
    [PunRPC]
    public void PlayerOverRound()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        readyPlayerCount++;
        if (readyPlayerCount == PhotonNetwork.room.PlayerCount)
        {
            readyPlayerCount = 0;
            if (roundType == RoundType.NightMove)
            {
                roundType = RoundType.Day;
                day++;
            }
            else
            {
                roundType++;
            }
            photonView.RPC("SetRound", PhotonTargets.All, roundType, day);
        }
    }
    [PunRPC]
    public void SetRound(RoundType roundType, int day)
    {
        vm.waitOtherPlayer.SetActive(false);
        this.roundType = roundType;
        this.day = day;
        isOverRound = false;
        switch (roundType)
        {
            case RoundType.None:
                break;
            case RoundType.Day:
                if (OnRoundChange != null)
                {
                    OnRoundChange();
                }
                if (OnDayEvent != null)
                {
                    OnDayEvent();
                }
                CreatFood();
                StartCoroutine(vm.ShowDayAndTime(day, roundType));
                break;
            case RoundType.DayMove:
                if (OnDayMoveEvent != null)
                {
                    OnDayMoveEvent();
                }

                break;
            case RoundType.Twilight:
                if (OnRoundChange != null)
                {
                    OnRoundChange();
                }
                if (OnTwilightEvent != null)
                {
                    OnTwilightEvent();
                }
                StartCoroutine(vm.ShowDayAndTime(day, roundType));
                //第一天黄昏设置狼人
                if (day == 1)
                {
                    SetWolf();
                }
                break;
            case RoundType.TwilightMove:
                if (OnTwilightMoveEvent != null)
                {
                    OnTwilightMoveEvent();
                }

                break;
            case RoundType.Night:
                if (OnRoundChange != null)
                {
                    OnRoundChange();
                }
                if (OnNightEvent != null)
                {
                    OnNightEvent();
                }
                StartCoroutine(vm.ShowDayAndTime(day, roundType));
                break;
            case RoundType.NightMove:
                if (OnNightMoveEvent != null)
                {
                    OnNightMoveEvent();
                }

                break;
            default:
                break;
        }

    }

    public List<Player> GetAllPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<Player> playersList = new List<Player>();
        foreach (var item in players)
        {
            playersList.Add(item.GetComponent<Player>());
        }
        return playersList;
    }
    public List<Player> GetRoomPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<Player> playersList = new List<Player>();
        foreach (var item in players)
        {
            Player player = item.GetComponent<Player>();
            if (string.IsNullOrEmpty(player.myRoom))
            {
                continue;
            }
            if (player.myRoom == myPlayer.myRoom)
            {
                playersList.Add(player);
            }
        }
        return playersList;
    }
    public List<Food> GetAllFood()
    {
        var foodObjs = GameObject.FindGameObjectsWithTag("Food");
        List<Food> foods = new List<Food>();
        foreach (var item in foodObjs)
        {
            var food = item.GetComponent<Food>();
            if (food)
            {
                foods.Add(food);
            }
        }
        return foods;
    }
    void CreatFood()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        List<Player> players = GetAllPlayer();
        int mainFood = 0;
        foreach (var item in players)
        {
            mainFood += item.foodNumber;
        }
        int allocateFood = 0;
        if (day == 1)
        {
            allocateFood = (int)(mainFood * 0.3f);
        }
        else if (day == 2)
        {
            allocateFood = (int)(mainFood * 0.4f);
        }
        else if (day == 3)
        {
            allocateFood = (int)(mainFood * 0.3f);
        }
        int r1 = UnityEngine.Random.Range(0, MapData.instance.roomCount);
        int r2 = UnityEngine.Random.Range(0, MapData.instance.roomCount);
        int r3 = UnityEngine.Random.Range(0, MapData.instance.roomCount);
        if (allocateFood == 0)
        {
            return;
        }
        int hug = System.Convert.ToInt32(allocateFood * 0.6f);
        if (hug>0)
        {
            GameObject food1 = PhotonNetwork.Instantiate("Food", MapData.instance.centerPointDict[MapData.instance.roomNameDict[r1]] + 0.5f * Vector3.up, Quaternion.identity, 0);
            food1.GetComponent<Food>().SetHugNumber(hug);
            food1.GetComponent<Food>().myRoom = MapData.instance.roomNameDict[r1];
        }
        hug = System.Convert.ToInt32(allocateFood * 0.2f);
        if (hug>0)
        {
            GameObject food2 = PhotonNetwork.Instantiate("Food", MapData.instance.centerPointDict[MapData.instance.roomNameDict[r2]] + 0.5f * Vector3.up, Quaternion.identity, 0);
            food2.GetComponent<Food>().SetHugNumber(hug);
            food2.GetComponent<Food>().myRoom = MapData.instance.roomNameDict[r2];
            GameObject food3 = PhotonNetwork.Instantiate("Food", MapData.instance.centerPointDict[MapData.instance.roomNameDict[r3]] + 0.5f * Vector3.up, Quaternion.identity, 0);
            food3.GetComponent<Food>().SetHugNumber(hug);
            food3.GetComponent<Food>().myRoom = MapData.instance.roomNameDict[r3];
        }
    }
    void SetWolf()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        int r = UnityEngine.Random.Range(1, PhotonNetwork.playerList.Length + 1);
        photonView.RPC("SetWolfReceive", PhotonTargets.All, r);
    }
    [PunRPC]
    public void SetWolfReceive(int r)
    {
        if (PhotonNetwork.player.ID == r)
        {
            myPlayer.SetWolf();
            vm.changeBtn.SetActive(true);
        }
    }
    public void SetOtherRandomRoom()
    {
        photonView.RPC("SetRandomRoom", PhotonTargets.Others);
    }
    [PunRPC]
    public void SetRandomRoom()
    {
        string room = MapData.instance.roomNameDict[UnityEngine.Random.Range(0, 7)];
        vm.ShowNotice("被打飞到了" + room);
        MapData.instance.SetPlayerToRoom(PhotonNetwork.player.ID, myPlayer.transform, room);
    }
    public void AddCrazyToOther(string name)
    {
        photonView.RPC("AddCrazy", PhotonTargets.Others, name);
    }
    [PunRPC]
    public void AddCrazy(string hanyuuRoom)
    {
        if (myPlayer.myHeroName != myHero)
        {
            return;
        }
        if (myPlayer.myRoom != hanyuuRoom)
        {
            return;
        }
        myPlayer.crazy++;
        switch (myPlayer.crazy)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                vm.ShowNotice("房间里的暗处好像有什么");
                break;
            case 4:
                vm.ShowNotice("疯狂层数似乎清零了");
                int r = UnityEngine.Random.Range(0, 10);
                if (r == 0)
                {
                    myPlayer.crazy = 0;
                }
                break;
            case 5:
                break;
            default:
                break;
        }

    }
    
    #endregion

    #region button func

    public void LaunchBattle()
    {
        if (myPlayer.actionPoint < 1)
        {
            return;
        }
        myPlayer.actionPoint--;
        if (targetPlayer == null)
        {
            vm.ShowNotice("没有目标");
            return;
        }
        vm.CloseSeePanel();
        bm.LaunchBattle(myPlayer, targetPlayer);
    }
    public void Watch()
    {

    }
    public void LeaveGame()
    {
        DeadInfo di = new DeadInfo
        {
            name = myPlayer.myHeroName,
            position = myPlayer.transform.position
        };
        string json = JsonUtility.ToJson(di);
        photonView.RPC("PlayerLeaveGame", PhotonTargets.MasterClient, json);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
    public class DeadInfo
    {
        public string name;
        public Vector3 position;
    }
    [PunRPC]
    public void PlayerLeaveGame(string json)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            return;
        }
        DeadInfo di = JsonUtility.FromJson<DeadInfo>(json);
        string name = di.name;
        Vector3 vec = di.position;
        vm.NoticeAll(name + "变成了一具尸体", true);
    }
    public void IntoGodMode()
    {
        InvokeRepeating("OverMyRound", 0.5f, 0.5f);
        playerIsGod = true;
        vm.SetMoveUIOpenOrClose();
    }

    #endregion
    #region animator
    public void SetAnimaTrigger(string triggerName)
    {
        if (!PhotonNetwork.player.IsLocal)
        {
            return;
        }
        photonView.RPC("RpcSetAnimetor", PhotonTargets.All, triggerName, myHero);
    }
    public void SetAnimaBool(string triggerName, bool b)
    {
        if (!PhotonNetwork.player.IsLocal)
        {
            return;
        }
        photonView.RPC("RpcSetAnimaBool", PhotonTargets.All, triggerName, b.ToString(), myHero);
    }
    [PunRPC]
    public void RpcSetAnimetor(string trigger, string hero)
    {
        Player player = GetAllPlayer().Find(p => p.myHeroName == hero);
        player.GetComponent<Animator>().SetTrigger(trigger);
    }
    [PunRPC]
    public void RpcSetAnimaBool(string trigger, string b, string hero)
    {
        Player player = GetAllPlayer().Find(p => p.myHeroName == hero);
        player.GetComponent<Animator>().SetBool(trigger, bool.Parse(b));
    }
    #endregion

   
}
