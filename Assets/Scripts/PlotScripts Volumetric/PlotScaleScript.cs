using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

namespace PlotScripts_Volumetric
{
    public class PlotScaleScript : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;
        private CreatePlot createPlot;

        public GameObject dimensionDropdown;
        private TMP_Dropdown dimensionDropdownComp;
        private int currDimension;

        public GameObject scaleDesc;
        private TMP_Text scaleDescComp;

        private Slider thisSlider;
        
        // Start is called before the first frame update
        void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();

            dimensionDropdownComp = dimensionDropdown.GetComponent<TMP_Dropdown>();

            currDimension = dimensionDropdownComp.value;

            scaleDescComp = scaleDesc.GetComponent<TMP_Text>();

            thisSlider = gameObject.GetComponent<Slider>();

            thisSlider.value = createPlot.transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (currDimension != dimensionDropdownComp.value)
            {
                currDimension = dimensionDropdownComp.value;
                SetScale();
            }
        }

        public void SetScale()
        {
            Vector3 newScale = new Vector3(1, 1, 1);
            if (currDimension is < 3 and >= 0)
            {
                scaleDescComp.enabled = true;
                scaleDescComp.text = "Größe des Plots:";
                scaleDescComp.color = Color.white;
                thisSlider.enabled = true;

                switch (currDimension)
                {
                    case 0:
                        // Debug.Log("Case 0");
                        newScale = new Vector3(thisSlider.value, 1, 1);
                        break;
                    case 1:
                        // Debug.Log("Case 1");
                        newScale = new Vector3(1, thisSlider.value, 1);
                        break;                    
                    case 2:
                        // Debug.Log("Case 2");
                        newScale = new Vector3(thisSlider.value, thisSlider.value, 1);
                        break;
                    case 3:
                        // Debug.Log("Case 3");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                scaleDescComp.text = "In 3D nicht verfügbar";
                scaleDescComp.color = new Color(0.6f, 0.6f, 0.6f);
                thisSlider.enabled = false;
            }

            createPlot.transform.localScale = newScale;
        }
    }
}
