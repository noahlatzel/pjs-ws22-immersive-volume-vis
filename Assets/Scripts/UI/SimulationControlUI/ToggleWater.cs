using ImportVolume;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SimulationControlUI
{
    public class ToggleWater : MonoBehaviour
    {
        public GameObject firstVolume;
        public GameObject secondVolume;

        public void ToggleVisibility()
        {
            firstVolume.GetComponent<LoadVolumes>().water = GetComponent<Toggle>().isOn;
            secondVolume.GetComponent<LoadVolumes>().water = GetComponent<Toggle>().isOn;
        }
    }
}
