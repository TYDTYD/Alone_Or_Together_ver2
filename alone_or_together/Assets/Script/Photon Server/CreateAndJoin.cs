using System.Collections;
using System;
using VivoxUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 룸에 대한 설정들을 맡습니다
/// </summary>
/// <remarks>
/// 멀티플레이를 위한 방을 열거나 방에 들어갈 수 있도록 해주는 스크립트입니다.
/// </remarks>
/// 
/// @date last change 2023/05/27
/// @author LSM
/// @class CreateAndJoin
public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public GameObject room_Prefab;
    public GameObject create_Option;
    public GameObject lobby;
    public Transform parent_room;
    public Toggle toggle;
    public Text warning;

    Dictionary<string, GameObject> room_Dict = new Dictionary<string, GameObject>();

    private void Start()
    {
        warning.text = "";
    }

    /// <summary>
    /// 방을 만드는 함수입니다
    /// </summary>
    public void CreateRoom()
    {
        if (RoomNameCheck(createInput.text))
        {
            room_Dict.Add(createInput.text, room_Prefab);
            VivoxManager.Instance.JoinChannel(createInput.text.GetHashCode().ToString(), ChannelType.NonPositional);
            if (toggle.isOn)
                PhotonNetwork.CreateRoom(createInput.text, new RoomOptions { MaxPlayers = 2, IsVisible = false });
            else
                PhotonNetwork.CreateRoom(createInput.text, new RoomOptions { MaxPlayers = 2 });
        }
        else
        {
            warning.text = "Room name must not contain spaces";
        }
    }

    bool RoomNameCheck(string name)
    {
        foreach(char c in name)
        {
            if (c == ' ')
                return false;
        }
        return true;
    }

    public void CreateOption()
    {
        create_Option.SetActive(true);
        lobby.SetActive(false);
    }

    public void CreateCancel()
    {
        create_Option.SetActive(false);
        lobby.SetActive(true);
    }

    /// <summary>
    /// 방에 참가하는 함수입니다
    /// </summary>
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        if (PhotonNetwork.InRoom)
            VivoxManager.Instance.JoinChannel(joinInput.text.GetHashCode().ToString(), ChannelType.NonPositional);
    }


    /// <summary>
    /// 씬 전환 함수입니다
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("Game_Waiting_Room");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                
                if (room_Dict.ContainsKey(room.Name))
                {
                    room_Dict.TryGetValue(room.Name, out GameObject tmp);
                    room_Dict.Remove(room.Name);
                    Destroy(tmp);
                }
            }
            else
            {
                if (!room_Dict.ContainsKey(room.Name))
                {
                    GameObject room_obj = Instantiate(room_Prefab, parent_room);
                    room_Dict.Add(room.Name, room_obj);
                    room_obj.name = room.Name;
                    if (room.PlayerCount == 2)
                        room_obj.GetComponentInChildren<Text>().text = room.Name + " Full";
                    else
                        room_obj.GetComponentInChildren<Text>().text ="Room Title : "+room.Name+" Current Player : "+room.PlayerCount+" / "+room.MaxPlayers;
                }
            }
        }
    }

    
}
