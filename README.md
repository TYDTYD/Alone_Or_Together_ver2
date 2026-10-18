# Alone Or Together
![main capsule](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/011e6aa9-5e00-4a85-8a09-e9592388c956)

### [플레이 영상](https://youtu.be/Okd6aUe-2yk)

## 프로젝트 소개
- 게임 장르 : 2인 멀티 협동 플랫포머
- 제작 기간 : 2022.09 ~ 2023.05
- 프로젝트 목표 : 협업을 통한 경험 및 다중 사용자 게임 개발
- 게임 소개 : 외계인에 납치당한 2명의 플레이어가 외계인의 실험 환경에서 협동을 통해 나아가는 이야기

## 개발 규모
- 팀 인원 : 4명
- 나의 역할 : 클라이언트, 서버 개발 ( 포톤 api를 통한 멀티 환경 조성, 게임 컨텐츠 제작 )
## Member
<table>
    <tr height="140px">
        <td align="center" width="130px">
            <a href="https://github.com/youwonsock"><img height="100px" width="100px" src="https://avatars.githubusercontent.com/u/46276141?v=4"/></a>
            <br />
            <a href="https://github.com/youwonsock">youwonsock</a>
        </td>
        <td align="center" width="130px">
            <a href="https://github.com/TYDTYD"><img height="100px" width="100px" src="https://avatars.githubusercontent.com/u/48386074?v=4"/></a>
            <br />
            <a href="https://github.com/TYDTYD">TYDTYD</a>
        </td>
        <td align="center" width="130px">
            <a href="https://github.com/ingsussi"><img height="100px" width="100px" src="https://avatars.githubusercontent.com/u/79362735?v=4"/></a>
            <br />
            <a href="https://github.com/ingsussi">ingsussi</a>
        </td>
        <td align="center" width="130px">
            <a href="https://github.com/CassisSoda"><img height="100px" width="100px" src="https://avatars.githubusercontent.com/u/97022429?v=4"/></a>
            <br />
            <a href="https://github.com/CassisSoda">CassisSoda</a>
        </td>
</table>

# 기술 경험

## 왜 많은 기술 중에서 Photon을 선택했는가?
![image](https://github.com/user-attachments/assets/9ddda425-774d-496d-a9a2-0dccedcc0f84)

당시 저희 팀원 중에서 멀티 플레이 게임을 제작해 본 경험이 있는 사람은 없었습니다.

서버를 직접 구현하기에는 여러 가지 제약이 있었고, 서버 구축을 도와주는 서드 파티(Third-party) 라이브러리 중에서 가장 쉽고 빠르게 접근할 수 있었던 것이 바로 Photon이었습니다.

Photon은 여러 기술 중에서 가장 자료가 많고, 대중성이 높았기 때문에 선택하게 되었습니다.

## Photon PUN을 통한 멀티 환경 조성

![image](https://github.com/user-attachments/assets/6f820171-91bc-42f3-8ba0-ac3f985f2969)

닉네임 입력 후 Start 버튼을 통해 서버 접속을 시도합니다.

![image](https://github.com/user-attachments/assets/644d6430-f9ce-44f2-8204-49b0f210c847)

서버 접속 중에는 로딩 애니메이션 문구를 재생합니다.

![image](https://github.com/user-attachments/assets/5078c295-d849-49e1-a71f-28749f2ea444)

서버 접속 성공 시 Photon Lobby로 입장합니다.
방을 생성하거나 참가할 수 있으며 생성된 방 목록을 볼 수 있습니다.

방을 생성할 때는 비밀번호를 설정할 수 있습니다.

![image](https://github.com/user-attachments/assets/02826e37-1b43-4a3c-a45a-d0c8fded31fa)

방에 들어가면 Photon Room으로 입장합니다.
같은 Photon Room에 있는 사람들은 동일한 Vivox 서버를 공유합니다.

</pre>  
</details>
<details>
  <summary>
    멀티 환경에서의 사용자 게임 동시 접속 및 준비 시스템 제작
  </summary>
    
```cs
    void Start()
    {
        Hashtable props = new Hashtable
        {
            {"IsPlayerReady", false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        ReadyGameButton.SetActive(!PhotonNetwork.IsMasterClient);
    }

    void UpdateWork()
    {
        // 레디 체크 함수를 통한 스타트 버튼 활성화/비활성화
        StartGameButton.interactable = CheckPlayersReady();
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

    // 레디 버튼 클릭 함수
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

    // 레디 상태 체크 함수
    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        for(int i=1; i<=PhotonNetwork.PlayerList.Length-1; i++)
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
```
</details>

Photon PUN을 이용해 서버 접속, 방 생성, 참가 시스템을 구현했으며, 멀티 대기실에서 방장과 참가자의 역할을 명확히 분리했습니다.

또한, 방장이 참가자에게 방장 권한을 양도할 수 있도록 구현하여 유연한 진행이 가능하도록 했습니다.

## Photon Pun을 통한 협력 & 경쟁 컨텐츠 제작
![image](https://github.com/user-attachments/assets/668ea563-ded3-43a0-b882-24c59b57bd21)

한 명은 위/아래 키, 나머지 한 명은 좌/우 키를 사용하여 차량을 협력해 운전하는 컨텐츠입니다. 

장애물을 피해 차량을 안전하게 운전해야 합니다.

![image](https://github.com/user-attachments/assets/19ca056f-9e2f-4811-b78d-76fd6885843e)
<details>
  <summary>
    미로 위치 바꾸기 오브젝트
  </summary>
   
```cs
public class Position_Switch : MonoBehaviourPunCallbacks
{
    GameObject[] index;
    GameObject player1;
    GameObject player2;
    
    private void Start()
    {
        Invoke("playerFind", 2f);
    }

    void playerFind()
    {
        index = GameObject.FindGameObjectsWithTag("Player");
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("playerIndex", RpcTarget.All, index[0].GetPhotonView().ViewID, index[1].GetPhotonView().ViewID);
        }
    }
    [PunRPC]
    void playerIndex(int view1, int view2)
    {
        player1 = PhotonView.Find(view1).gameObject;
        player2 = PhotonView.Find(view2).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetPhotonView().IsMine)
        {
            AudioManager.Instance.AddSfxSoundData(SFXClip.PosChangeItem, false, transform.position);
            Vector3 pos1 = player1.transform.position;
            Vector3 pos2 = player2.transform.position;
            photonView.RPC("SwitchPlayerPositions", RpcTarget.AllViaServer, pos1, pos2);
        }
    }

    [PunRPC]
    void SwitchPlayerPositions(Vector3 pos1, Vector3 pos2)
    {
        player1.transform.position = pos2;
        player2.transform.position = pos1;

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}
```
</details>

한 명은 미로 안에서, 다른 한 명은 컨테이너 박스에서 탈출을 도와줍니다. 미로 안의 특정 오브젝트에 닿으면 두 플레이어의 위치가 바뀌며, 탈출에 성공하면 컨테이너 박스가 내려옵니다.

![image](https://github.com/user-attachments/assets/2e3e7594-c6f1-42a8-bc0f-8eff6788afae)

그래플링 훅을 활용해 특정 지형을 오를 수 있는 맵을 제작했습니다. 먼저 목적지에 도착하는 사람에 따라 서로 다른 엔딩이 제공됩니다.

Photon PUN을 통해 서로 상호작용 할 수 있는 협력 컨텐츠 및 경쟁 컨텐츠를 기획 및 제작했습니다.



## Vivox와 Photon을 통한 유저 상호작용 시스템 설계

![image](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/8b082265-51b0-4f2f-9dc0-7b5f4cde3cd1)

협력 게임이므로 원활한 소통을 위해 음성 대화 기능이 필요하다고 판단했습니다.

따라서 Vivox를 사용하여 채팅 및 음성 대화를 구현했습니다.

<details>
  <summary>
    오디오 서버 시스템 구축
  </summary>

```cs
public class VivoxManager : Singleton<VivoxManager>
{
    public class Vivox
    {
        public Client client;

        public Uri server = new Uri("https://unity.vivox.com/appconfig/14568-vivox-97738-udash");
        public string issuer;
        public string domain;
        public string tokenKey;
        public TimeSpan timeSpan = TimeSpan.FromSeconds(90);

        public ILoginSession loginSession;
        public IChannelSession channelSession;
        public ChannelId channelId;
    }

    ChatManager input;
    public Vivox vivox = new Vivox();
    public bool isLogin = false;

    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();
    }

    private void OnApplicationQuit()
    {
        vivox.client.Uninitialize();
    }

    public void UserCallbacks(bool bind, IChannelSession session)
    {
        if (bind)
        {
            vivox.channelSession.Participants.AfterKeyAdded += AddUser;
            vivox.channelSession.Participants.BeforeKeyRemoved += LeaveUser;
        }
        else
        {
            vivox.channelSession.Participants.AfterKeyAdded -= AddUser;
            vivox.channelSession.Participants.BeforeKeyRemoved -= LeaveUser;
        }
    }

    public void AddUser(object sender, KeyEventArg<string> userData)
    {
        var temp = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = temp[userData.Key];
    }

    public void LeaveUser(object sender, KeyEventArg<string> userData)
    {
        var temp = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = temp[userData.Key];
    }

    public void Login(string name)
    {
        AccountId accountId = new AccountId(vivox.issuer, name, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accountId);
        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokenKey, vivox.timeSpan),
            callback =>
            {
                try
                {
                    vivox.loginSession.EndLogin(callback);
                    isLogin = true;
                    Debug.Log("로그인 완료");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debug.Log("로그인 실패");
                }
            });
    }

    public void JoinChannel(string channelName, ChannelType channelType)
    {
        vivox.channelId = new ChannelId(vivox.issuer, channelName, vivox.domain, channelType);
        vivox.channelSession = vivox.loginSession.GetChannelSession(vivox.channelId);
        UserCallbacks(true, vivox.channelSession);
        ChannelCallbacks(true, vivox.channelSession);
        vivox.channelSession.BeginConnect(true, true, true, vivox.channelSession.GetConnectToken(vivox.tokenKey, vivox.timeSpan),
            callback =>
            {
                try
                {
                    vivox.channelSession.EndConnect(callback);
                    Debug.Log("채널 접속 완료");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        
    }

    public void LeaveChannel()
    {
        UserCallbacks(false, vivox.channelSession);
        ChannelCallbacks(false, vivox.channelSession);
        vivox.channelSession.Disconnect();
    }

    public void ChannelCallbacks(bool bind,IChannelSession session)
    {
        if (bind)
        {
            session.MessageLog.AfterItemAdded += ReceiveMessage;
        }
        else
        {
            session.MessageLog.AfterItemAdded -= ReceiveMessage;
        }
    }

    public void SendMsg(string str)
    {
        vivox.channelSession.BeginSendText(str, callback =>
        {
             try
             {
                 vivox.channelSession.EndSendText(callback);
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 throw;

             }
        });
    }

    public void ReceiveMessage(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs){
        var message= queueItemAddedEventArgs.Value.Message;
        input = GameObject.FindGameObjectWithTag("ChatInput").GetComponent<ChatManager>();
        input.InputChat(message);
    }
}
```
</details>

<details>
  <summary>
    오디오 대기실 시스템 관리
  </summary>
    
```cs
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
```
</details>

# 트러블 슈팅

## Photon의 네트워크 지연 발생

<details>
  <summary>
    사용자 간의 움직임 동기화 지연 최소화
  </summary>
    
```cs
    if (!photonView.IsMine)
{
    if (transform.parent != null)
        transform.localPosition =
            Vector3.Lerp(
                transform.localPosition,
                networkLocalPosition,
                Time.deltaTime * 10);
    else
        transform.position =
            Vector3.Lerp(
                transform.position,
                networkPosition,
                Time.deltaTime * 10);

    if (Vector3.Distance(
        transform.position, networkPosition) > 10)
        transform.position = networkPosition;
}
```
</details>

문제점 : 네트워크 동기화 시 플레이어의 움직임이 불안정하며, 순간 이동(Spike) 현상이 발생했습니다.

문제 원인 분석 : Photon은 Transform 정보를 일정 간격으로 동기화하는 방식이므로, 네트워크 지연이 발생하면 보간(Interpolation) 작업 없이 플레이어가 순간 이동하는 현상이 발생할 수 있다고 파악했습니다.

해결책 : 현재 플레이어와 네트워크 응답으로 받은 플레이어의 위치가 기준치 이상일 경우, 해당 위치로 바로 이동하도록 처리하였습니다.

## 상호작용 아이템 오류 발생

문제점 : 서로의 위치를 바꾸는 아이템이 작동할 때, 한 명의 위치는 그대로 있거나 원래 위치로 돌아가는 문제가 발생했습니다.

문제 원인 분석 : 아이템이 사용 시 상대방의 현재 위치를 참조하는 방식이었지만, 네트워크 환경에 따라 호출 속도 차이가 발생하여 더 빠른 환경의 플레이어가 이미 위치를 변경했고, 느린 환경의 플레이어가 그 위치를 뒤늦게 참조하는 것이 원인이었습니다.

해결책 : 위치를 바꾸기 직전에 상대방의 위치를 참조하여, 해당 데이터를 기반으로 교환하도록 수정하였습니다. 이를 통해 네트워크 환경에 관계없이 안정적으로 위치가 변경되었습니다.

# 그 외 개발한 것들

## 2 스테이지 맵 디자인
![image](https://github.com/user-attachments/assets/e4594dd8-896a-4f89-9f8f-c56852552825)

## 장애물 피하기 컨텐츠
![image](https://github.com/user-attachments/assets/3ad51af1-1f0e-4302-a05e-f5612ac34b80)

## 그래플링 맵 디자인
![image](https://github.com/user-attachments/assets/f1bccd72-32d1-4da6-9844-50c6cfe3f6bf)

## 스팀 출시
![image](https://github.com/user-attachments/assets/3c821342-d40b-4cfb-9c59-af5783fc4dba)

## Benefit 시스템
플레이 방식에 따라 달라지는 컨텐츠
