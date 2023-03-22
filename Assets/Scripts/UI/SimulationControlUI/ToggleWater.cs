using ImportVolume;
using UnityEngine;

namespace UI.SimulationControlUI
{
    public class ToggleWater : MonoBehaviour
    {
        public GameObject firstVolume;
        public GameObject secondVolume;

        private VolumeManager firstVolumeManager;
        private VolumeManager secondVolumeManager;
        
        private bool initialVisibility = false;
    
        // Start is called before the first frame update
        void Start()
        {
            firstVolumeManager = firstVolume.GetComponent<LoadVolumes>().volumeManager;
            secondVolumeManager = secondVolume.GetComponent<LoadVolumes>().volumeManager;
        }

        public void ToggleVisibility()
        {
            initialVisibility = !initialVisibility;
            firstVolumeManager.SetWater(initialVisibility);
            secondVolumeManager.SetWater(initialVisibility);
        }
    }
}
