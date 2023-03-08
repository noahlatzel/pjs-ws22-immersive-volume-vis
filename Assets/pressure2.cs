using UnityEngine;
using UnityEngine.UI;

public class pressure2 : MonoBehaviour
{
    public GameObject volume;
    public GameObject checkbox;
    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        checkbox = GameObject.Find("Pressure2");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkbox.GetComponent<Toggle>().isOn)
        {
            volume.GetComponent<LoadVolumes>().pressure = true;
        }
        else
        {
            volume.GetComponent<LoadVolumes>().pressure = false;
        }
    }
}
