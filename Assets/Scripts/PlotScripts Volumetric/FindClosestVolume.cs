using UnityEngine;
using UnityEngine.UI;

namespace PlotScripts_Volumetric
{
    public class FindClosestVolume : MonoBehaviour
    {
        [Tooltip("Assign plotObject.")] public GameObject plotGameObject;

        public GameObject cameraPos;

        public GameObject demoSphere;
        
        private CreatePlot createPlot;

        private UnityEngine.UI.Button thisButton;
        
        
        
        // Start is called before the first frame update
        void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();
            thisButton = gameObject.GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void pressButton()
        {
            Vector3 closestPoint =
                createPlot.GetClosestPointOnSelectedGraph(cameraPos.transform.position, 4);
            
            demoSphere.transform.position = closestPoint;
            
            Debug.Log(closestPoint);
        }
    }
}
