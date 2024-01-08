using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// 게임을 시작하기 전 대기방에서 게임을 준비하고 시작하는 처리를 맡은 스크립트
/// </summary>
/// 
/// @date last change 2023/05/27
/// @author LSM
/// @class Waiting_Room
public class Waiting_Room : MonoBehaviourPunCallbacks
{
    [SerializeField] Button StartGameButton;
    [SerializeField] GameObject StageButton;
    [SerializeField] GameObject ReadyGameButton;
    [SerializeField] GameObject StageSelection;
    [SerializeField] GameObject StartException;
    [SerializeField] Button[] Stages = new Button[4];
    [SerializeField] Sprite[] StagePic = new Sprite[4];
    [SerializeField] Text Information;
    [SerializeField] Text StageText;
    Sprite currentStage;
    

    bool IsReady;

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateManager.SubscribeToUpdate(UpdateWork);
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        UpdateManager.UnsubscribeFromUpdate(UpdateWork);
    }

    void Start()
    {
        Hashtable props = new Hashtable
        {
            {"IsPlayerReady", false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        ReadyGameButton.SetActive(!PhotonNetwork.IsMasterClient);
        currentStage = StageButton.GetComponent<Image>().sprite;
        StageText.text = "Tutorial";
    }

    // Update is called once per frame
    void UpdateWork()
    {
        StartGameButton.interactable = CheckPlayersReady();
        if (CheckPlayersReady())
        {
            Information.text = "All players are ready.\nYou can start the game.";
            for (int i = 0; i < Stages.Length; i++)
            {
                if (i == 0 && PlayerPrefs.GetFloat("Tutorial") == 0)
                {
                    //Stages[i].interactable = false;
                }
                else if (PlayerPrefs.GetFloat("Stage" + i + "Clear") == 0)
                {
                    //Stages[i].interactable = false;
                }
            }
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                Information.text = "All players are not ready yet.\nUnable to start the game.";
            else
                Information.text = "You need 1 more player to start the game.";
        }
        else
        {
            Information.text = "";
        }
    }

    // 방을 떠나는 함수
    public void OnLeaveGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[1]);
        }
        PhotonNetwork.LeaveRoom();
        VivoxManager.Instance.vivox.channelSession.Disconnect();
        if(VivoxManager.Instance.vivox.channelId != null)
            VivoxManager.Instance.vivox.loginSession.DeleteChannelSession(VivoxManager.Instance.vivox.channelId);
    }

    // 튜토리얼 씬으로 옮겨주고 이 방에 다른 플레이어가 들어오지 못하도록 하는 함수
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(currentStage.name);
    }

    public void OnStageButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartException.SetActive(false);
        StageSelection.SetActive(true);
    }

    // 방을 떠났을 때 씬을 옮기고 방장을 인계하는 작업을 해주는 함수
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OnJoinedLobby();
        SceneManager.LoadScene("Game_Lobby"); 
    }

    public void OnReadyGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        else
        {
            if (IsReady)
            {
                IsReady = false;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsPlayerReady", false } });
                ReadyGameButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                IsReady = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsPlayerReady", true } });
                ReadyGameButton.GetComponent<Image>().color = Color.red;
            }
        }
    }

    // 방장이 모든 플레이어가 레디했는지 확인하는 함수
    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        for(int i=1; i<PhotonNetwork.PlayerList.Length; i++)
        {
            object isPlayerReady;
            
            if (PhotonNetwork.PlayerList[i].CustomProperties.TryGetValue("IsPlayerReady", out isPlayerReady))
            {
                if ((bool)isPlayerReady)
                    return true;
            }
            else
                return false;
        }

        return false;
    }

    [PunRPC]
    private void SyncStage(int i, string str)
    {
        StageButton.GetComponent<Image>().sprite = StagePic[i];
        StageText.text = str;
    }

    public void TutorialClicked()
    {
        photonView.RPC("SyncStage", RpcTarget.AllBuffered, 0, "Tutorial");
        
        currentStage = StagePic[0];
        /*
        StageButton.GetComponent<Image>().sprite = StagePic[0];
        StageText.text = "Tutorial";
        */
        // punRpc 함수로 다른 사람에게도 스테이지 사진 보여주기
        StartException.SetActive(true);
        StageSelection.SetActive(false);
    }

    public void Stage1Clicked()
    {
        photonView.RPC("SyncStage", RpcTarget.AllBuffered, 1, "Stage 1");
        
        currentStage = StagePic[1];
        /*
        StageButton.GetComponent<Image>().sprite = StagePic[1];
        StageText.text = "Stage 1";
        */
        StartException.SetActive(true);
        StageSelection.SetActive(false);
    }

    public void Stage2Clicked()
    {
        photonView.RPC("SyncStage", RpcTarget.AllBuffered, 2, "Stage 2");
        
        currentStage = StagePic[2];
        /*
        StageButton.GetComponent<Image>().sprite = StagePic[2];
        StageText.text = "Stage 2";
        */
        StartException.SetActive(true);
        StageSelection.SetActive(false);
    }

    public void Stage3Clicked()
    {
        StageText.text = "Stage 3";
        //StageButton.GetComponent<Image>().sprite =
        StartException.SetActive(true);
        StageSelection.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        ReadyGameButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }
}
