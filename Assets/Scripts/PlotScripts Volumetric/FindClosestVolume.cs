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
        
        public GameObject RenderedVolume;
        private LoadVolumes loadVolumes;
        
        
        // Start is called before the first frame update
        void Start()
        {
            createPlot = plotGameObject.GetComponent<CreatePlot>();
            loadVolumes = RenderedVolume.GetComponent<LoadVolumes>();
            thisButton = gameObject.GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void setClosestVolume()
        {
            var position = cameraPos.transform.position;
            Vector3 closestPoint =
                createPlot.GetClosestPointOnSelectedGraph(position, createPlot.selectedRun);

            int closestPointIndex =
                createPlot.GetIndexOfClosestPointOnSelectedGraph(position, createPlot.selectedRun);
                
            demoSphere.transform.position = closestPoint;
            
            loadVolumes.SetFrame(closestPointIndex);
            
            Debug.Log("Position: "+ closestPoint + ", Timestep: " + closestPointIndex);
        }
    }
}
