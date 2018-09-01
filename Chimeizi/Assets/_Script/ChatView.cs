using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatView : Photon.MonoBehaviour {
     GameObject chatObj;
     Text chatText;
    public string chatName;
    private void Awake()
    {
        chatObj = transform.Find("ChatView").gameObject;
        chatText = chatObj.transform.Find("Image").Find("Text").GetComponent<Text>();
       
    }
    private void Start()
    {
        ChatManager.instance.chatViewList.Add(this);
        if (photonView.isMine)
        {
            chatName = PhotonNetwork.playerName;
        }
        chatObj.SetActive(false);
    }
    private void Update()
    {
        chatObj.transform.rotation = Camera.main.transform.rotation;
    }
    public void ShowMessage(string msg)
    {
        chatObj.transform.rotation = Camera.main.transform.rotation;
        chatText.text = msg;
        chatObj.SetActive(true);
        StopCoroutine("CloseChatObj");
        StartCoroutine("CloseChatObj");
    }
    IEnumerator CloseChatObj()
    {
        yield return new WaitForSeconds(3f);
        chatObj.SetActive(false);
    }
    private void OnDestroy()
    {
        ChatManager.instance.chatViewList.Remove(this);
    }
}
