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

        public GameObject volumeTransformer;
        private GameObject[] volumes;
        
        public GameObject uiVolumeToggle;
        private EnableVols uiVolumeToggleComp;
        
        // Start is called before the first frame update
        void Start()
        {
            uiVolumeToggleComp = uiVolumeToggle.GetComponent<EnableVols>();
            
            createPlot = plotGameObject.GetComponent<CreatePlot>();
            
            volumes = new GameObject[volumeTransformer.transform.childCount];

            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
            }
            
            thisButton = gameObject.GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetClosestVolume()
        {
            var position = cameraPos.transform.position;
            Vector3 closestPoint =
                createPlot.GetClosestPointOnSelectedGraph(position, createPlot.selectedRun);

            int closestPointIndex =
                createPlot.GetIndexOfClosestPointOnSelectedGraph(position, createPlot.selectedRun);
                
            demoSphere.transform.position = closestPoint;
            
            for (int i = 0; i < uiVolumeToggleComp.volumeVisibility.Length; i++)
            {
                if (uiVolumeToggleComp.volumeVisibility[i])
                {
                    volumes[i].GetComponent<LoadVolumes>().SetFrame(closestPointIndex);
                }
            }
            
            Debug.Log("Position: "+ closestPoint + ", Timestep: " + closestPointIndex);
        }
    }
}
