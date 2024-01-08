using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public int index;
    public bool isMute, OtherMute;
    int Volume, OtherVolume;
    [SerializeField] GameObject[] Profile = new GameObject[2];
    [SerializeField] Button[] Profile_Btn = new Button[2];
    [SerializeField] Image[] Profile_Img = new Image[2];
    [SerializeField] Text[] Profile_Text = new Text[2];
    public Sprite Other, Mine, mute, sound;

    // Start is called before the first frame update
    void Start()
    {
        isMute = VivoxManager.Instance.vivox.client.AudioInputDevices.Muted;
        OtherMute = VivoxManager.Instance.vivox.client.AudioOutputDevices.Muted;
        Volume = VivoxManager.Instance.vivox.client.AudioInputDevices.VolumeAdjustment;
        OtherVolume = VivoxManager.Instance.vivox.client.AudioOutputDevices.VolumeAdjustment;

        Mine = sound;
        Other = sound;
        
        MasterCheckInit();
    }

    void MasterCheckInit()
    {
        if (PhotonNetwork.InRoom)
        {
            for (int i = 0; i < 2; i++)
            {
                Profile_Btn[i].onClick.RemoveAllListeners();
                if (i < PhotonNetwork.PlayerList.Length)
                {
                    Profile[i].SetActive(true);
                    Profile_Text[i].text = PhotonNetwork.PlayerList[i].NickName;
                    if (PhotonNetwork.PlayerList[i].IsLocal)
                    {
                        Profile_Img[i].sprite = sound;
                        Profile_Btn[i].onClick.AddListener(MuteClicked);
                    }
                    else
                    {
                        Profile_Img[i].sprite = sound;
                        Profile_Btn[i].onClick.AddListener(OtherMuteClicked);
                    }
                }
                else
                    Profile[i].SetActive(false);
            }
        }
    }

    void MasterCheck()
    {
        if (PhotonNetwork.InRoom)
        {
            for (int i = 0; i < 2; i++)
            {
                Profile_Btn[i].onClick.RemoveAllListeners();
                if (i < PhotonNetwork.PlayerList.Length)
                {
                    Profile[i].SetActive(true);
                    Profile_Text[i].text = PhotonNetwork.PlayerList[i].NickName;
                    if (PhotonNetwork.PlayerList[i].IsLocal)
                    {
                        Profile_Img[i].sprite = Mine;
                        Profile_Btn[i].onClick.AddListener(MuteClicked);
                    }
                    else
                    {
                        Profile_Img[i].sprite = Other;
                        Profile_Btn[i].onClick.AddListener(OtherMuteClicked);
                    }
                }
                else
                    Profile[i].SetActive(false);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        MasterCheck();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        MasterCheck();
    }

    void MuteClicked()
    {
        VivoxManager.Instance.vivox.client.AudioInputDevices.Muted = !isMute;
        isMute = !isMute;
        if (isMute)
            Mine = mute;
        else
            Mine = sound;
        for (int i = 0; i < 2; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.PlayerList[i].IsLocal)
                    Profile_Img[i].sprite = Mine;
            }
        }
    }

    void OtherMuteClicked()
    {
        VivoxManager.Instance.vivox.client.AudioOutputDevices.Muted = !OtherMute;
        OtherMute = !OtherMute;
        if (OtherMute)
            Other = mute;
        else
            Other = sound;
        for (int i = 0; i < 2; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (!PhotonNetwork.PlayerList[i].IsLocal)
                    Profile_Img[i].sprite = Other;
            }
        }
    }
}
