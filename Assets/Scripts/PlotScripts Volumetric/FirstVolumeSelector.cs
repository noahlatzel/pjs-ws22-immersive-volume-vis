using System.Collections.Generic;
using PlotScripts_Volumetric;
using TMPro;
using UnityEngine;

public class FirstVolumeSelector : MonoBehaviour
{
    public GameObject volumeTransformer;

    public GameObject runSelection;
    private TMP_Dropdown runSelectionDropdown;

    public GameObject layerSelection;
    private TMP_Dropdown layerSelectionDropdown;


    private TMP_Dropdown thisDropdown;
    private GameObject[] volumes;


    private int oldValue;
    
    // Start is called before the first frame update
    private void Start()
    {
        volumes = new GameObject[volumeTransformer.transform.childCount];

        for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;

        runSelectionDropdown = runSelection.GetComponent<TMP_Dropdown>();

        layerSelectionDropdown = layerSelection.GetComponent<TMP_Dropdown>();

        thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

        var options = new List<TMP_Dropdown.OptionData>();

        options.Add(new TMP_Dropdown.OptionData("--"));

        foreach (var volume in volumes)
        {
            options.Add(new TMP_Dropdown.OptionData(volume.name));
        }

        thisDropdown.options = options;

        thisDropdown.value = 0;
        oldValue = thisDropdown.value - 1;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SelectFirstVolume()
    {
        var currSelectedVolume = thisDropdown.value - 1;

        var currSelectedLayer = layerSelectionDropdown.value;
        
        for (int i = 0; i < volumes.Length; i++)
        {
            if (!gameObject.GetComponentInParent<EnableVols>().volumeVisibility[currSelectedVolume])
            {
                if (volumes[i].activeInHierarchy)
                {
                    var thisLV = volumes[i].GetComponent<LoadVolumes>();
                    thisLV.meteorite = false;
                    thisLV.pressure = false;
                    thisLV.temperature = false;
                    thisLV.water = false;
                } 
            }
            else
            {
                thisDropdown.value = oldValue + 1;
                Debug.Log("FirstVolumeSelector.SelectFirstVolume: Cannot Select same volume twice.");
                return;
            }
        }

        if (currSelectedVolume >= 0)
        {
            Debug.Log("In if");
            if (volumes[currSelectedVolume].activeInHierarchy)
            {
                var thisLV = volumes[currSelectedVolume].GetComponent<LoadVolumes>();
                switch (currSelectedLayer)
                {
                    case 0:
                        thisLV.pressure = true;
                        break;
                    case 1:
                        thisLV.temperature = true;
                        break;
                    case 2:
                        thisLV.water = true;
                        break;
                    case 3:
                        thisLV.meteorite = true;
                        break;
                    default:
                        Debug.Log("FirstVolumeSelector.SelectFirstVolume: Invalid Value");
                        break;
                }

                if (oldValue >= 0)
                {
                    gameObject.GetComponentInParent<EnableVols>().volumeVisibility[oldValue] = false;    
                }
                gameObject.GetComponentInParent<EnableVols>().volumeVisibility[currSelectedVolume] = true;

                oldValue = currSelectedVolume;
            }
            else
            {
                Debug.Log("FirstVolumeSelector.SelectFirstVolume: Selected Volume not available");
            }
        }
    }
}