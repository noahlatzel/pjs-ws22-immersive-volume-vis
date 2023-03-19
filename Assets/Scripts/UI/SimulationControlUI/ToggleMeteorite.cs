using ImportVolume;
using UnityEngine;

namespace UI.SimulationControlUI
{
    public class ToggleMeteorite : MonoBehaviour
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
            volumeManager.SetMeteorite(initialVisibility);
        }
    
    }
}
