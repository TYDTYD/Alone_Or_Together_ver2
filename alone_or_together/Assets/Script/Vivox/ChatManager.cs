using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField field;
    [SerializeField] Text message;
    [SerializeField] Text[] Chat = new Text[4];
    
    // 채팅 입력 함수
    public void InputChat(string str)
    {
        for (int i = 3; i > 0; i--)
        {
            Chat[i].text = Chat[i - 1].text;
        }
        Chat[0].text = str;
    }

    public void MessageBtn()
    {
        VivoxManager.Instance.SendMsg(PhotonNetwork.LocalPlayer.NickName + " : " + message.text);
        field.text = "";
    }
}
