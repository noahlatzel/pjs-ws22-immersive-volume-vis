using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityVolumeRendering;

/// <summary>
/// For the script to work you have to copy your dataset into the Datasets/ folder. Each dataset 
/// should contain one directory for each attribute (e.g. Temperature/, Pressure/, ...) for 
/// optimal use. The script generates the binary files and stores them in Datasets/DatasetName/AttributeName_bin/
/// for further use.
/// </summary>
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
                        
                        DownScaleTexture3D(texture, 150, 150, 150);
                        
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

    void DownScaleTexture3D(Texture3D texture, int newWidth, int newHeight, int newDepth)
    {
        // Retrieve the pixel data from the texture
        Color[] pixels = texture.GetPixels();
        
        // Use a scaling algorithm to reduce the size of the pixel data
        Color[] resampledPixels = ScaleTexture(pixels, texture.width, texture.height, texture.depth, newWidth, newHeight, newDepth);

        Texture3D texture3D = new Texture3D(newWidth, newHeight, newDepth, TextureFormat.RGBA32, false);
        
        // Update the texture with the resampled pixel data
        texture3D.SetPixels(resampledPixels);
        texture3D.Apply();
    }
    
    // Scales the given 3D texture data using bicubic interpolation
    Color[] ScaleTexture(Color[] pixels, int width, int height, int depth, int newWidth, int newHeight, int newDepth)
    {
        Color[] resampledPixels = new Color[newWidth * newHeight * newDepth];

        // Precompute the scale factors
        float xScale = (float)width / newWidth;
        float yScale = (float)height / newHeight;
        float zScale = (float)depth / newDepth;

        // Iterate over the pixels of the new image
        for (int z = 0; z < newDepth; z++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    // Compute the corresponding position in the old image
                    float oldX = x * xScale;
                    float oldY = y * yScale;
                    float oldZ = z * zScale;

                    // Use bicubic interpolation to compute the color at the new position
                    resampledPixels[x + y * newWidth + z * newWidth * newHeight] = BicubicInterpolate(pixels, width, height, depth, oldX, oldY, oldZ);
                }
            }
        }

        return resampledPixels;
    }
    
    // Performs bicubic interpolation on the given 3D texture data
    Color BicubicInterpolate(Color[] pixels, int width, int height, int depth, float x, float y, float z)
    {
        // Compute the integer and fractional parts of the position
        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);
        int z0 = Mathf.FloorToInt(z);
        float dx = x - x0;
        float dy = y - y0;
        float dz = z - z0;

        // Compute the coefficients of the bicubic polynomial
        float a0 = (1 - dx) * (1 - dy) * (1 - dz);
        float a1 = dx * (1 - dy) * (1 - dz);
        float a2 = (1 - dx) * dy * (1 - dz);
        float a3 = dx * dy * (1 - dz);
        float a4 = (1 - dx) * (1 - dy) * dz;
        float a5 = dx * (1 - dy) * dz;
        float a6 = (1 - dx) * dy * dz;
        float a7 = dx * dy * dz;

        // Initialize the result to zero
        Color result = Color.clear;

        // Add the contributions of each pixel to the result
        for (int d = -1; d <= 2; d++)
        {
            for (int b = -1; b <= 2; b++)
            {
                for (int a = -1; a <= 2; a++)
                {
                    // Compute the indices of the pixel
                    int x1 = Mathf.Clamp(x0 + a, 0, width - 1);
                    int y1 = Mathf.Clamp(y0 + b, 0, height - 1);
                    int z1 = Mathf.Clamp(z0 + d, 0, depth - 1);
                    int index = x1 + y1 * width + z1 * width * height;

                    // Compute the coefficient of the pixel
                    float coefficient = a0 * (1 - a) * (1 - b) * (1 - d) +
                                       a1 * a * (1 - b) * (1 - d) +
                                       a2 * (1 - a) * b * (1 - d) +
                                       a3 * a * b * (1 - d) +
                                       a4 * (1 - a) * (1 - b) * d +
                                       a5 * a * (1 - b) * d +
                                       a6 * (1 - a) * b * d +
                                       a7 * a * b * d;

                    // Add the contribution of the pixel to the result
                    result += pixels[index] * coefficient;
                }
            }
        }

        return result;
    }

}
