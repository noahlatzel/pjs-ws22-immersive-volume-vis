using TMPro;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class RunSelection : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        private CreatePlot createPlot;

        private TMP_Dropdown thisDropdown;

        // Start is called before the first frame update
        private void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();

            thisDropdown = gameObject.GetComponent<TMP_Dropdown>();

            thisDropdown.value = createPlot.selectedRun;
        }

        public void SetSelectedRun()
        {
            createPlot.selectedRun = thisDropdown.value;
        }

        // // Update is called once per frame
        // void Update()
        // {
        //
        // }
    }
}
