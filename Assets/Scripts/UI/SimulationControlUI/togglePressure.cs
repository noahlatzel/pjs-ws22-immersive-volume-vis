using ImportVolume;
using UnityEngine;

public class togglePressure : MonoBehaviour
{
    public GameObject volume;

    private VolumeManager volumeManager;

    private bool initialVisibility = true;
    
    // Start is called before the first frame update
    void Start()
    {
        volumeManager = volume.GetComponent<LoadVolumes>().volumeManager;
    }

    public void ToggleVisibility()
    {
        initialVisibility = !initialVisibility;
        volumeManager.SetPressure(initialVisibility);
    }
}
