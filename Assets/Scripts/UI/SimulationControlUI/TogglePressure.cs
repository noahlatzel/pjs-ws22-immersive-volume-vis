using ImportVolume;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class TogglePressure : MonoBehaviour
    {
        public GameObject firstVolume;
        public GameObject secondVolume;

        public void ToggleVisibility()
        {
            firstVolume.GetComponent<LoadVolumes>().pressure = GetComponent<Toggle>().isOn;
            secondVolume.GetComponent<LoadVolumes>().pressure = GetComponent<Toggle>().isOn;
        }
    }
}
