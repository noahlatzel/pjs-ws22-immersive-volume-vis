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

    private Material[] materials = new Material[4];

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
                
                // Save each Material property for later use
                materials[i] = obj.GetComponentInChildren<MeshRenderer>().material;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Loads binaries in original size and in scaled down size.
    // Usage: - materials[0].SetTexture("_DataTex",LoadBinaryToTexture3D(300, 300, 300,
    //          "Assets/Datasets/Dataset1/Pressure_bin/prs_098.bin"));
    //        - materials[0].SetTexture("_DataTex",LoadBinaryToTexture3D(100, 100, 100,
    //          "Assets/Datasets/Dataset1/Pressure_bin/prs_098.bin"));
    // TODO Slight offset when rendering the volume parts from the bottom are rendered at the top, needs fix
    Texture3D LoadBinaryToTexture3D(int width, int height, int depth, String path)
    {
        // Copied from Importer
        TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
        // Create Texture3D object
        Texture3D texture = new Texture3D(width, height, depth, texFormat, false);
        
        // Load pixelData from binary file
        byte[] pixelData = File.ReadAllBytes(path);
        
        // Set pixel data to loaded binary data with
        // mipLevel 0 according to implementation in IImageFileImporter.Import
        texture.SetPixelData(pixelData, 0);
        
        // Apply texture to load in GPU
        texture.Apply();
        
        return texture;
    }
}
