using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Services.Core;
using VivoxUnity;
using Photon.Pun;

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