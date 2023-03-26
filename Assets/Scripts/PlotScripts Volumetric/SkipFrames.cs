using ImportVolume;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    public class SkipFrames : MonoBehaviour
    {
        [Tooltip("Anzahl an Frames welche uebersprungen werden soll.")]
        public int amountOfFrames;

        public GameObject volumeTransformer;

        public GameObject uiVolumeToggle;
        private EnableVols uiVolumeToggleComp;
        private GameObject[] volumes;

        // Start is called before the first frame update
        private void Start()
        {
            volumes = new GameObject[volumeTransformer.transform.childCount];

            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = volumeTransformer.transform.GetChild(i).gameObject;
            }
            
            uiVolumeToggleComp = uiVolumeToggle.GetComponent<EnableVols>();
        }

        public void skipFrames()
        {
            foreach (var volumeAttribute in volumes)
            {
                volumeAttribute.GetComponent<LoadVolumes>().timestep += amountOfFrames;
            }

            /*for (var i = 0; i < uiVolumeToggleComp.volumeVisibility.Length; i++)
                if (uiVolumeToggleComp.volumeVisibility[i])
                {
                    var currComp = volumes[i].GetComponent<LoadVolumes>();
                    var newTimestep = currComp.timestep + amountOfFrames;
                    if (currComp.getCount() > newTimestep && newTimestep >= 0)
                    {
                        volumes[i].GetComponent<LoadVolumes>().timestep += amountOfFrames;
                        Debug.Log("Neuer Timestep: " + volumes[i].GetComponent<LoadVolumes>().timestep);
                    }
                    else
                        Debug.Log("Frameskip nicht möglich, Out of Bounds.");
                }*/
        }
    }
}