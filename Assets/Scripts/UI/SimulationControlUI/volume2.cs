using TMPro;
using UnityEngine;

public class volume2 : MonoBehaviour
{
    public GameObject volume;
    public GameObject dropdown;

    // Start is called before the first frame update
    void Start()
    {
        volume = GameObject.Find("RenderedVolume2");
        dropdown = GameObject.Find("Volume2");
    }

    // Update is called once per frame
    void Update()
    {
        switch (dropdown.GetComponentInChildren<TextMeshProUGUI>().text)
        {
            case ("yA11"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yA11");
                break;
            case ("yA31"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yA31");
                break;
            case ("yA32"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yA32");
                break;
            case ("yB11"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yB11");
                break;
            case ("yB31"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yB31");
                break;
            case ("yC11"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yC11");
                break;
            case ("yC31"):
                volume.GetComponent<LoadVolumes>().volumeManager.SetDataset("yC31");
                break;
            case ("Default"): break;

        }
    }
}
