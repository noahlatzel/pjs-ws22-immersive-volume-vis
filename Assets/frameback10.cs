using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class frameback10 : MonoBehaviour
{
    public bool playing;

    public GameObject volume;

    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        playing = volume.GetComponent<LoadVolumes>().play;
    }

    public void SkipFrame(InputAction.CallbackContext context)
    {
        if (playing)
        {
            volume.GetComponent<LoadVolumes>().timesPerSecond = volume.GetComponent<LoadVolumes>().timesPerSecond - 10; ;
        }
        else
        {
            volume.GetComponent<LoadVolumes>().volumeManager.PreviousFrame(10);
        }
    }
}
