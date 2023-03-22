using ImportVolume;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class SkipFrame10 : MonoBehaviour
    {
        public bool playing;

        public GameObject volume;
        public GameObject volume2;
        public GameObject dropdown;
        public GameObject synchron;
        
        // Start is called before the first frame update
        void Start()
        {
            dropdown = GameObject.Find("Volume2");
            playing = volume.GetComponent<LoadVolumes>().play;
        }

        public void SkipFrame()
        {
            if (playing)
            {
                volume.GetComponent<LoadVolumes>().timesPerSecond = volume.GetComponent<LoadVolumes>().timesPerSecond+10;
            }
            else
            {
                volume.GetComponent<LoadVolumes>().volumeManager.SkipFrame(10);
                
                // Set timeStep in inspector
                volume.GetComponent<LoadVolumes>().timestep = volume.GetComponent<LoadVolumes>().volumeManager.currentTimeStep;
                
                if (synchron.GetComponent<Toggle>().isOn)
                {
                    volume2.GetComponent<LoadVolumes>().volumeManager.SkipFrame(10);
                    volume2.GetComponent<LoadVolumes>().timestep = volume.GetComponent<LoadVolumes>().volumeManager.currentTimeStep;
                }
            }
        }
    }
}
