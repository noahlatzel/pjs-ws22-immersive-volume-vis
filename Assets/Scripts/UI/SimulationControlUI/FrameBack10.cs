using ImportVolume;
using TMPro;
using UnityEngine;

namespace UI.SimulationControlUI
{
    public class FrameBack10 : MonoBehaviour
    {
        public bool playing;

        public GameObject volume;
        public GameObject volume2;
        public GameObject dropdown;

        // Start is called before the first frame update
        void Start()
        {
            volume = GameObject.Find("RenderedVolume");
            volume2 = GameObject.Find("RenderedVolume2");
            dropdown = GameObject.Find("Volume2");
            playing = volume.GetComponent<LoadVolumes>().play;
        }

        public void SkipFrame()
        {
            if (playing)
            {
                volume.GetComponent<LoadVolumes>().timesPerSecond = volume.GetComponent<LoadVolumes>().timesPerSecond - 10; 
                volume2.GetComponent<LoadVolumes>().timesPerSecond = volume.GetComponent<LoadVolumes>().timesPerSecond - 10; 
            }
            else
            {
                volume.GetComponent<LoadVolumes>().volumeManager.SkipFrame(-10);
                volume2.GetComponent<LoadVolumes>().volumeManager.SkipFrame(-10);
                
                // Set timeStep in inspector
                GameObject.Find("RenderedVolume").GetComponent<LoadVolumes>().timestep = volume.GetComponent<LoadVolumes>().volumeManager.currentTimeStep;
                GameObject.Find("RenderedVolume2").GetComponent<LoadVolumes>().timestep = volume.GetComponent<LoadVolumes>().volumeManager.currentTimeStep;
            }
        }
    }
}
