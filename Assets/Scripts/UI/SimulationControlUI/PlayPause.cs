using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPause : MonoBehaviour
{
    public bool playing;

    public GameObject volume;
    public GameObject volume2;
    public GameObject dropdown;
    public GameObject referenceButton;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        volume2 = GameObject.Find("RenderedVolume2");
        dropdown = GameObject.Find("Volume2");
    }

    public void ChangeStartStop()
    {
        if (!playing)
        {
            playing = true;
            referenceButton.GetComponent<PlayPause>().playing = true;
            if (dropdown.GetComponentInChildren<TextMeshProUGUI>().text != "Default")
            {
                volume2.GetComponent<LoadVolumes>().play = true;
            }
            volume.GetComponent<LoadVolumes>().play = true;
        }
        else 
        {
            playing = false;
            referenceButton.GetComponent<PlayPause>().playing = false;
            if (dropdown.GetComponentInChildren<TextMeshProUGUI>().text != "Default")
            {
                volume2.GetComponent<LoadVolumes>().play = false;
            }
            volume.GetComponent<LoadVolumes>().play = false;
        }
        transform.gameObject.SetActive(false);
        referenceButton.SetActive(true);
    }
}


