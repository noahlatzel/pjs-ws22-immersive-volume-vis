using System;
using System.IO;
using UnityEngine;
using UnityVolumeRendering;

public class LoadVolumes : MonoBehaviour
{
    [Tooltip("Specify the name of the dataset you want to use. The dataset needs to be stored in Assets/Datasets/")]
    public String datasetName = "Dataset1";

    public String pressureDirectory = "Pressure";
    public String temperatureDirectory = "Temperature";
    public String waterDirectory = "Water";
    public String meteoriteDirectory = "Meteorite";
    
    // Load the first volume per attribute with the importer guarantee
    // correct configuration of Material properties etc.
    // Start is called before the first frame update
    void Start()
    {
        // Generate array of all directories to use it in for-loop
        String[] directories = { temperatureDirectory, pressureDirectory, waterDirectory, meteoriteDirectory};
        String[] volumeAttributeNames = { "Temperature", "Pressure", "Water", "Meteorite"};
        String datasetPath = "Assets/Datasets/";
        
        // Create importer
        IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);

        for (int i = 0; i < directories.Length; i++)
        {
            // Get first volume
            String[] volumes = Directory.GetFiles(datasetPath + datasetName + "/" + directories[i], "*.nii");

            if (volumes.Length > 0)
            {
                // Import dataset
                VolumeDataset dataset = importer.Import(volumes[0]);
                
                // Create object from dataset
                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                
                // Transform object and set hierarchy
                obj.transform.SetParent(transform);
                obj.transform.rotation = Quaternion.identity;
                obj.name = volumeAttributeNames[i];
                
                // Disable MeshRenderer
                obj.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
