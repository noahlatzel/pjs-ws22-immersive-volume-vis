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
            volumeTransformer.transform.position = new Vector3(0f, 6.5f, 20f);
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
    }
}