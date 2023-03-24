using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnVolume : MonoBehaviour
{
    private UnityEngine.UI.Button thisButton;

    public GameObject volumeTransformer;
    private GameObject[] volumes;
    
    // Start is called before the first frame update
    void Start()
    {
        volumes = new GameObject[volumeTransformer.transform.childCount];

        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
        }
            
        thisButton = gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void turnVolume90()
    {
        foreach (var volume in volumes)
        {
            volume.transform.eulerAngles += new Vector3(0, 45, 0);
        }
    }

    public void setRotation0()
    {
        foreach (var volume in volumes)
        {
            volume.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
