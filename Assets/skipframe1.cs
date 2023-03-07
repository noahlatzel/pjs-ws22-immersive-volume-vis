using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class skipframe1 : MonoBehaviour
{
    public bool playing;

    public GameObject volume;

    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        playing = volume.GetComponent<LoadVolumes>().play;
    }

    public void Update()
    {
        if (Input.GetButtonDown("+1 Frame"))
        {
            SkipFrame();
        }
    }
    public void SkipFrame()
    {
        if (playing)
        {
            volume.GetComponent<LoadVolumes>().timesPerSecond++;
        }
        else
        {
            volume.GetComponent<LoadVolumes>().volumeManager.NextFrame();
        }
    }
}
