using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotScripts_Volumetric
{
    public class EnableVols : MonoBehaviour
    {
        public GameObject runSelection;

        public GameObject layerSelection;

        public GameObject volumeTransformer;
        public bool[] volumeVisibility;

        public int firstVolume = -1; //Volume that is set by first dropdown, -1 if none
        public int secondVolume = -1; //Volume that is set by second dropdown, -1 if none
        public bool posChanged = false;

        private TMP_Dropdown layerSelectionDropdown;
        private TMP_Dropdown runSelectionDropdown;

        public GameObject VolumeTurn;
        public GameObject Volume1Desc;
        public GameObject Volume1Drop;
        public GameObject Volume2Desc;
        public GameObject Volume2Drop;

        private Toggle thisToggle;

        private GameObject[] volumes;

        // Start is called before the first frame update
        private void Start()
        {
            volumes = new GameObject[volumeTransformer.transform.childCount];

            // Debug.Log("Anzahl an Volumenobjekten: " + volumes.Length);

            for (var i = 0; i < volumes.Length; i++) volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;

            // Debug.Log("Name von Volumen 3: " + volumes[3].name);

            runSelectionDropdown = runSelection.GetComponent<TMP_Dropdown>();

            layerSelectionDropdown = layerSelection.GetComponent<TMP_Dropdown>();

            volumeVisibility = new bool[volumes.Length];

            for (var i = 0; i < volumeVisibility.Length; i++) volumeVisibility[i] = false;

            thisToggle = gameObject.GetComponent<Toggle>();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void activateVolumes()
        {
            volumeTransformer.transform.position = new Vector3(0f, 6.2f, 20f);
            volumeTransformer.transform.localScale = new Vector3(20f, 20f, 20f);

            foreach (var volume in volumes)
                if (volume.activeInHierarchy)
                {
                    var thisLV = volume.GetComponent<LoadVolumes>();
                    thisLV.meteorite = false;
                    thisLV.pressure = false;
                    thisLV.temperature = false;
                    thisLV.water = false;
                }

            for (var i = 0; i < volumeVisibility.Length; i++) volumeVisibility[i] = false;

            if (thisToggle.isOn)
            {
                VolumeTurn.SetActive(true);
                Volume1Desc.SetActive(true);
                Volume1Drop.SetActive(true);
                Volume2Desc.SetActive(true);
                Volume2Drop.SetActive(true);
            }
            else
            {
                VolumeTurn.SetActive(false);
                Volume1Desc.SetActive(false);
                Volume1Drop.SetActive(false);
                Volume2Desc.SetActive(false);
                Volume2Drop.SetActive(false);
            }
        }

        public void SetLayerVisibility()
        {
            for (int i = 0; i < volumeVisibility.Length; i++)
            {
                var thisLV = volumes[i].GetComponent<LoadVolumes>();

                thisLV.meteorite = false;
                thisLV.pressure = false;
                thisLV.temperature = false;
                thisLV.water = false;
                if (volumeVisibility[i])
                {
                    switch (layerSelectionDropdown.value)
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
                            Debug.Log("EnableVols.SetLayerVisibility: Invalid Value");
                            break;
                    }
                }
            }
            
            PositionVolumes();
        }

        public void PositionVolumes()
        {
            if (firstVolume > -1 && secondVolume > -1)
            {
                volumes[firstVolume].transform.localPosition = new Vector3(-0.52f, 0f, 0f);
                volumes[secondVolume].transform.localPosition = new Vector3(0.52f, 0f, 0f);
            }
            else if (firstVolume != secondVolume)
            {
                if (firstVolume > -1)
                {
                    volumes[firstVolume].transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                if (secondVolume > -1)
                {
                    volumes[secondVolume].transform.localPosition = new Vector3(0f, 0f, 0f);
                }
            }
        }
    }
}