using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPAuse : MonoBehaviour
{
    public Sprite playDefault;
    public Sprite pauseDefault;

    public bool playing;

    public GameObject volume;
    public GameObject volume2;
    public GameObject dropdown;

    public Button button;


    // Start is called before the first frame update
    void Start()
    {
        playing = false;
        volume = GameObject.Find("RenderedVolume");
        volume2 = GameObject.Find("RenderedVolume2");
        dropdown = GameObject.Find("Volume2");
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
            if (!(dropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Default"))
            {
                volume2.GetComponent<LoadVolumes>().play = true;
            }
                volume.GetComponent<LoadVolumes>().play = true;
        }
        else 
        {
            button.image.sprite = pauseDefault;
            playing = false;
            if (!(dropdown.GetComponentInChildren<TextMeshProUGUI>().text == "Default"))
            {
                volume2.GetComponent<LoadVolumes>().play = false;
            }
                volume.GetComponent<LoadVolumes>().play = false;

        }

    }
}


