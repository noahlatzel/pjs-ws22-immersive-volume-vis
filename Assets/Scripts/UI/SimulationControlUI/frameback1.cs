using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class frameback1 : MonoBehaviour
{
    public bool playing;

    public GameObject volume;
    public GameObject volume2;
    public GameObject dropdown;

    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        volume2 = GameObject.Find("RenderedVolume2");
        dropdown = GameObject.Find("Volume2");
        playing = volume.GetComponent<LoadVolumes>().play;
    }

    /*public void Update()
    {
        if (Input.GetButtonDown("-1 Frame"))
        {
            SkipFrame();
        }
    }*/

    public void SkipFrame()
    {
        if (playing)
        {
            if (!(dropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Default"))
            {
                volume2.GetComponent<LoadVolumes>().timesPerSecond--;
            }
                volume.GetComponent<LoadVolumes>().timesPerSecond--;
        }
        else
        {
            if (!(dropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Default"))
            {
                volume2.GetComponent<LoadVolumes>().volumeManager.PreviousFrame();
            }
                volume.GetComponent<LoadVolumes>().volumeManager.PreviousFrame();
        }
    }
}
