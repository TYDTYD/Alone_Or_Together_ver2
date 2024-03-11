#### Alone Or Together
![main capsule](https://github.com/TYDTYD/Alone_Or_Together_ver2/assets/48386074/011e6aa9-5e00-4a85-8a09-e9592388c956)
### 프로젝트 소개
- 게임 장르 : 2인 멀티 협동 플랫포머
- 제작 기간 : 2022.09 ~ 2022.05
- 프로젝트 목표 : 협업을 통한 경험 및 다중 사용자 게임 개발
- 게임 소개 : 외계인에 납치당한 2명의 플레이어가 외계인의 실험 환경에서 협동을 통해 나아가는 이야기

### 개발 규모
- 팀 인원 : 4명
- @youwonsock @TYDTYD @ingsussi @CassisSoda
- 나의 역할 : 개발자 ( 포톤 api를 통한 멀티 환경 조성, 게임 컨텐츠 제작 )

### 겪었던 힘들었던 점들
- 일부 일정을 따르지 않는 팀원간의 불화 (일정 관리 툴이 필요하다는 것을 깨달음)
- 첫 멀티 게임 제작인 만큼 로컬, 리모트 간의 RPC 함수 이해에 대한 어려움
- 설계만 4개월정도 걸렸음 이에 따라 너무 루즈해지는 감이 있지 않나 싶음
### 기술 설명서
- GitHub 및 Github desktop을 통한 협업 개발
- GitHub project를 통한 일정 관리
- 멀티 환경에서의 사용자 상호작용 아이템 제작
<pre>
  <code>
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
  </code>
</pre>
- 멀티 환경에서의 사용자 게임 동시 접속 및 준비 시스템 제작
- 이미지 넣기
<pre>
  <code>
    
  </code>
</pre>
- 사용자 간의 움직임 동기화 지연 최소화
<pre>
  <code>
    
  </code>
</pre>
- 멀티 환경에서의 사용자 컨텐츠 제작 (2인 협동 자동차)
- 영상 넣으면 좋을 것 같은데
<pre>
  <code>
    
  </code>
</pre>
- vivox api를 통한 멀티 채팅 시스템 구현 및 멀티 오디오 시스템 구현
- 이미지 넣기 or 영상 넣기
<pre>
  <code>
    
  </code>
</pre>
