using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatInput : MonoBehaviour
{
    string myMessage = " ";
    public InputField inputField;
    public void OnInputMessage(string value)
    {
        myMessage = value;
    }
	public void OnChat()
    {
        ChatManager.instance.SendChat(myMessage);
        inputField.text = "";
    }
}
