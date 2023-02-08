using TMPro;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class DimensionSelection : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;
        
        private TMP_Dropdown thisDropdown;
        
        // Start is called before the first frame update
        void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();
            
            thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

            thisDropdown.value = createPlot.dimension;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetDimension()
        {
            createPlot.SetPlotData(thisDropdown.value+1);
            createPlot.dimension = thisDropdown.value + 1;
        }
    }
}
