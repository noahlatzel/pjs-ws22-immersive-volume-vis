using System.IO;
using ImportVolume;
using TMPro;
using UnityEngine;

namespace UI.SimulationControlUI
{
    public class SelectVolumeDataset : MonoBehaviour
    {
        public GameObject volume;
        private string[] dataSetPaths;

        // Start is called before the first frame update
        void Start()
        {
            // Get all dataset paths
            dataSetPaths = Directory.GetDirectories("Assets/Datasets/");
        
            // Set dropdown options
            foreach (var t in dataSetPaths)
            {
                GetComponent<TMP_Dropdown>().options.Add(
                    new TMP_Dropdown.OptionData(Path.GetFileName(t)));
            }
        
            GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate
            {
                DropDownValueChanged(GetComponent<TMP_Dropdown>().value);
            });

            
        }

        void DropDownValueChanged(int value)
        {
            volume.GetComponent<LoadVolumes>().volumeManager.SetDataset(dataSetPaths[value]);
        }
    
    }
}
