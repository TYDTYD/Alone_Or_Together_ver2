using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;

public class Connect_Loading : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text loading;
    CancellationTokenSource source = new CancellationTokenSource();
    void Start()
    {
        LoadingTextAni().Forget();
        PhotonNetwork.JoinLobby();
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
        SceneManager.LoadScene("Game_Lobby");
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
        while (!PhotonNetwork.InLobby)
        {
            if (count == 2)
            {
                loading.text = "Loading.";
                count = 0;
            }
            loading.text += ".";
            count += 1;
            await UniTask.Delay(500);
        }
    }
}
