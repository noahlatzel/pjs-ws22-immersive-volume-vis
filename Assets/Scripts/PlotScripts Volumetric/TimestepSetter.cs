using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimestepSetter : MonoBehaviour
{

    public GameObject RenderedVolume;
    private ImportVolume.LoadVolumes loadVolumes;
    
    private UnityEngine.UI.Button thisButton;
    
    // Start is called before the first frame update
    void Start()
    {
        loadVolumes = RenderedVolume.GetComponent<ImportVolume.LoadVolumes>();
        thisButton = gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPress(int timeStep)
    {
        loadVolumes.SetFrame(timeStep);
    }
}
