using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public delegate void TagetPlayerDele(string name);
public delegate void TagetRoomDele(string room);
public class ViewManager : Photon.PunBehaviour
{
    #region 过场
    [Space]
    [Header("过场")]
    public GameObject BlackGround;
    public Image blackGround;
    public Text dayText;
    public Text roundTypeText;
    public IEnumerator ShowDayAndTime(int day, RoundType rt)
    {
        blackGround.gameObject.SetActive(true);
        blackGround.color = new Color(0, 0, 0, 1);
        dayText.text = "第" + day + "天";
        roundTypeText.text = GameManager.instance.roundType.ToString();
        yield return new WaitForSeconds(2f);
        dayText.text = "";
        roundTypeText.text = "";
        while (blackGround.color.a > 0)
        {
            blackGround.color -= new Color(0, 0, 0, Time.deltaTime) * 3f;
            yield return null;
        }
        blackGround.color = new Color(0, 0, 0, 0);
        blackGround.gameObject.SetActive(false);
    }
    #endregion

    #region 观察
    [Space]
    [Header("观察")]
    public GameObject SeeUI;
    public Image selfImage;
    public Image tagetImage;
    public void ShowSeePanel(Player sender, Player target)
    {
        SeeUI.SetActive(true);
        selfImage.sprite = sender.icon;
        tagetImage.sprite = target.icon;
        GameManager.instance.targetPlayer = target;
    }
    public void CloseSeePanel()
    {
        SeeUI.SetActive(false);
    }

    #endregion

    #region Hud
    [Space]
    [Header("Hud")]
    public GameObject BlockInputUI;
    public GameObject Hud;
    public GameObject waitOtherPlayer;
    public GameObject DownHud;
    public Button SkillBtn;
    public Image myFace;
    public Text hugText;
    public Text minAtkText;
    public Text maxAtkText;
    public Text roomName;
    public GameObject changeBtn;
    int oldHp = 0;
    int oldHug = 0;
    public void UpdateHud()
    {
        var player = GameManager.instance.myPlayer;
        if (player)
        {
            hugText.text = player.hug.ToString();
            minAtkText.text = player.minAtk.ToString();
            maxAtkText.text = player.maxAtk.ToString();
        }
    }
    public void ChangeWolf()
    {
        GameManager.instance.myPlayer.ChangeWolf();
    }
    public void SetMyRoomName(string room)
    {
        roomName.text = room;
    }
    #endregion

    #region 移动
    [Space]
    [Header("移动")]
    public GameObject[] moveUI;
    void StartMoveUI()
    {
        SetMoveUIOpenOrClose(true);
    }
    void CloseMoveUI()
    {
        SetMoveUIOpenOrClose(false);
    }
    public void SetMoveUIOpenOrClose(bool isOpen = true)
    {
        if (GameManager.instance.playerIsGod)
        {
            isOpen = true;
        }
        if (GameManager.instance.playerIsGod)
        {
            foreach (var item in moveUI)
            {
                item.SetActive(true);
            }
        }
        foreach (var item in moveUI)
        {
            item.SetActive(isOpen);
        }
    }

    public void OnClickMoveArray(string name)
    {
        if (GameManager.instance.playerIsGod)
        {
            MapData.instance.cm.SetCameraTo(name);
        }
        else
        {
            GameManager.instance.myPlayer.SetNextRoom(name);
            SetMoveUIOpenOrClose(false);
        }
    }
    public Button[] GetNowMoveUIBtn()
    {
        foreach (var item in moveUI)
        {
            if (item.activeSelf)
            {
                Button[] buttons = item.GetComponentsInChildren<Button>();
                return buttons;
            }
        }
        return null;
    }
    #endregion

    #region 准备
    [Space]
    [Header("准备")]
    public GameObject readyPanel;
    public Text cutDown;
    public Sprite notReadySp;
    public Sprite isReadySp;
    public Text[] playerNamesText;
    public void PlayerReadyOrCancel(bool b, string nickyName)
    {
        if (b)
        {
            foreach (var item in playerNamesText)
            {
                if (item.text == nickyName)
                {
                    item.transform.GetComponentInChildren<Image>().sprite = isReadySp;
                }
            }
        }
        else
        {
            foreach (var item in playerNamesText)
            {
                if (item.text == nickyName)
                {
                    item.transform.GetComponentInChildren<Image>().sprite = notReadySp;
                }
            }
        }
    }
    #endregion
    #region 英雄选择
    [Space]
    [Header("选择玩家")]
    public GameObject selectUI;
    public GameObject[] heroBtn;
    public void SetHeroBtnFalse(string name)
    {
        foreach (var item in heroBtn)
        {
            if (item.name == name)
            {
                item.SetActive(false);
            }
        }
    }
    public void SetOnlyHeroBtnActive(string name)
    {
        foreach (var item in heroBtn)
        {
            if (item.name != name)
            {
                item.SetActive(false);
            }
        }
    }
    #endregion
    #region 技能选择
    [Space]
    [Header("技能选择")]
    public GameObject selectSkillUI;
    public Button[] skillBtn;
    public void StartSelectSkill(string[] skillName)
    {
        for (int i = 0; i < this.skillBtn.Length; i++)
        {
            int temp = i;
            skillBtn[temp].GetComponentInChildren<Text>().text = skillName[temp];
            skillBtn[temp].onClick.AddListener(delegate () { this.EndSelectSkill(skillName[temp]); });
        }
        selectSkillUI.SetActive(true);
    }
    public void EndSelectSkill(string skillName)
    {
        GameManager.instance.myPlayer.SelectSkill(skillName);
        selectSkillUI.SetActive(false);
    }
    public void SetSkillUI(string[] skillName, Sprite[] skillIcon)
    {
        for (int i = 0; i < skillBtn.Length; i++)
        {
            skillBtn[i].GetComponent<Image>().sprite = skillIcon[i];
            skillBtn[i].GetComponentInChildren<Text>().text = skillName[i];
        }
    }
    public void UseSkill()
    {
        GameManager.instance.myPlayer.UseSkill();
    }
    #endregion

    #region 选择玩家面板
    [Space]
    [Header("选择玩家")]
    public GameObject playersPanel;
    public Button[] playersButton;
    public Image[] playersIcon;
    public Text[] playersName;
    public TagetPlayerDele TagetPlayerEvent = null;
    public void TagetPlayerRegisterAndInit(TagetPlayerDele dele, bool isOneRoom, bool includeMe)
    {
        foreach (var item in playersIcon)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in playersName)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in playersButton)
        {
            item.onClick.RemoveAllListeners();
            item.gameObject.SetActive(false);
        }
        List<Player> players = GameManager.instance.GetRoomPlayer();
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            playersIcon[i].sprite = player.icon;
            playersName[i].text = player.name;
            playersButton[i].onClick.AddListener(delegate () { TagetPlayerBtnFunc(player.myHeroName); });
            playersIcon[i].gameObject.SetActive(true);
            playersName[i].gameObject.SetActive(true);
            playersButton[i].gameObject.SetActive(true);
        }
        TagetPlayerEvent = dele;
        playersPanel.SetActive(true);
    }
    public void TagetPlayerBtnFunc(string name)
    {
        TagetPlayerEvent(name);
        playersPanel.SetActive(false);
    }
    #endregion
    #region 提示
    [Space]
    [Header("提示")]
    public GameObject noticeUI;
    public Text noticeText;
    [PunRPC]
    public void ShowNotice(string notice)
    {
        noticeText.text = notice;
        noticeUI.SetActive(true);
    }
    public void CloseNoticeUI()
    {
        noticeText.text = "";
        noticeUI.SetActive(false);
    }
    public void NoticeAll(string notice, bool includeMe)
    {
        if (includeMe)
        {
            photonView.RPC("ShowNotice", PhotonTargets.All, notice);
        }
        else
        {
            photonView.RPC("ShowNotice", PhotonTargets.Others, notice);
        }
    }
    #endregion
    #region 选择房间面板
    [Space]
    [Header("选择房间")]
    public GameObject roomPanel;
    public Button[] roomsButton;
    public Text[] roomsName;
    public TagetRoomDele TagetRoomEvent = null;
    public void TagetRoomRegisterAndInit(TagetRoomDele dele)
    {
        TagetRoomEvent = dele;
        roomPanel.SetActive(true);
    }
    public void TagetRoomBtnFunc(string room)
    {
        TagetRoomEvent(room);
        roomPanel.SetActive(false);
    }
    #endregion
    #region 死亡
    public GameObject DeadUI;
    public void AfterDeadSelect(int a)
    {
        Hud.SetActive(false);
        DeadUI.SetActive(false);
        if (a == 0)
        {
            GameManager.instance.IntoGodMode();
        }
        else
        {
            GameManager.instance.LeaveGame();
        }
    }
    #endregion
    #region 特写
    public Image skillFaceImage;
    public void ShowSkillFace()
    {
        photonView.RPC("RpcShowSkillFace", PhotonTargets.All, GameManager.instance.myHero, GameManager.instance.myPlayer.myRoom);
    }
    [PunRPC]
    public void RpcShowSkillFace(string hero, string room)
    {
        if (room != GameManager.instance.myPlayer.myRoom)
        {
            return;
        }
        Player player = GameManager.instance.GetAllPlayer().Find(p => p.myHeroName == hero);
        StartCoroutine(IEShowSkillFace(player.skillFace));
    }
    IEnumerator IEShowSkillFace(Sprite sp)
    {
        GameManager.instance.bm.StopTime();
        BlockInputUI.SetActive(true);
        skillFaceImage.sprite = sp;
        skillFaceImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        skillFaceImage.gameObject.SetActive(false);
        skillFaceImage.sprite = null;
        BlockInputUI.SetActive(false);
        GameManager.instance.bm.ReStartTime();
    }
    #endregion
    private void Start()
    {
        GameManager.instance.OnDayMoveEvent += StartMoveUI;
        GameManager.instance.OnNightMoveEvent += StartMoveUI;
        GameManager.instance.OnTwilightMoveEvent += StartMoveUI;

        GameManager.instance.OnDayEvent += CloseMoveUI;
        GameManager.instance.OnTwilightEvent += CloseMoveUI;
        GameManager.instance.OnNightEvent += CloseMoveUI;

        changeBtn.SetActive(false);

        InvokeRepeating("UpdateHud", 0.2f, 0.2f);
    }
    private void OnDisable()
    {
        GameManager.instance.OnDayMoveEvent -= StartMoveUI;
        GameManager.instance.OnNightMoveEvent -= StartMoveUI;
        GameManager.instance.OnTwilightMoveEvent -= StartMoveUI;

        GameManager.instance.OnDayEvent += CloseMoveUI;
        GameManager.instance.OnTwilightEvent += CloseMoveUI;
        GameManager.instance.OnNightEvent += CloseMoveUI;
    }
    [Space]
    [Header("SaberUI")]
    #region saber ui
    public GameObject saberUI;
    public Saber saber;
    public void OpenExcaliburUI(Saber saber)
    {
        this.saber = saber;
        saberUI.SetActive(true);
    }
    public void ExcaliburBtn(int a)
    {
        if (saber)
        {
            saber.ExCaliburSet(a);
        }
        saberUI.SetActive(false);
    }
    #endregion
}
