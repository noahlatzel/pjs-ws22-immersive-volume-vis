using ImportVolume;
using UnityEngine;

namespace UI.SimulationControlUI
{
    public class ToggleTemperature : MonoBehaviour
    {
        public GameObject volume;

        private VolumeManager volumeManager;

        private bool initialVisibility = false;
    
        // Start is called before the first frame update
        void Start()
        {
            volumeManager = volume.GetComponent<LoadVolumes>().volumeManager;
        }

        public void ToggleVisibility()
        {
            initialVisibility = !initialVisibility;
            volumeManager.SetTemperature(initialVisibility);
        }
    }
}
