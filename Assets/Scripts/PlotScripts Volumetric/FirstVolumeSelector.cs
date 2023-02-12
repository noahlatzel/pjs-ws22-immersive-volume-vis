using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirstVolumeSelector : MonoBehaviour
{
    public GameObject volumeTransformer;
    private GameObject[] volumes;
 
    public GameObject runSelection;
    private TMP_Dropdown runSelectionDropdown;

    public GameObject layerSelection;
    private TMP_Dropdown layerSelectionDropdown;
    
    private TMP_Dropdown thisDropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        volumes = new GameObject[volumeTransformer.transform.childCount];

        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
        }
        
        thisDropdown = gameObject.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
