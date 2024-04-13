using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Start_Scene_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("StartClear", 1);
            SceneManager.LoadScene("Game_Lobby");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
