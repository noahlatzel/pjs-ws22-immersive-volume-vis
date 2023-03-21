using System.Collections.Generic;
using PlotScripts_Volumetric;
using TMPro;
using UnityEngine;

public class VolumeSelector : MonoBehaviour
{
    public GameObject volumeTransformer;

    public GameObject runSelection;

    public GameObject layerSelection;

    public GameObject uiVolumeToggle;

    public bool isThisFirstDropdown;
    private TMP_Dropdown layerSelectionDropdown;
    private TMP_Dropdown runSelectionDropdown;

    private TMP_Dropdown thisDropdown;
    private EnableVols uiVolumeToggleComp;
    private GameObject[] volumes;

    // Start is called before the first frame update
    private void Start()
    {
        uiVolumeToggleComp = uiVolumeToggle.GetComponent<EnableVols>();

        volumes = new GameObject[volumeTransformer.transform.childCount];

        for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;

        runSelectionDropdown = runSelection.GetComponent<TMP_Dropdown>();

        layerSelectionDropdown = layerSelection.GetComponent<TMP_Dropdown>();

        thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

        var options = new List<TMP_Dropdown.OptionData>();

        options.Add(new TMP_Dropdown.OptionData("--"));

        foreach (var volume in volumes) options.Add(new TMP_Dropdown.OptionData(volume.name));

        thisDropdown.options = options;

        thisDropdown.value = 0;

        if (isThisFirstDropdown)
            uiVolumeToggleComp.firstVolume = -1;
        else
            uiVolumeToggleComp.secondVolume = -1;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SelectVolume()
    {
        var currSelectedVolume = thisDropdown.value - 1;

        var currSelectedLayer = layerSelectionDropdown.value;

        int lastVolume;

        if (isThisFirstDropdown)
            lastVolume = uiVolumeToggleComp.firstVolume;
        else
            lastVolume = uiVolumeToggleComp.secondVolume;

        if (lastVolume >= 0) uiVolumeToggleComp.volumeVisibility[lastVolume] = false;

        if (currSelectedVolume >= 0)
        {
            if (uiVolumeToggleComp.volumeVisibility[currSelectedVolume])
            {
                Debug.Log("Same volume can not be displayed twice.");

                thisDropdown.value = lastVolume + 1;
            }
            else
            {
                uiVolumeToggleComp.volumeVisibility[currSelectedVolume] = true;

                if (isThisFirstDropdown)
                    uiVolumeToggleComp.firstVolume = currSelectedVolume;
                else
                    uiVolumeToggleComp.secondVolume = currSelectedVolume;
            }

            uiVolumeToggleComp.SetLayerVisibility();
        }
    }
}