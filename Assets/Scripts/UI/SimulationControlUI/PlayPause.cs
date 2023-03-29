using System.Collections;
using ImportVolume;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.SimulationControlUI
{
    public class PlayPause : MonoBehaviour
    {
        public bool playing;

        public GameObject volume;
        public GameObject volume2;
        public GameObject referenceButton;
        private GameObject speedSliderGameObject;
        private UnityEngine.UI.Slider speedSlider;
    
        // Start is called before the first frame update
        void Start()
        {
            speedSliderGameObject = GameObject.Find("PlaySpeed");
            speedSlider = speedSliderGameObject.GetComponent<UnityEngine.UI.Slider>();
        }

        public void ChangeStartStop()
        {
            if (!playing)
            {
                playing = true;
                referenceButton.GetComponent<PlayPause>().playing = true;
                StartCoroutine(Play());
            }
            else 
            {
                playing = false;
                referenceButton.GetComponent<PlayPause>().playing = false;
            }

            GetComponent<UnityEngine.UI.Button>().enabled = false;
            GetComponent<UnityEngine.UI.Image>().enabled = false;
            referenceButton.GetComponent<UnityEngine.UI.Button>().enabled = true;
            referenceButton.GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

        IEnumerator Play()
        {
            while (playing)
            {
                volume.GetComponent<LoadVolumes>().timestep++;
                volume2.GetComponent<LoadVolumes>().timestep++;
                yield return new WaitForSeconds(1 / speedSlider.value);
            }
        }
    }
}


