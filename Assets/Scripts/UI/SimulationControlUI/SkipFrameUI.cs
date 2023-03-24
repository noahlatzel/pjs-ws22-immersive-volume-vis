using ImportVolume;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class SkipFrameUI : MonoBehaviour
    {
        public GameObject volume;
        public GameObject volume2;
        public GameObject synchron;

        public void SkipFrame(int numberOfFrames)
        {
            volume.GetComponent<LoadVolumes>().timestep += numberOfFrames;
                
            if (synchron.GetComponent<Toggle>().isOn) volume2.GetComponent<LoadVolumes>().timestep += numberOfFrames;
        }
    }
}