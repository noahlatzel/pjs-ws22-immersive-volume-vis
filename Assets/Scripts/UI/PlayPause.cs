using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

public class PlayPause : MonoBehaviour
{
    public Sprite playSprite;
    public Sprite stopSprite;

    public Button button;

    public GameObject VolumeScriptStarter;

    // Start is called before the first frame update
    void Start()
    {
        VolumeScriptStarter = GameObject.Find("VolumeScriptStarter");
        Debug.Log(VolumeScriptStarter.name);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
