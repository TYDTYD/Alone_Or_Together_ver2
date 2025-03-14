using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
/// <summary>
/// 로딩 화면에서 서버 접속을 담당하는 스크립트
/// </summary>
/// <remarks>
/// 서버 접속을 시도하는 역할을 맡습니다
/// </remarks>
/// @date last change 2023/05/27
/// @author LSM
/// @class Connect
public class Connect : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Start함수에서 서버 접속을 시도합니다
    /// </summary>
    
    [SerializeField] private Text loading;
    [SerializeField] private GameObject loading_Img;
    [SerializeField] private GameObject lobby;
    CancellationTokenSource source = new CancellationTokenSource();
    void Start()
    {
        // 서버 접속 시도
        if (!PhotonNetwork.IsConnected)
            ConnectToServer();
        PhotonNetwork.AutomaticallySyncScene = true;
        
        LoadingTextAni().Forget();
    }
    public override void OnEnable()
    {
        base.OnEnable();
        if (source != null)
        {
            source.Dispose();
        }
        source = new CancellationTokenSource();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (source != null)
            source.Cancel();
    }

    private void OnDestroy()
    {
        if (source != null)
        {
            source.Cancel();
            source.Dispose();
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 서버 접속에 성공하면 로비씬으로 이동합니다
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 서버 접속에 실패하면 접속에 실패한 이유를 로그로 띄우고 다시 서버 접속을 시도합니다
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("연결 끊김");
        Debug.Log(cause);
        ConnectToServer();
    }

    private async UniTask LoadingTextAni()
    {
        int count = 0;
        while (!PhotonNetwork.InLobby || !VivoxManager.Instance.isLogin)
        {
            if (count == 2)
            {
                loading.text = "Loading";
                count = -1;
            }
            loading.text += ".";
            count += 1;
            await UniTask.Delay(500);
        }
        loading_Img.SetActive(false);
        lobby.SetActive(true);
    }

    public void Quit()
    {
        PhotonNetwork.Disconnect();
        VivoxManager.Instance.vivox.loginSession.Logout();
        Application.Quit();
    }
}