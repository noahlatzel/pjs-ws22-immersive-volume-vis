using UnityEngine;
using UnityEngine.UI;

public class temperature2 : MonoBehaviour
{
    public GameObject volume;
    public GameObject checkbox;
    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        checkbox = GameObject.Find("Temperature2");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkbox.GetComponent<Toggle>().isOn)
        {
            volume.GetComponent<LoadVolumes>().temperature = true;
        }
        else
        {
            volume.GetComponent<LoadVolumes>().temperature = false;
        }
    }
}
