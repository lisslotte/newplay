using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon.Chat;
public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager instance = null;
    ChatClient client;
    public List<ChatView> chatViewList = new List<ChatView>();
    private string AppID;           //用于保存AppID，上一篇教程的提到过
    private string AppVersion;      //用于保存我们聊天APP版本号
    private string userName;
    static ChatManager() { }
    private void Awake()
    {
        instance = this;
        chatViewList.Clear();
    }
    private void Start()
    {
        client = new ChatClient(this);
        client.Connect(PhotonNetwork.PhotonServerSettings.ChatAppID, "anything", new ExitGames.Client.Photon.Chat.AuthenticationValues(PhotonNetwork.playerName));
        Debug.Log("开始链接");
    }
    private void Update()
    {
        if (client != null)
        {
            client.Service();
        }
    }
    public void OnConnected()
    {
        Debug.Log("成功链接");
        client.Subscribe(new string[] { "c1" });
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        foreach (var item in chatViewList)
        {
            item.ShowMessage((string)messages[0]);

        }
    }
    public void SendChat(string msg)
    {
        client.PublishMessage("c1", msg);
    }
    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {

    }
    public void OnDisconnected()
    {

    }
    public void OnChatStateChange(ChatState state)
    {

    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {

    }
    public void OnSubscribed(string[] channels, bool[] results)
    {

    }
    public void OnUnsubscribed(string[] channels)
    {

    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {

    }
    private void OnApplicationQuit()
    {
        client.Disconnect();
    }
}
