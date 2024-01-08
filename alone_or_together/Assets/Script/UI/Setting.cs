using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject setting;
    public void SelectResolution()
    {
        if (dropdown.value == 0)
            Screen.SetResolution(1920, 1080, true);
        else if (dropdown.value == 1)
            Screen.SetResolution(1920, 1080, false);
        else if(dropdown.value == 2)
            Screen.SetResolution(1600, 900, false);
        else if(dropdown.value == 3)
            Screen.SetResolution(1280, 720, false);
        else
            Screen.SetResolution(960, 540, false);
    }

    public void SettingClick()
    {
        setting.SetActive(true);
    }

    public void SettingCancel()
    {
        setting.SetActive(false);
    }
}
