## Alone Or Together
![main capsule](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/011e6aa9-5e00-4a85-8a09-e9592388c956)

### [플레이 영상](https://youtu.be/Okd6aUe-2yk)

### 프로젝트 소개
- 게임 장르 : 2인 멀티 협동 플랫포머
- 제작 기간 : 2022.09 ~ 2023.05
- 프로젝트 목표 : 협업을 통한 경험 및 다중 사용자 게임 개발
- 게임 소개 : 외계인에 납치당한 2명의 플레이어가 외계인의 실험 환경에서 협동을 통해 나아가는 이야기

### 개발 규모
- 팀 인원 : 4명
- 나의 역할 : 클라이언트, 서버 개발 ( 포톤 api를 통한 멀티 환경 조성, 게임 컨텐츠 제작 )
### Member
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

### 겪었던 힘들었던 점들
- 일정 관리에 대한 필요성 (일정 관리 툴이 필요하다는 것을 깨달음)
- 멀티 게임의 특성상 로컬, 리모트 간의 개념 이해 및 RPC 함수 이해에 대한 어려움
- 좋은 설계에 대한 기준점 모호 (설계 기간이 길어짐)
### 기술 설명서
- GitHub 및 Github desktop을 통한 협업 개발
- GitHub project를 통한 일정 관리
- Photon Pun2를 사용한 멀티 게임 제작
- 멀티 환경에서의 사용자 상호작용 아이템 제작

<details>
  <summary>
    상호작용 아이템 코드
  </summary>
<pre>
  <code>
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
  </code>
</pre>  
</details>
<details>
  <summary>
    멀티 환경에서의 사용자 게임 동시 접속 및 준비 시스템 제작
  </summary>
<pre>
  <code>
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
  </code>
</pre>
</details>
<details>
  <summary>
    사용자 간의 움직임 동기화 지연 최소화
  </summary>
<pre>
  <code>
    private void WalkAndSprint(P_Input input, bool TPV = true)
    {
        float lastFrameSec = Time.deltaTime;

        currentSpeed = new Vector3(rigid.velocity.x, 0, rigid.velocity.z).magnitude;
        inputMagnitude = input.Move.magnitude;

        maxSpeed = input.Sprint ? sprintSpeed : walkSpeed;
        maxSpeed = input.Move == Vector2.zero ? 0 : maxSpeed;

        _animationBlend = Mathf.Lerp(_animationBlend, maxSpeed, lastFrameSec * speedChangeRate);
        _animationBlend = _animationBlend < 0.01f ? 0 : _animationBlend;

        // 이동 속도 설정
        if (currentSpeed < maxSpeed - 0.1f || currentSpeed > maxSpeed + 0.1f)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            moveSpeed = Mathf.Lerp(currentSpeed, maxSpeed * inputMagnitude,
                lastFrameSec*speedChangeRate);

            // round speed to 3 decimal places
            moveSpeed = Mathf.Round(moveSpeed * 1000f) / 1000f;
        }
        else
            moveSpeed = maxSpeed;

        // 회전 설정
        rotation = Mathf.Atan2(input.Move.x, input.Move.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        if(TPV && input.Move != Vector2.zero)
            rigid.rotation = Quaternion.Euler(0.0f,Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref _rotationVelocity, RotationSmoothTime), 0.0f);

        rigid.MovePosition(transform.position + (Quaternion.Euler(0.0f, rotation, 0.0f) * Vector3.forward).normalized * (moveSpeed * lastFrameSec));

        animator.SetFloat(GameManager.animIDSpeed, _animationBlend);
        animator.SetFloat(GameManager.animIDMotionSpeed, inputMagnitude);
    }
  </code>
</pre>
</details>

#### Vivox Api를 통한 멀티 채팅 시스템 구현 및 멀티 오디오 시스템 구현

<details>
  <summary>
    오디오 서버 시스템 구축
  </summary>
<pre>
  <code>
    public class VivoxManager : Singleton<VivoxManager>
{
    public class Vivox
    {
        public Client client;

        public Uri server = new Uri("https://unity.vivox.com/appconfig/14568-vivox-97738-udash");
        public string issuer = "14568-vivox-97738-udash";
        public string domain = "mtu1xp.vivox.com";
        public string tokenKey = "CImCDdxDROuGjMggtuFpGyKuwYZuOP0a";
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
  </code>
</pre>
</details>

<details>
  <summary>
    오디오 대기실 시스템 관리
  </summary>
<pre>
  <code>
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
  </code>
</pre>
</details>

<details>
  <summary>
    오디오 서버 시스템 구축
  </summary>
<pre>
  <code>
      public class VivoxManager : Singleton<VivoxManager>
{
    public class Vivox
    {
        public Client client;
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
  </code>
</pre>
</details>


![image](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/8b082265-51b0-4f2f-9dc0-7b5f4cde3cd1)
