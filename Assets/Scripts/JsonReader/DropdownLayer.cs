using UnityEngine;

namespace JsonReader
{
    public class DropdownLayer : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;
        private bool[] layerVisibilities;

        private TMPro.TMP_Dropdown thisDropdown;

        // Start is called before the first frame update
        void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();

            layerVisibilities = createPlot.layerVisibilities;

            thisDropdown = gameObject.GetComponent<TMPro.TMP_Dropdown>();

            thisDropdown.value = 0;
            
            SetLayerVisible();
        }

        // Update is called once per frame
        void Update()
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
