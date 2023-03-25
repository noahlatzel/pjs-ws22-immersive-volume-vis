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
        private int count;
        private String originalPath;
        private bool usingScaled = false;
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

        private bool IsVisible()
        {
            return active;
        }
    }
}