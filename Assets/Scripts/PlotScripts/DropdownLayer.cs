using TMPro;
using UnityEngine;

namespace JsonReader
{
    public class DropdownLayer : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;
        private bool[] layerVisibilities;

        private TMP_Dropdown thisDropdown;

        // Start is called before the first frame update
        private void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();

            layerVisibilities = createPlot.layerVisibilities;

            thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

            thisDropdown.value = 0;

            SetLayerVisible();
        }

        // Update is called once per frame
        private void Update()
        {
            layerVisibilities = createPlot.layerVisibilities;
        }

        public void SetLayerVisible()
        {
            layerVisibilities = new[] { false, false, false, false };
            layerVisibilities[thisDropdown.value] = true;

            createPlot.layerVisibilities = layerVisibilities;

            createPlot.SetVisibilities();
        }
    }
}