
using System.Collections;
using System.Collections.Generic;
using ImportVolume;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayPauseScript : MonoBehaviour
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


    public void changeStartStop(InputAction.CallbackContext context)
    {
     
        if (context.performed && !playing)
        {
            button.image.sprite = playDefault;
            playing = true;
            volume.GetComponent<LoadVolumes>().play = true;
        }
        else if (context.performed && playing)
        {
            button.image.sprite = pauseDefault;
            playing = false;
            volume.GetComponent<LoadVolumes>().play = false;

        }

    }
}

