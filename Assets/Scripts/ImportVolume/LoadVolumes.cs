using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityVolumeRendering;

namespace ImportVolume
{
    public class LoadVolumes : MonoBehaviour
    {
        [Tooltip("Specify the name of the dataset you want to use. The dataset needs to be stored in Assets/Datasets/")]
        public String datasetName = "yB11";

        [Tooltip("Show/Hide pressure volume.")]
        public bool pressure;
    
        [Tooltip("Show/Hide temperature volume.")]
        public bool temperature;
    
        [Tooltip("Show/Hide water volume.")]
        public bool water;
    
        [Tooltip("Show/Hide meteorite volume.")]
        public bool meteorite;

        [Tooltip("Start/Stop the animation.")]
        [SerializeField] public bool play;
    
        [Tooltip("Specify the frames per second of the animation.")]
        [SerializeField]
        public int timesPerSecond = 1;
    
        [Tooltip("Start/Stop the buffer.")]
        public bool isBuffering;

        public bool useScaledVersion;
    
        [Tooltip("Adjust the buffer speed (buffer per second)")]
        public int bufferSpeed = 5;

        public bool forward = true;
        public int timestep = 0;

        // Manager class
        public VolumeManager volumeManager;
        public int targetFramerate = 90;
        public bool mainScene;
        
        // Show timeStep in UI
        public GameObject timeStepUI;
        private TextMeshProUGUI tmpUI;
        private String cachedText;
        
        void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFramerate;
        }

        // Load the first volume per attribute with the importer; guarantees
        // correct configuration of Material properties etc.
        // Start is called before the first frame update
        void Start()
        {
            // Create manager class
            volumeManager = new VolumeManager(datasetName);

            // Render volumes on start 
            RenderOnStart(volumeManager);
            
            tmpUI = timeStepUI.GetComponent<TextMeshProUGUI>();
            cachedText = tmpUI.text;
            
            Debug.Log(volumeManager.IsReadingBinary());
        }

        // Update is called once per frame
        void Update()
        {
            // Update volume visibility according to public variables
            if (!mainScene)
            {
                volumeManager.SetVisibilities(new []{pressure, temperature, water, meteorite});
            }
            else
            {
                volumeManager.SetVisiblitiesForAttributes(new []{pressure, temperature, water, meteorite});
            }

            // Set time step in valid interval
            timestep = Math.Max(0, Math.Min(timestep, volumeManager.GetCount() - 1));

            StartCoroutine(LoadCurrentFrame());
            
            // Only change time step in UI if necessary (performance)
            if (mainScene && cachedText != timestep.ToString())
            {
                tmpUI.text = timestep.ToString();
                cachedText = tmpUI.text;
            }
            
            // Set frame rate
            if(Application.targetFrameRate != targetFramerate)
                Application.targetFrameRate = targetFramerate;
        }

        IEnumerator LoadCurrentFrame()
        {
            foreach(var volumeAttribute in volumeManager.GetVolumeAttributes())
            {
                // Only execute if loaded time step is not the set time step
                if (!volumeAttribute.loadingFrame && (volumeAttribute.loadedTimeStep != timestep || volumeAttribute.loadedDataSet != datasetName))
                {
                    StartCoroutine(volumeAttribute.LoadCurrentFrame(timestep, datasetName));
                }
                yield return null;
            }
        }
        
        IEnumerator LoadCurrentFrameSplit()
        {
            foreach(var volumeAttribute in volumeManager.GetVolumeAttributes())
            {
                // Only execute if loaded time step is not the set time step
                if (!volumeAttribute.loadingFrame && (volumeAttribute.loadedTimeStep != timestep || volumeAttribute.loadedDataSet != datasetName))
                {
                    StartCoroutine(volumeAttribute.LoadCurrentFrameSplit(timestep, datasetName));
                }
                yield return null;
            }
        }

        public int getCount()
        {
            int count = 0;
            if (volumeManager.GetVolumeAttributes().Length > 0)
            {
                count = volumeManager.GetVolumeAttributes()[0].GetCount();

                foreach (var volAtt in volumeManager.GetVolumeAttributes())
                {
                    int currCount = volAtt.GetCount();
                    if (currCount < count)
                    {
                        count = currCount;
                    }
                }
            }

            return count;
        }

        void RenderOnStart(VolumeManager volumeManagerObject)
        {
            // Create importer
            IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);

            foreach (VolumeAttribute volumeAttribute in volumeManagerObject.GetVolumeAttributes())
            {
                // Import dataset
                VolumeDataset dataset = importer.Import(volumeAttribute.GetFirstVolumePathForStart());
                
                // Create object from dataset
                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                
                // Transform object and set hierarchy
                Transform transform1;
                (transform1 = obj.transform).SetParent(transform, false);
                transform1.rotation = Quaternion.identity;
                transform1.localPosition = Vector3.zero;
                obj.name = volumeAttribute.GetName();
                
                // Disable MeshRenderer
                obj.GetComponentInChildren<MeshRenderer>().enabled = false;
                
                // Save each Material property and MeshRenderer for later use
                volumeAttribute.SetMaterialReference(obj.GetComponentInChildren<MeshRenderer>().material);
                volumeAttribute.SetMeshRendererReference(obj.GetComponentInChildren<MeshRenderer>());
            }
        }

    
        public void ChangeAttributeNames(VolumeManager refVolumeManager)
        {
            // Set names
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).name = refVolumeManager.GetVolumeAttributes()[i].GetName();
                refVolumeManager.GetVolumeAttributes()[i].SetMeshRendererReference(transform.GetChild(i).gameObject.GetComponentInChildren<MeshRenderer>());
                refVolumeManager.GetVolumeAttributes()[i].SetMaterialReference(transform.GetChild(i).gameObject.GetComponentInChildren<MeshRenderer>().material);
            }
        }

    }
}