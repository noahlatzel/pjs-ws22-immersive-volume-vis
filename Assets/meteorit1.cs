using UnityEngine;
using UnityEngine.UI;

public class meteorit1 : MonoBehaviour
{
    public GameObject volume;
    public GameObject checkbox;
    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        checkbox = GameObject.Find("Meteorit1");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkbox.GetComponent<Toggle>().isOn)
        {
            volume.GetComponent<LoadVolumes>().meteorite = true;
        }
        else
        {
            volume.GetComponent<LoadVolumes>().meteorite = false;
        }
    }
}
