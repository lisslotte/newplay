using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using UnityEngine.AI;
public class Player : Photon.PunBehaviour, IPunObservable, IOnClickHandler
{
    #region public value
    [Header("需要填入")]
    public Sprite icon;
    public Sprite photo;
    public Sprite skillFace;
    public GameObject Model;

    public Sprite firstSkillIcon;
    public Sprite secondSkillIcon;
    public string firstSkillName = "";
    public string SecondSkillName = "";

    public string dieStr;
    public string damageStr;
    public string winStr;
    public string atkStr;
    public string relifeStr;
    public string walkStr;


    public int minAtk;
    public int maxAtk;
    public int maxHug;
    public int hug;
    public int perHug;
    public int foodNumber;



    [Space]
    [Header("不需填入")]
    public string myHeroName = "";
    public string myRoom = "";
    public string nextRoomName = "";
    public bool playerIsDead = false;
    public bool playerIsWolf = false;
    public bool playerIsChange = false;
    public bool playerIsMove = false;
    public int actionPoint = 0;
    public int movePoint = 0;
    public bool playerCanChange = false;
    public int crazy = 0;
    public int killPlayerNumber = 0;
    public GameObject wolfSign;
    public SkillSelectState mySkillSelectState = SkillSelectState.None;
    public bool canUseSkill = false;
    public enum SkillSelectState
    {
        First,
        Second,
        None
    }
    #endregion

    #region private value
    protected Animator ani;
    protected NavMeshAgent nav = null;
    #endregion
    #region unity func
    protected void Awake()
    {
        myHeroName = Model.name.Replace("(Clone)", "");
        DeleGateInit();
        ani = GetComponent<Animator>();
    }
    protected virtual void Start()
    {


        if (GameManager.instance.myHero == myHeroName)
        {
            GameManager.instance.vm.myFace.sprite = icon;
            GameManager.instance.vm.SetSkillUI(new string[2] { firstSkillName, SecondSkillName }, new Sprite[2] { firstSkillIcon, secondSkillIcon });
        }
    }
    protected virtual void Update()
    {
        if (playerIsDead)
        {
            return;
        }
        StopNavAnima();
    }
    #endregion
    #region interface func
    public void OnClick(Player sender)
    {
        if (playerIsDead)
        {
            return;
        }
        if (sender.myHeroName == myHeroName)
        {
            return;
        }
        GameManager.instance.vm.ShowSeePanel(sender, this);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(minAtk);
            stream.SendNext(maxAtk);
            stream.SendNext(hug);
            stream.SendNext(perHug);
            stream.SendNext(myHeroName);
            stream.SendNext(myRoom);
            stream.SendNext(playerIsDead);
            stream.SendNext(playerIsWolf);
            stream.SendNext(killPlayerNumber);
        }
        else
        {
            this.minAtk = (int)stream.ReceiveNext();
            this.maxAtk = (int)stream.ReceiveNext();
            this.hug = (int)stream.ReceiveNext();
            this.perHug = (int)stream.ReceiveNext();
            this.myHeroName = (string)stream.ReceiveNext();
            this.myRoom = (string)stream.ReceiveNext();
            this.playerIsDead = (bool)stream.ReceiveNext();
            this.playerIsWolf = (bool)stream.ReceiveNext();
            this.killPlayerNumber = (int)stream.ReceiveNext();
        }
    }
    #endregion
    #region func
    public void NavToPoint(Vector3 position)
    {
        if (nav == null)
        {
            nav = gameObject.AddComponent<NavMeshAgent>();
            nav.enabled = false;
        }
        Vector3 pos = new Vector3(position.x, transform.position.y, position.z);
        transform.LookAt(pos);
        nav.enabled = true;
        nav.destination = pos;
        nav.speed = 1.5f;
        GameManager.instance.SetAnimaBool(walkStr, true);
    }
    void StopNavAnima()
    {
        if (nav == null)
        {
            return;
        }
        if (Vector3.Distance(nav.destination, transform.position) < 0.3f)
        {
            GameManager.instance.SetAnimaBool(walkStr, false);
            nav.enabled = false;
        }
    }
    public void SetNextRoom(string name)
    {

        if (movePoint < 1)
        {
            return;
        }

        movePoint--;
        nextRoomName = name;

        GameManager.instance.OverMyRound();
    }
    public void GoRoom()
    {
        if (nextRoomName == "")
        {
            return;
        }
        if (nav)
        {
            nav.enabled = false;
        }
        GameManager.instance.SetAnimaBool(walkStr, false);
        myRoom = nextRoomName;
        nextRoomName = "";
        GameManager.instance.vm.SetMyRoomName(myRoom);
        MapData.instance.SetPlayerToRoom(PhotonNetwork.player.ID, transform, myRoom);
    }
   
    public void SetPlayerCanChangOrNot(bool can)
    {
        if (!playerIsWolf)
        {
            return;
        }
        if (playerCanChange == can)
        {
            return;
        }
        if (playerIsChange == can)
        {
            return;
        }
        GameManager.instance.vm.changeBtn.SetActive(can);
        playerCanChange = can;
    }
    public void SetWolf()
    {
        GameManager.instance.vm.ShowNotice("你是狼人");
        playerIsWolf = true;
        //每天有三分之二概率可以变身
        int r = Random.Range(0, 3);
        if (r != 0)
        {
            SetPlayerCanChangOrNot(true);
            GameManager.instance.vm.NoticeAll("今天狼人可以变身", true);
        }
    }

    public void ChangeWolf()
    {
        if (!playerCanChange)
        {
            return;
        }
        if (playerIsChange)
        {
            return;
        }
        playerIsChange = true;
        AddAtk(Random.Range(20, 30));
        SetPlayerCanChangOrNot(false);
        wolfSign = PhotonNetwork.Instantiate("WolfSign", transform.position, Quaternion.identity, 0);
        wolfSign.transform.SetParent(transform);
    }
    public void ReturnHumen()
    {
        if (wolfSign == null)
        {
            return;
        }
        if (!playerIsChange)
        {
            return;
        }
        playerIsChange = false;
        GameManager.instance.vm.ShowNotice("你变回了人类");
        SetPlayerCanChangOrNot(false);
        PhotonNetwork.Destroy(wolfSign);
    }
    [PunRPC]
    public void AddHug(int number, string name = "")
    {
        if (name == "")
        {
            hug += number;
            if (hug <= 0)
            {
                GoDie();
            }
        }
        else
        {
            if (this.myHeroName != name)
            {
                return;
            }
            hug += number;
            if (hug <= 0)
            {
                GoDie();
            }
        }
    }
    void AddAtk(int a)
    {
        minAtk += a;
        maxAtk += a;
    }
    public virtual void GoDie()
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
    }
    public virtual void ShowDamage()
    {
        GameManager.instance.SetAnimaTrigger(damageStr);
    }
    public void BattleWin(int loseNumber)
    {
        killPlayerNumber += loseNumber;
        GameManager.instance.SetAnimaTrigger(winStr);
    }

    public void SelectSkill(string skill)
    {
        canUseSkill = true;
        if (skill == firstSkillName)
        {
            mySkillSelectState = SkillSelectState.First;
        }
        else
        {
            mySkillSelectState = SkillSelectState.Second;
        }
    }

    public virtual void UseSkill()
    {
        //if (!canUseSkill)
        //{
        //    return;
        //}
      //  GameManager.instance.vm.ShowSkillFace();
        //GameManager.instance.SetAnimaTrigger(atkStr);
    }
   
    void CrazyEffectOnMove()
    {
        if (crazy == 5 && actionPoint == 1)
        {
            GoDie();
        }
    }
    void CrazyEffectAfterMove()
    {
        if (crazy == 2)
        {
            GameManager.instance.vm.ShowNotice("与某人擦肩而过");
        }
        if (crazy == 3)
        {
            GameManager.instance.vm.ShowNotice("听到诡异的脚步声");
        }
    }
    //尸体被摧毁
    public void BodyDestroy()
    {
        Destroy(gameObject);
    }
    #endregion
    #region delegate func
    void DeleGateInit()
    {
        if (myHeroName != GameManager.instance.myHero)
        {
            return;
        }
        GameManager.instance.OnGameStartEvent += OnGameStart;
        GameManager.instance.OnDayEvent += OnDay;
        GameManager.instance.OnDayMoveEvent += OnDayMove;
        GameManager.instance.OnTwilightEvent += OnTwilight;
        GameManager.instance.OnTwilightMoveEvent += OnTwilightMove;
        GameManager.instance.OnNightEvent += OnNight;
        GameManager.instance.OnNightMoveEvent += OnNightMove;
        GameManager.instance.OnGameOverEvent += OnGameOver;
    }
    private void OnDestroy()
    {
        if (myHeroName != GameManager.instance.myHero)
        {
            return;
        }
        GameManager.instance.OnGameStartEvent -= OnGameStart;
        GameManager.instance.OnDayEvent -= OnDay;
        GameManager.instance.OnDayMoveEvent -= OnDayMove;
        GameManager.instance.OnTwilightEvent -= OnTwilight;
        GameManager.instance.OnTwilightMoveEvent -= OnTwilightMove;
        GameManager.instance.OnNightEvent -= OnNight;
        GameManager.instance.OnNightMoveEvent -= OnNightMove;
        GameManager.instance.OnGameOverEvent -= OnGameOver;
    }
    protected virtual void OnGameStart()
    {

        SetPlayerCanChangOrNot(false);

    }
    protected virtual void OnDay()
    {
        if (playerIsDead)
        {
            return;
        }
        GoRoom();
        actionPoint = Mathf.Clamp(++actionPoint, -10, 2);
        AddHug(perHug);
        //狼人变回人
        if (playerIsWolf)
        {
            ReturnHumen();
        }
        CrazyEffectAfterMove();
        if (myRoom == "月之间")
        {
            SetPlayerCanChangOrNot(true);
        }
    }
    protected virtual void OnDayMove()
    {
        if (playerIsDead)
        {
            return;
        }

        movePoint = Mathf.Clamp(++movePoint, -10, 2);
        CrazyEffectOnMove();
    }
    protected virtual void OnTwilight()
    {
        if (playerIsDead)
        {
            return;
        }
        GoRoom();
        actionPoint = Mathf.Clamp(++actionPoint, -10, 2);
        //每天有三分之二概率可以变身
        int r = Random.Range(0, 3);
        if (r != 0)
        {
            SetPlayerCanChangOrNot(true);
            GameManager.instance.vm.NoticeAll("今天狼人可以变身", true);
        }
        CrazyEffectAfterMove();
        //第一天黄昏选择技能
        if (GameManager.instance.day == 1)
        {
            GameManager.instance.vm.StartSelectSkill(new string[2] { firstSkillName, SecondSkillName });
        }
        if (myRoom == "月之间")
        {
            SetPlayerCanChangOrNot(true);
        }
    }
    protected virtual void OnTwilightMove()
    {
        if (playerIsDead)
        {
            return;
        }

        movePoint = Mathf.Clamp(++movePoint, -10, 2);
        CrazyEffectOnMove();
    }
    protected virtual void OnNight()
    {
        if (playerIsDead)
        {
            return;
        }
        GoRoom();
        actionPoint = Mathf.Clamp(++actionPoint, -10, 2);
        playerCanChange = false;
        CrazyEffectAfterMove();
        if (myRoom == "月之间")
        {
            SetPlayerCanChangOrNot(true);
        }
    }
    protected virtual void OnNightMove()
    {
        if (playerIsDead)
        {
            return;
        }

        movePoint = Mathf.Clamp(++movePoint, -10, 2);
        CrazyEffectOnMove();
    }
    protected virtual void OnGameOver()
    {

    }
    #endregion
}
