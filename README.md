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
- 일정 관리에 대한 필요성 (일정 관리 툴이 필요하다는 것을 깨달았습니다)
- 멀티 게임의 특성상 로컬, 리모트 간의 개념 이해 및 RPC 함수 이해에 대한 어려움
- 좋은 설계에 대한 기준점 모호 (설계 기간이 길어졌습니다)
### 기술 설명서
- GitHub 및 Github desktop을 통한 협업 개발
- GitHub project를 통한 일정 관리
- Photon Pun2를 사용한 멀티 게임 제작
- 멀티 환경에서의 사용자 상호작용 아이템 제작

</pre>  
</details>
<details>
  <summary>
    멀티 환경에서의 사용자 게임 동시 접속 및 준비 시스템 제작
  </summary>
<pre>

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
</pre>
</details>
<details>
  <summary>
    사용자 간의 움직임 동기화 지연 최소화
  </summary>
<pre>

```cs
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
```
</pre>
</details>

#### Vivox Api를 통한 멀티 채팅 시스템 구현 및 멀티 오디오 시스템 구현

![image](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/8b082265-51b0-4f2f-9dc0-7b5f4cde3cd1)

<details>
  <summary>
    오디오 서버 시스템 구축
  </summary>
<pre>

```cs
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
```
</pre>
</details>

<details>
  <summary>
    오디오 대기실 시스템 관리
  </summary>
<pre>
    
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
</pre>
</details>

### 상호작용 컨텐츠
<details>
  <summary>
    미로 위치 바꾸기 오브젝트
  </summary>
 <pre>
   
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
 </pre>
</details>
<details>
  <summary>
    협력 자동차
  </summary>
 <pre>
   
```cs
public class Car_Controller_Ver2 : MonoBehaviourPunCallbacks, IChangeCarSpeed
{
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

    private void UpdateWork()
    {
        if (!together.BoardingAll)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.5f);
            return;
        }

        // We determine the speed of the car.
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        //CAR PHYSICS
        if(together.BoardingNum == 0)
        {
            if (Input.GetKey(KeyCode.W))
            {
                CancelInvoke("DecelerateCar");
                GoForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                CancelInvoke("DecelerateCar");
                GoReverse();
            }
            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
            {
                InvokeRepeating("DecelerateCar", 0f, 0.5f);
                ThrottleOff();
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                TurnLeft();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                TurnRight();
            }
            else if(steeringAxis != 0)
                ResetSteeringAngle();

            photonView.RPC(nameof(SyncSteer), RpcTarget.All, steeringAxis, frontLeftCollider.steerAngle, frontRightCollider.steerAngle);
        }

        // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
        AnimateWheelMeshes();
    }

    /// <summary>
    /// 차량 조향 각도 동기화 메서드
    /// </summary>
    /// <param name="steeringAxis"></param>
    /// <param name="LCSteerAngle"></param>
    /// <param name="RCSteerAngle"></param>
    [PunRPC]
    private void SyncSteer(float steeringAxis, float LCSteerAngle, float RCSteerAngle)
    {
        this.steeringAxis = steeringAxis;
        this.frontLeftCollider.steerAngle = LCSteerAngle;
        this.frontRightCollider.steerAngle = RCSteerAngle;
    }

    /// <summary>
    /// 차량 최대 속도 변경 메서드
    /// </summary>
    public void ChangeCarSpeed()
    {
        maxSpeed = 300;
        maxReverseSpeed = 100;
        accelerationMultiplier = 20;

        brakeForce = 600;
        decelerationMultiplier = 0.1f;

        TryGetComponent<AudioSource>(out AudioSource source);
        source.pitch = 0.8f;
    }

    //
    //STEERING METHODS
    //

    //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method turns the front car wheels to the right. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //The following method takes the front car wheels to their default position (rotation = 0). The speed of this movement will depend
    // on the steeringSpeed variable.
    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward()
    {
        // The following part sets the throttle power to 1 smoothly.
        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }
        //If the car is going backwards, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
        //is safe to apply positive torque to go forward.
        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                // If the maxSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    // This method apply negative torque to the wheels in order to go backwards.
    public void GoReverse()
    {
        // The following part sets the throttle power to -1 smoothly.
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }
        //If the car is still going forward, then apply brakes in order to avoid strange
        //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
        //is safe to apply negative torque to go reverse.
        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
                // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
                // could be a bit higher than expected.
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    public void DecelerateCar()
    {
        // The following part resets the throttle power to 0 smoothly.
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
        // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
        // also cancel the invoke of this method.
        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    // This function applies brake torque to the wheels according to the brake force given by the user.
    public void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }
}
```
 </pre>
</details>

### Audio Manager를 통한 사운드 관리
<details>
  <summary>
    AudioManager
  </summary>
 <pre>
   
```cs
public class AudioManager : Singleton<AudioManager>
{
    private class AudioData
    {
        AudioClip audioClip;
        bool loop;
        Vector3 pos;
        Transform parent;

        public AudioClip AudioClip { get { return audioClip; } }
        public bool Loop { get { return loop; } }
        public Vector3 Position { get { return pos; } }
        public Transform Parent { get { return parent; } }

        public AudioData(AudioClip clip, Vector3 pos, bool loop = false)
        {
            audioClip = clip;
            this.pos = pos;
            this.loop = loop;
            this.parent = null;
        }
        public AudioData(AudioClip clip, Transform parent, bool loop = false)
        {
            audioClip = clip;
            this.parent = parent;
            this.loop = loop;
            this.pos = Vector3.zero;
        }
    }

    protected AudioManager() { }

    #region Fields

    //- Private -
    private CancellationTokenSource _source;

    [SerializeField] [Range(0, 1)] private float master = 1;
    [SerializeField] [Range(0, 1)] private float sfx = 1;
    [SerializeField] [Range(0, 1)] private float bgm = 1;

    [SerializeField] private AudioClip[] sfxClips = new AudioClip[Enum.GetValues(typeof(SFXClip)).Length];
    [SerializeField] private AudioClip[] bgmClips = new AudioClip[Enum.GetValues(typeof(BGMClip)).Length];

    private Queue<AudioData> dateQueue = new Queue<AudioData>();

    private Queue<AudioSource> sfxQueue = new Queue<AudioSource>();

    private AudioSource bgmSource;

    //- Public -

    #endregion



    #region Property

    //- Public -
    public float Master
    {
        get { return master; }
        set
        {
            master = value;
            bgmSource.volume = master * bgm;
        }
    }
    public float Sfx { get { return sfx; } set { sfx = value; } }
    public float Bgm
    {
        get { return bgm; }
        set
        {
            bgm = value;
            bgmSource.volume = master * bgm;
        }
    }

    #endregion



    #region Methods
    //- Private -
    private void UpdateWork()
    {
        if (dateQueue.Count > 0)
            PlaySfxSound(dateQueue.Dequeue()).Forget();
    }

    /// <summary>
    /// AudioData를 받아 재생해주는 메서드
    /// </summary>
    /// <param name="data">재생할 오디오 데이터</param>
    /// <returns></returns>
    private async UniTaskVoid PlaySfxSound(AudioData data)
    {
        AudioSource audio = sfxQueue.Count > 0 ? sfxQueue.Dequeue() : CreateAudioSource();

        audio.transform.position = data.Position;
        audio.clip = data.AudioClip;
        audio.loop = data.Loop;
        audio.volume = master * sfx;
        audio.transform.parent = data.Parent;

        audio.Play();

        await UniTask.Delay(TimeSpan.FromSeconds(audio.clip.length), ignoreTimeScale: false);

        if(!data.Loop)
            audio.transform.parent = this.transform;

        sfxQueue.Enqueue(audio);
    }

    /// <summary>
    /// 초기화 메서드
    /// </summary>
    private void Init()
    {
        bgmSource = CreateAudioSource();

        for (int i = 0; i < 10; i++)
            sfxQueue.Enqueue(CreateAudioSource());
    }

    /// <summary>
    /// 새로운 AudioSource 생성 메서드
    /// </summary>
    /// <returns></returns>
    private AudioSource CreateAudioSource()
    {
        var obj = new GameObject();
        obj.transform.parent = this.transform;
        var audio = obj.AddComponent<AudioSource>();
        audio.loop = false;

        return audio;
    }

    /// <summary>
    /// 씬 전환시 실행되는 메서드
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.name)
        {
            case "Game_Start":
            case "Game_Lobby":
            case "Game_Waiting_Room":
            case "Ending":
                SetBGM(BGMClip.Menu);
                break;
            case "Tutorial":
                SetBGM(BGMClip.Tutorial);
                break;
            case "Stage_1":
                SetBGM(BGMClip.Stage1_1);
                break;
            case "Stage_2":
                SetBGM(BGMClip.Stage2_1);
                break;
            case "Credit":
                SetBGM(BGMClip.Credit);
                break;
        }    
    }

    //- Public -

    /// <summary>
    /// BGM 설정 메서드
    /// </summary>
    /// <param name="data"></param>
    public void SetBGM(BGMClip clip, bool loop = true)
    {
        bgmSource.loop = loop;
        bgmSource.clip = bgmClips[(int)clip];

        bgmSource.Play();
    }

    /// <summary>
    /// SFX Sound 재생 메서드
    /// </summary>
    /// <param name="data"></param>
    public void AddSfxSoundData(SFXClip clip, bool loop, Vector3 pos)
    {
        if (sfxClips[(int)clip] != null)
            dateQueue.Enqueue(new AudioData(sfxClips[(int)clip], pos, loop));
        else
            Debug.Log("Audio Clip is Null");
    }

    /// <summary>
    /// 음원을 따라가는 SFX Sound 재생 메서드
    /// </summary>
    /// <param name="data"></param>
    public void AddSfxSoundData(SFXClip clip, bool loop, Transform transform)
    {
        if (sfxClips[(int)clip] != null)
            dateQueue.Enqueue(new AudioData(sfxClips[(int)clip], transform, loop));
        else
            Debug.Log("Audio Clip is Null");
    }

    /// <summary>
    /// 음소거
    /// </summary>
    public void MuteAll()
    {
        Master = 0;
        Bgm = 0;
        Sfx = 0;
    }

    public void MuteBgm()
    {
        Bgm = 0;
    }

    public void MuteSfx()
    {
        Sfx = 0;
    }

    #endregion



    #region UnityEvent

    private void OnEnable()
    {
        if (_source != null)
            _source.Dispose();

        _source = new CancellationTokenSource();

        UpdateManager.SubscribeToUpdate(UpdateWork);

        GameManager.Instance.AddOnSceneLoaded(OnSceneLoaded);
    }

    private void OnDisable()
    {
        if (_source != null)
            _source.Cancel();

        UpdateManager.UnsubscribeFromUpdate(UpdateWork);

        if(GameManager.Instance != null)
            GameManager.Instance.RemoveOnSceneLoaded(OnSceneLoaded);
    }

    /// <summary>
    /// Awake에서 실행할 작업을 구현하는 메서드
    /// </summary>
    protected override void OnAwakeWork()
    {
        Init();
    }

    /// <summary>
    /// OnDestroyed에서 실행할 작업을 구현하는 메서드
    /// </summary>
    protected override void OnDestroyedWork()
    {
        base.OnDestroyedWork();

        if (_source != null)
        {
            _source.Cancel();
            _source.Dispose();
        }
    }

    #endregion
}
```
 </pre>
</details>
