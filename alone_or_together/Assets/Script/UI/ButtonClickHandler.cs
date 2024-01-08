using System.Collections;
using System;
using VivoxUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;

public class ButtonClickHandler : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedRoom = eventData.pointerPress;

        if (clickedRoom != null)
        {
            PhotonNetwork.JoinRoom(clickedRoom.name);
            VivoxManager.Instance.JoinChannel(clickedRoom.name.GetHashCode().ToString(), ChannelType.NonPositional);
        }
    }
}
