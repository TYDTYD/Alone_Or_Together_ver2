using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// 이지 스테이지 선택시 활성화되는 스크립트
/// </summary>
/// 
/// @date last change 2023/05/27
/// @author LSM
/// @class EasyTrigger
public class EasyTrigger : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject EasyWall;
    [SerializeField] GameObject HardTrigger;
    [SerializeField] GameObject F;

    IEnumerator Open(GameObject block)
    {
        while (block.transform.position.y < 30)
        {
            block.transform.position += Vector3.up * 0.01f;
            yield return null;
        }
        yield break;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(Open(EasyWall));
                HardTrigger.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            F.SetActive(true);
        }
    }
}
