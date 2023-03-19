using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityVolumeRendering;
using Debug = UnityEngine.Debug;

namespace ImportVolume
{
    /// <summary>
    /// For the script to work you have to copy your dataset into the Datasets/ folder. Each dataset 
    /// should contain one directory for each attribute (e.g. Temperature/, Pressure/, ...) for 
    /// optimal use. The script generates the binary files and stores them in Datasets/DatasetName/AttributeName_bin/
    /// for further use. The down-sampled textures are stored with the postfix _sub.
    /// </summary>
    public class StoreBinaries : MonoBehaviour
    {
        [Tooltip("Specify the side length for the scaled version of the volume.")]
        public int sideLength = 100;

        private readonly Queue<ProcessElementStruct> processingQueue = new();
        private IImageFileImporter importer;
        private int counter;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private int initialCount;
        
        // Start is called before the first frame update
        void Start()
        {
            // Create importer to import all .nii files
            importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
            
            // Get all dataset paths
            string[] dataSetPaths = Directory.GetDirectories("Assets/Datasets/");
        
            foreach (String dataSetPath in dataSetPaths)
            {
                // Get all paths to the (four) volume attributes for each dataset
                string[] volumeAttributePaths = Directory.GetDirectories(dataSetPath).Where(path => !path.EndsWith("_bin")).ToArray();
            
                foreach (String volumeAttributePath in volumeAttributePaths)
                {
                    // Create stopwatch for performance measures
                    Stopwatch stopWatch = Stopwatch.StartNew();
                
                    // Set path to store the binary files in separate directory
                    // @ forces the String to be interpreted verbatim
                    String binaryPath = volumeAttributePath + @"_bin/";
                    if (!Directory.Exists(binaryPath))
                    {
                        Directory.CreateDirectory(binaryPath);
                        Debug.Log("Missing directory " + binaryPath + " successfully created.");
                    }

                    // Get all paths to .nii files for the given attribute
                    string[] fileEntries = Directory.GetFiles(volumeAttributePath, "*.nii");
                
                    // Create importer to import all .nii files
                    importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
                
                    // Counter to keep track of added files
                    //int counter = 0;
                
                    foreach (String file in fileEntries)
                    {
                        // Extract fileName from path
                        String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";
                        
                        if (!File.Exists(binaryPath + fileName))
                        {
                            ProcessElementStruct processElement = new ProcessElementStruct
                            {
                                binaryPath = binaryPath,
                                fileEntry = file
                            };

                            processingQueue.Enqueue(processElement); 
                        }
                        /*
                        // Extract fileName from path
                        String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";
                        String subFilename = Path.GetFileNameWithoutExtension(file) + "_sub.bin";

                        // Only convert to binary if it has not been converted already
                        if (!File.Exists(binaryPath + fileName))
                        {
                            // Convert .nii to dataset and create object from it
                            VolumeDataset dataset = importer.Import(file);
                            VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                    
                            // Get Texture3D of rendered volume
                            // The volumeData is stored in the first Texture3D "_DataTex"
                            Texture3D texture = (Texture3D) obj.GetComponentInChildren<MeshRenderer>().material.GetTexture("_DataTex");
                        
                            // Scale down the Texture3D of the rendered volume
                            Texture3D subTexture = DownScaleTexture3D(texture, sideLength, 
                                sideLength, sideLength);
                        
                            // Extract pixel data from texture to save it 
                            // mipLevel 0 according to implementation in IImageFileImporter.Import
                            byte[] pixelData = texture.GetPixelData<byte>(0).ToArray();
                            byte[] subPixelData = subTexture.GetPixelData<byte>(0).ToArray();

                            // Save pixel data to binary file
                            File.WriteAllBytes(binaryPath + fileName, pixelData);
                            File.WriteAllBytes(binaryPath + subFilename, subPixelData);
                        
                            // Increment counter to keep track of added files
                            counter++;
                    
                            // Destroy created object 
                            Destroy(obj);
                            Destroy(GameObject.Find("VolumeRenderedObject_test"));
                        }
                        */
                    }
                    stopWatch.Stop();
                
                    // Display success message when new files have been added and measure the time
                    // Replace "\" with "/" for uniform look
                    /*if (counter > 0)
                    {
                        Debug.Log($"Added {counter} binary {(counter == 1 ? "file" : "files")} from directory: {volumeAttributePath.Replace(@"\", "/")} " +
                                  $"and scaled down each texture in {stopWatch.Elapsed:m\\:ss\\.fff}.");
                    }*/
                }
            }
            initialCount = processingQueue.Count;
            stopwatch.Start();
        }

        private void Update()
        {
            if (processingQueue.Count > 0)
            {
                PreprocessVolume(processingQueue.Dequeue());
                TimeSpan timePerVolume = stopwatch.Elapsed / ++counter;
                TimeSpan remainingTime = timePerVolume * processingQueue.Count;
                DateTime finishedUntil = DateTime.Now.Add(remainingTime);
                Debug.Log($"Processed {counter}/{initialCount} volumes in {stopwatch.Elapsed:hh\\:mm\\:ss} | time per volume {timePerVolume:mm\\:ss\\.fff} | approx. remaining time: {remainingTime:hh\\:mm\\:ss} | finished until {finishedUntil:HH:mm:ss}");
            }
        }

        void PreprocessVolume(ProcessElementStruct processElementStruct)
        {
            String file = processElementStruct.fileEntry;
            String binaryPath = processElementStruct.binaryPath;
            
            // Extract fileName from path
            String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";
            String subFilename = Path.GetFileNameWithoutExtension(file) + "_sub.bin";

            // Only convert to binary if it has not been converted already
            if (!File.Exists(binaryPath + fileName))
            {
                // Convert .nii to dataset and create object from it
                VolumeDataset dataset = importer.Import(file);
                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
        
                // Get Texture3D of rendered volume
                // The volumeData is stored in the first Texture3D "_DataTex"
                Texture3D texture = (Texture3D) obj.GetComponentInChildren<MeshRenderer>().material.GetTexture("_DataTex");
            
                // Scale down the Texture3D of the rendered volume
                Texture3D subTexture = DownScaleTexture3D(texture, sideLength, 
                    sideLength, sideLength);
            
                // Extract pixel data from texture to save it 
                // mipLevel 0 according to implementation in IImageFileImporter.Import
                byte[] pixelData = texture.GetPixelData<byte>(0).ToArray();
                byte[] subPixelData = subTexture.GetPixelData<byte>(0).ToArray();

                // Save pixel data to binary file
                File.WriteAllBytes(binaryPath + fileName, pixelData);
                File.WriteAllBytes(binaryPath + subFilename, subPixelData);

                // Destroy created object 
                Destroy(obj);
                Destroy(GameObject.Find("VolumeRenderedObject_test"));
            }
        }

        // Scales the given 3D texture down and returns a new 3D texture.
        Texture3D DownScaleTexture3D(Texture3D texture, int newWidth, int newHeight, int newDepth)
        {
            // Retrieve the pixel data from the texture
            Color[] pixels = texture.GetPixels();
        
            // Use a scaling algorithm to reduce the size of the pixel data
            Color[] resampledPixels = ScaleTexture(pixels, texture.width, texture.height, texture.depth, newWidth, newHeight, newDepth);
        
            // Copied from Importer
            TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
            // Create new Texture3D with given size
            Texture3D texture3D = new Texture3D(newWidth, newHeight, newDepth, texFormat, false);
        
            // Update the texture with the resampled pixel data
            // No need to call texture3D.Apply() because we don't use the texture directly.
            // We just want the binary data.
            texture3D.SetPixels(resampledPixels);
        
            return texture3D;
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

    struct ProcessElementStruct
    {
        public String binaryPath;
        public String fileEntry;
    }
}
