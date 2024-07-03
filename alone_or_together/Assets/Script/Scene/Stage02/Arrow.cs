using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Arrow : MonoBehaviourPunCallbacks
{
    Image arrow;

    private void Start()
    {
        arrow = GetComponent<Image>();
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

    private void UpdateWork()
    {
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            photonView.RPC("SetArrow", RpcTarget.All);
        }
        else
        {
            arrow.enabled = false;
        }
    }
    [PunRPC]
    public void DestroyArrow()
    {
        arrow.enabled = false;
    }

    [PunRPC]
    public void SetArrow()
    {
        if (!photonView.IsMine && arrow!=null)
            arrow.enabled = true;
        else if(arrow != null)
            arrow.enabled = false;
    }
}
