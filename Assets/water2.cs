using UnityEngine;
using UnityEngine.UI;

public class water2 : MonoBehaviour
{
    public GameObject volume;
    public GameObject checkbox;
    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume");
        checkbox = GameObject.Find("Water2");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkbox.GetComponent<Toggle>().isOn)
        {
            volume.GetComponent<LoadVolumes>().volumeManager.SetWater(true);
        }
        else
        {
            volume.GetComponent<LoadVolumes>().volumeManager.SetWater(false);
        }
    }
}
