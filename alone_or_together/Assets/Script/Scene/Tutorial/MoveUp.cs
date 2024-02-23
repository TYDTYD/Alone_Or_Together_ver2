using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        yield return new WaitForSeconds(2.0f);
        while (transform.position.y < 100)
        {
            transform.position += Vector3.up * 0.01f;
            yield return null;
        }
        yield break;
    }
}
