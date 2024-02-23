using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VivoxUnity;
/// <summary>
/// 게임 시작씬에서 모든 처리를 담당하고 로딩 씬으로 넘겨주는 역할
/// </summary>
/// <remarks>
/// 닉네임을 설정하고 게임 로딩씬으로 넘어가는 역할을 맡은 스크립트입니다.
/// 이외의 모든 처리를 담당했습니다
/// </remarks>
public class Scene_Load : MonoBehaviourPunCallbacks
{
    public event Action Tutorial;
    public InputField Nickname;
    public GameObject player;
    public GameObject start;
    public GameObject information;
    [SerializeField] Text warning;
    Start_Player move;
    VivoxManager.Vivox vivox;

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

    private void Start()
    {
        move = player.GetComponent<Start_Player>();
        vivox = VivoxManager.Instance.vivox;
    }

    void UpdateWork()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// 입력이 시작되면 플레이어 비활성화
    /// </summary>
    public void input()
    {
        move.enabled = false;
    }
    /// <summary>
    /// 입력이 끝나면 플레이어 활성화
    /// </summary>
    public void endInput()
    {
        move.enabled = true;
    }

    public void nickname()
    {
        start.SetActive(true);
        information.SetActive(false);
        
    }

    

    /// <summary>
    /// 닉네임을 적고 버튼을 누르면 이 함수가 실행되며 로딩씬으로 넘어갑니다.
    /// </summary>
    public void Load()
    {
        bool pass = true;

        foreach (char c in Nickname.text)
        {
            if (c == ' ')
            {
                warning.text = "Nickname must not contain spaces";
                pass = false;
                // 한글 검사 only english
            }
        }

        if (pass && Nickname.text != "")
        {
            VivoxManager.Instance.Login(VivoxManager.Instance.GetInstanceID().ToString());
            Debug.Log(VivoxManager.Instance.GetInstanceID().ToString());
            PhotonNetwork.LocalPlayer.NickName = Nickname.text;
            if (PlayerPrefs.GetFloat("StartClear") == 0)
                SceneManager.LoadScene("Start_Scene");
            else
                SceneManager.LoadScene("Game_Lobby");
        }
        else if (pass)
            warning.text = "Nickname must be at least one characters";
        Tutorial();
    }


}
