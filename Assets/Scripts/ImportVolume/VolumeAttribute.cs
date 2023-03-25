using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImportVolume
{
    public class VolumeAttribute
    {
        private String name;
        private bool active;
        private String[] filePaths;
        private readonly String[] filePathsScaled;
        private Material material;
        private MeshRenderer meshRenderer;
        private readonly Queue<byte[]> bufferQueue;
        private readonly LimitedStack<Texture3D> bufferStack;
        private int count;
        private String originalPath;
        private bool usingScaled = false;
        private readonly Assembly dll;
        private static readonly int DataTex = Shader.PropertyToID("_DataTex");
        private MaterialPropertyBlock materialPropertyBlock;
        
        public int loadedTimeStep = -1;
        public String loadedDataSet = "";
        public bool loadingFrame;
        
        public VolumeAttribute(String path)
        {
            name = new DirectoryInfo(path).Name;
            filePaths = Directory.GetFiles(path + "_bin/", "*.bin")
                .Where(filePath => !filePath.EndsWith("_sub.bin")).ToArray();
            filePathsScaled = Directory.GetFiles(path + "_bin/", "*_sub.bin");
            count = filePaths.Length;
            originalPath = path;
            bufferQueue = new Queue<byte[]>();
            bufferStack = new LimitedStack<Texture3D>(10);
    
            // Load the DLL into the Assembly object
            dll = Assembly.LoadFrom("Assets/DLLs/ThreadedBinaryReader.dll");
        }

        public IEnumerator LoadCurrentFrameSplit(int timeStep, String dataSet)
        {
            if (IsVisible())
            {
                loadingFrame = true;
            
                for (int i = 0; i < 10; i++)
                {
                    String path = filePaths[timeStep * 10 + i];
                    byte[] split = File.ReadAllBytes(path);
                    yield return null;
                    Texture3D currentTexture = (Texture3D) material.GetTexture($"_DataTex{i}");
                    currentTexture.wrapMode = TextureWrapMode.Clamp;
                    currentTexture.SetPixelData(split, 0);
                    yield return null;
                    currentTexture.Apply();
                    material.SetTexture($"_DataTex{i}", currentTexture);
                }

                // Set loaded time step to time step
                loadedTimeStep = timeStep;
                loadedDataSet = dataSet;
                loadingFrame = false;
            }
        }
        
        public IEnumerator LoadCurrentFrame(int timeStep, String dataSet)
        {
            if (IsVisible())
            {
                if (loadedDataSet != dataSet)
                {
                    Debug.Log(dataSet);
                    filePaths = Directory.GetFiles("Assets/Datasets/" + dataSet + "/" + name + "_bin/", "*.bin")
                        .Where(filePath => !filePath.EndsWith("_sub.bin")).ToArray();
                    count = filePaths.Length;
                    timeStep = Math.Min(timeStep, count - 1);
                }

                loadingFrame = true;

                String path = filePaths[timeStep];
                byte[] byteData = {};
                byteData = File.ReadAllBytes(path);
                yield return null;
                Texture3D currentTexture = (Texture3D) material.GetTexture(DataTex);
                currentTexture.wrapMode = TextureWrapMode.Clamp;
                currentTexture.SetPixelData(byteData, 0);
                yield return null;
                currentTexture.Apply();
                material.SetTexture(DataTex, currentTexture);
                
                // Set loaded time step to time step
                loadedTimeStep = timeStep;
                loadedDataSet = dataSet;
                loadingFrame = false;
            }
        }
        
        private ushort[] ReadUShortArray(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at the specified path.", path);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                int numberOfUShorts = (int)(fileStream.Length / sizeof(ushort));
                ushort[] ushortArray = new ushort[numberOfUShorts];

                for (int i = 0; i < numberOfUShorts; i++)
                {
                    ushortArray[i] = binaryReader.ReadUInt16();
                }

                return ushortArray;
            }
        }
        
        private String GetVolumePath(int number)
        {
            if (number < 0)
            {
                number = count + number;
            }

            return usingScaled ? filePathsScaled[number] : filePaths[number];
        }

        public String GetFirstVolumePathForStart()
        {
            return Directory.GetFiles(originalPath, "*.nii")[0];
        }

        public String GetName()
        {
            return name;
        }

        public int GetCount()
        {
            return count;
        }

        public void SetMaterialReference(Material materialRef)
        {
            material = materialRef;
        }

        public void SetMeshRendererReference(MeshRenderer meshRendererRef)
        {
            meshRenderer = meshRendererRef;
        }

        public void SetVisibility(bool visibility)
        {
            meshRenderer.enabled = visibility;
            active = visibility;
        }

        public bool IsVisible()
        {
            return active;
        }

        public void NextFrame()
        {
            // Check if textures are buffered
            if (bufferQueue.Count > 0)
            {
                // Set the current texture to the next texture in the buffer depended on usingScaled
                //Texture3D newTexture = usingScaled ? new Texture3D(100, 100, 100, texFormat, false) : 
                //    new Texture3D(300, 300, 300, texFormat, false);
            
                // Get the currently loaded texture
                Texture3D currentTexture = (Texture3D) material.GetTexture(DataTex);
            
                // Save current Texture3D to bufferStack (for PreviousFrame())
                bufferStack.Push(currentTexture);

                // Set pixel data from bufferQueue
                currentTexture.SetPixelData(bufferQueue.Dequeue(), 0);

                // Set _DataTex texture of material to newly loaded texture
                material.SetTexture(DataTex, currentTexture);

                // Upload new texture to GPU -> major bottleneck, can not be called async/in coroutine/ in
                // a separate thread
                currentTexture.Apply();
            }
        }

        public void ClearBufferQueue()
        {
            bufferQueue.Clear();
        }

        // Loads the binary from the specified path and stores the loaded byte array in a queue.
        // Loading the binary has to be separated from the main function because Unity functions classes
        // may not be called in other threads than the main thread but we want to load our data in a separate 
        // thread to reduce lag.
        public void BufferNextFrame(int currentTimeStep)
        {
            // Restrict buffer size
            if (bufferQueue.Count <= 10)
            {
                // Load pixelData from binary file at next position
                int nextVolumeToBuffer = (currentTimeStep + 1 + bufferQueue.Count) % count;
            
                dll.GetType("ThreadedBinaryReader.FileReader").GetMethod("ReadFileInThread")
                    ?.Invoke(null, new object[] { GetVolumePath(nextVolumeToBuffer), bufferQueue });
            }
        }
    
        // Loads the binary from the specified path and stores the loaded byte array in a queue.
        // Loading the binary has to be separated from the main function because Unity functions classes
        // may not be called in other threads than the main thread but we want to load our data in a separate 
        // thread to reduce lag.
        public void BufferNextFrameReverse(int currentTimeStep)
        {
            // Restrict buffer size
            if (bufferQueue.Count <= 10)
            {
                // Load pixelData from binary file at next position
                int nextVolumeToBuffer = (currentTimeStep - 1 - bufferQueue.Count) % count;
                dll.GetType("ThreadedBinaryReader.FileReader").GetMethod("ReadFileInThread")
                    ?.Invoke(null, new object[] { GetVolumePath(nextVolumeToBuffer), bufferQueue });
            }
        }

        public void SetUsingScale(bool usage)
        {
            // Check if scale was changed during runtime
            if (usingScaled != usage)
            {
                // Clear bufferQueue during runtime
                bufferQueue.Clear();
            
                // Clear bufferStack during runtime
                while (bufferStack.Count() > 0)
                {
                    bufferStack.Pop();
                }
            }
            usingScaled = usage;
        }
    }
}