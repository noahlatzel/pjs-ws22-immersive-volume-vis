
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayPAuse : MonoBehaviour
{
    public Sprite playDefault;
    public Sprite pauseDefault;

    public bool playing;

    public GameObject volume;

    public Button button;


    // Start is called before the first frame update
    void Start()
    {
        playing = false;
        volume = GameObject.Find("RenderedVolume");
    }

    public void Update()
    {
        if (Input.GetButtonDown("Play/Pause"))
        {
           ChangeStartStop();
        }
    }

    public void ChangeStartStop()
    {

        if (!playing)
        {
            button.image.sprite = playDefault;
            playing = true;
            volume.GetComponent<LoadVolumes>().play = true;
        }
        else 
        {
            button.image.sprite = pauseDefault;
            playing = false;
            volume.GetComponent<LoadVolumes>().play = false;

        }

    }
}


