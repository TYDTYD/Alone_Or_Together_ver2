using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.transform.position = GameManager.Instance.RespawnPoint;
        }
    }
}
