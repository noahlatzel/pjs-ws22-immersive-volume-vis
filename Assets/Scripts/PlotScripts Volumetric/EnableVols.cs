using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotScripts_Volumetric
{
    public class EnableVols : MonoBehaviour
    {
        public GameObject runSelection;
        private TMP_Dropdown runSelectionDropdown;

        public GameObject layerSelection;
        private TMP_Dropdown layerSelectionDropdown;

        public GameObject volumeTransformer;
        private GameObject[] volumes;
    
        private Toggle thisToggle;
        // Start is called before the first frame update
        void Start()
        {
            volumes = new GameObject[volumeTransformer.transform.childCount];
            
            Debug.Log("Anzahl an Volumenobjekten: " + volumes.Length);

            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
            }
            
            Debug.Log("Name von Volumen 3: " + volumes[3].name);
            
            runSelectionDropdown = runSelection.GetComponent<TMP_Dropdown>();
            
            layerSelectionDropdown = layerSelection.GetComponent<TMP_Dropdown>();
            
            thisToggle = gameObject.GetComponent<Toggle>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void activateVolumes()
        {
            volumeTransformer.transform.position = new Vector3(0f, 6.5f, 20f);
            volumeTransformer.transform.localScale = new Vector3(20f, 20f, 20f);
            
            foreach (var volume in volumes)
            {
                if (volume.activeInHierarchy)
                {
                    var thisLV = volume.GetComponent<LoadVolumes>();
                    thisLV.meteorite = false;
                    thisLV.pressure = false;
                    thisLV.temperature = false;
                    thisLV.water = false;   
                }
            }
            if (thisToggle.isOn)
            {
                int currSelectedRun = runSelectionDropdown.value;

                int currSelectedLayer = layerSelectionDropdown.value;

                if (volumes[currSelectedRun].activeInHierarchy)
                {
                    var thisLV = volumes[currSelectedRun].GetComponent<LoadVolumes>();
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
                            Debug.Log("EnableVols.activateVolumes: Invalid Value");
                            break;                        
                    }
                }
            }
        }
    }
}
