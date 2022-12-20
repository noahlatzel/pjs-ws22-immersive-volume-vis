using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityVolumeRendering;

public class StoreBinaries : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get all dataset paths
        string[] dataSetPaths = Directory.GetDirectories("Assets/Datasets/");
        
        foreach (String dataSetPath in dataSetPaths)
        {
            // Get all paths to the (four) volume attributes for each dataset
            string[] volumeAttributePaths = Directory.GetDirectories(dataSetPath).Where(name => !name.EndsWith("_bin")).ToArray();

            foreach (String volumeAttributePath in volumeAttributePaths)
            {
                // Set path to store the binary files in separate directory
                String binaryPath = volumeAttributePaths + "_bin";
                
                // Get all paths to .nii files for the given attribute
                string[] fileEntries = Directory.GetFiles(volumeAttributePath, "*.nii");
                
                // Create importer to import all .nii files
                IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);

                foreach (String file in fileEntries)
                {
                    // Convert .nii to dataset and create object from it
                    VolumeDataset dataset = importer.Import(file);
                    VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                    
                    // Get Texture3D of rendered volume
                    // The volumeData is stored in the first Texture3D "_dataTex"
                    // Access by Integer is faster than by String.
                    Texture3D texture = obj.GetComponentInChildren<MeshRenderer>().material.GetTexture(0);
                    
                    // Extract pixel data from texture to save it 
                    byte[] pixelData = texture.GetPixelData<byte>(0).ToArray();
                    
                    // Save pixel data to binary file
                    File.WriteAllBytes(binaryPath, pixelData);
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
