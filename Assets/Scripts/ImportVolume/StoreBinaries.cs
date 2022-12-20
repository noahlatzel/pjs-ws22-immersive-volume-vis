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
            string[] volumeAttributePaths = Directory.GetDirectories(dataSetPath).Where(path => !path.EndsWith("_bin")).ToArray();

            foreach (String volumeAttributePath in volumeAttributePaths)
            {
                // Set path to store the binary files in separate directory
                // @ forces the String to be interpreted verbatim
                String binaryPath = volumeAttributePath + @"_bin/";
                
                // Get all paths to .nii files for the given attribute
                string[] fileEntries = Directory.GetFiles(volumeAttributePath, "*.nii");
                
                // Create importer to import all .nii files
                IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
                
                // Counter to keep track of added files
                int counter = 0;
                
                foreach (String file in fileEntries)
                {
                                        
                    // Extract fileName from path
                    String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";

                    // Only convert to binary if it has not been converted already
                    if (!File.Exists(binaryPath + fileName))
                    {
                        // Convert .nii to dataset and create object from it
                        VolumeDataset dataset = importer.Import(file);
                        VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                    
                        // Get Texture3D of rendered volume
                        // The volumeData is stored in the first Texture3D "_DataTex"
                        Texture3D texture = (Texture3D) obj.GetComponentInChildren<MeshRenderer>().material.GetTexture("_DataTex");
                    
                        // Extract pixel data from texture to save it 
                        // mipLevel 0 according to implementation in IImageFileImporter.Import
                        byte[] pixelData = texture.GetPixelData<byte>(0).ToArray();

                        // Save pixel data to binary file
                        File.WriteAllBytes(binaryPath + fileName, pixelData);
                        
                        // Increment counter to keep track of added files
                        counter++;
                    
                        // Destroy created object
                        Destroy(obj);
                    }
                }
                
                // Display success message when new files have been added
                if (counter > 0)
                {
                    Debug.Log($"Added {counter} binary {(counter == 1 ? "file" : "files")} from directory: {volumeAttributePath}");
                }
            }
        }
    }
}
