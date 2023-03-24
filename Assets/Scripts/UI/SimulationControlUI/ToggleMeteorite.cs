using ImportVolume;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class ToggleMeteorite : MonoBehaviour
    {
        public GameObject firstVolume;
        public GameObject secondVolume;

        public void ToggleVisibility()
        {
            firstVolume.GetComponent<LoadVolumes>().meteorite = GetComponent<Toggle>().isOn;
            secondVolume.GetComponent<LoadVolumes>().meteorite = GetComponent<Toggle>().isOn;
        }
    }
}
