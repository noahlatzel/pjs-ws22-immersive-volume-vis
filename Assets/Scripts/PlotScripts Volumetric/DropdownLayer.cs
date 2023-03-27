using System.Collections.Generic;
using ImportVolume;
using TMPro;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class DropdownLayer : MonoBehaviour
    {
        public GameObject uiVolumeToggle;
        private EnableVols uiVolumeToggleComp;
        public GameObject volumeTransformer;
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;
        private int visibleLayer;
        private GameObject[] volumes;
        
        private TMP_Dropdown thisDropdown;

        private GameObject selectedVolume;
        
        // Start is called before the first frame update
        private void Start()
        {
            uiVolumeToggleComp = uiVolumeToggle.GetComponent<EnableVols>();

            createPlot = plotGameObject.GetComponent<CreatePlot>();

            visibleLayer = createPlot.visibleLayer;
            
            thisDropdown = gameObject.GetComponent<TMP_Dropdown>();
            
            selectedVolume = GameObject.Find("yB11");
            
            volumes = new GameObject[volumeTransformer.transform.childCount];

            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
                if (volumeTransformer.transform.GetChild(i).childCount > 0)
                {
                    selectedVolume = volumeTransformer.transform.GetChild(i).gameObject;
                }
            }

            // Clear option list
            gameObject.GetComponent<TMP_Dropdown>().options = new List<TMP_Dropdown.OptionData>();
            
            // Set dropdown options
            for (int i = 0; i < selectedVolume.transform.childCount; i++)
            {
                gameObject.GetComponent<TMP_Dropdown>().options.Add(
                    new TMP_Dropdown.OptionData(selectedVolume.transform.GetChild(i).name));
            }
            
            thisDropdown.value = 0; //Pressure is visible on program start

            SetLayerVisible();
        }

        // Update is called once per frame
        private void Update()
        {
            visibleLayer = createPlot.visibleLayer;
        }

        public void SetLayerVisible()
        {
            visibleLayer = thisDropdown.value;

            createPlot.visibleLayer = visibleLayer;

            createPlot.SetVisibilities();
            
            uiVolumeToggleComp.SetLayerVisibility();

            foreach (var volume in volumes)
            {
                if (volume.transform.childCount > 0)
                {
                    volume.GetComponent<LoadVolumes>().meteorite = 0 == thisDropdown.value;
                    volume.GetComponent<LoadVolumes>().pressure = 1 == thisDropdown.value;
                    volume.GetComponent<LoadVolumes>().temperature = 2 == thisDropdown.value;
                    volume.GetComponent<LoadVolumes>().water = 3 == thisDropdown.value;
                }
            }
        }
    }
}