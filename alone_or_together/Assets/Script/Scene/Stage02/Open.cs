using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 2스테이지 문을 여는 스크립트
/// </summary>
/// @date last change 2023/05/27
/// @author LSM
/// @class Open
public class Open : MonoBehaviour
{
    [SerializeField] GameObject a;
    [SerializeField] GameObject b;
    [SerializeField] GameObject RespawnPoint;

    IEnumerator OpenTheDoor(GameObject a, GameObject b)
    {
        while (a.transform.position.z < -67 && b.transform.position.z > -224)
        {
            a.transform.position += Vector3.forward * 0.01f;
            b.transform.position -= Vector3.forward * 0.01f;
            yield return null;
        }
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(OpenTheDoor(a, b));
            RespawnPoint.transform.position = new Vector3(538.51f, 2.4f, -140.04f);
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
