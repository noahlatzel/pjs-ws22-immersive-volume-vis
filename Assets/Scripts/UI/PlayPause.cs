using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayPause : MonoBehaviour
{
    public Sprite playSprite;
    public Sprite stopSprite;

    public bool playing;

    public Button button;

    public GameObject VolumeScriptStarter;

    // Start is called before the first frame update
    void Start()
    {
        playing = false;
        VolumeScriptStarter = GameObject.Find("VolumeScriptStarter");
        Debug.Log(VolumeScriptStarter.name);

    }

    // Update is called once per frame
    void Update()
    {
        changeStartStop();
    }

    public void swapState()
    {
        playing = !playing;
    }

    public void changeStartStop()
    {
        if (!playing)
        {
            button.image.sprite = playSprite;
        }
        else
        {
            button.image.sprite = stopSprite;
        }
    }
}
