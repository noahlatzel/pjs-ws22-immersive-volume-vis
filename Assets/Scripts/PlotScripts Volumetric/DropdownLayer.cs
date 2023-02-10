using TMPro;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class DropdownLayer : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;
        private int visibleLayer;

        private TMP_Dropdown thisDropdown;

        // Start is called before the first frame update
        private void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();

            visibleLayer = createPlot.visibleLayer;

            thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

            thisDropdown.value = 0; //Pressure is visible on program start

            SetLayerVisible();
        }

        // Update is called once per frame
        private void Update()
        {
            visibleLayer = createPlot.visibleLayer;
        }

        public void SetLayerVisible()
        {
            visibleLayer = thisDropdown.value;

            createPlot.visibleLayer = visibleLayer;

            createPlot.SetVisibilities();
        }
    }
}