using ImportVolume;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class ToggleTemperature : MonoBehaviour
    {
        public GameObject firstVolume;
        public GameObject secondVolume;

        public void ToggleVisibility()
        {
            firstVolume.GetComponent<LoadVolumes>().temperature = GetComponent<Toggle>().isOn;
            secondVolume.GetComponent<LoadVolumes>().temperature = GetComponent<Toggle>().isOn;
        }
    }
}
