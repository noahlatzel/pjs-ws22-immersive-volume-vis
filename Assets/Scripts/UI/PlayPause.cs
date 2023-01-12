using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayPause : MonoBehaviour
{
    public Sprite playDefault;
    public Sprite playHighlight;
    public Sprite playPressed;
    public Sprite pauseDefault;
    public Sprite pauseHighlight;
    public Sprite pausePressed;

    public bool playing;

    public Button button;

    //public GameObject VolumeScriptStarter;

    // Start is called before the first frame update
    void Start()
    {
        playing = false;
        //VolumeScriptStarter = GameObject.Find("VolumeScriptStarter");
        //Debug.Log(VolumeScriptStarter.name);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeStartStop()
    {
        playing = !playing;
        SpriteState buttonSS = button.spriteState;
        
        if (!playing)
        {
            Debug.Log("Play Case");
            button.image.sprite = playDefault;
            buttonSS.highlightedSprite = playHighlight;
            buttonSS.pressedSprite = playPressed;
            buttonSS.selectedSprite = playHighlight;
            buttonSS.disabledSprite = playPressed;
        }
        else
        {
            Debug.Log("Pause Case");
            button.image.sprite = pauseDefault;
            buttonSS.highlightedSprite = pauseHighlight;
            buttonSS.pressedSprite = pausePressed;
            buttonSS.selectedSprite = pauseHighlight;
            buttonSS.disabledSprite = pausePressed;
        }
        Debug.Log(buttonSS.selectedSprite.name);
    }
}
