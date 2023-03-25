using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;

namespace ImportVolume
{
    public class VolumeManager
    {
        private String dataSetPath;
        private VolumeAttribute[] volumeAttributes;
        public int currentTimeStep;
        public String dataSetName;
        private bool isReadingBinary;
        private bool forward = true;
        public GameObject referencedGameObject;


        public VolumeManager(String dataSetName)
        {
            dataSetPath = $"Assets/Datasets/{dataSetName}";
            this.dataSetName = dataSetName;
            AddVolumeAttributes();
        }
    
        public VolumeManager(String dataSetName, GameObject referencedGameObject)
        {
            dataSetPath = $"Assets/Datasets/{dataSetName}";
            this.referencedGameObject = referencedGameObject;
            AddVolumeAttributes();
        }

        private void AddVolumeAttributes()
        {
            var volumeAttributePaths =
                Directory.GetDirectories(this.dataSetPath).Where(filePath => !filePath.EndsWith("_bin")).ToArray();
            volumeAttributes = new VolumeAttribute[volumeAttributePaths.Count()];
            for (var i = 0; i < volumeAttributes.Length; i++)
            {
                volumeAttributes[i] = new VolumeAttribute(volumeAttributePaths[i]);
            }
        }

        public VolumeAttribute[] GetVolumeAttributes()
        {
            return volumeAttributes;
        }

        public void SetVisiblitiesForAttributes(bool[] visibilities)
        {
            for (var i = 0; i < visibilities.Length; i++)
            {
                volumeAttributes[i].SetVisibility(visibilities[i]);
            }
        }

        public void SetVisibilities(bool[] visibilities)
        {
            for (var i = 0; i < visibilities.Length; i++)
            {
                volumeAttributes[i].SetVisibility(visibilities[i]);
            }

            GameObject foundObject = new GameObject();
            bool found = false;
            List<String[]> names = new List<String[]>();

           
            String[] prs = new[] { "pressure", "prs", "Pressure" };
            names.Add(prs);

            String[] tev = new[] { "temperature", "tev", "Temperature" };
            names.Add(tev);

            String[] v02 = new[] { "water", "v02", "Water" };
            names.Add(v02);

            String[] v03 = new[] { "meteorite", "v03", "Meteorite" };
            names.Add(v03);

            for (var i = 0; i < visibilities.Length; i++)
            {
                
                for(int j = 0; j < names.Count; j++)
                {
                    if (names[j].Contains(volumeAttributes[i].GetName()))
                    {
                        volumeAttributes[i].SetVisibility(visibilities[j]);

                        found = true;
                        //Debug.Log("LoadVolumes.SetVisibilities: VolumeAttribute " + volumeAttributes[i].getName() + " gefunden!");
                    }
                }
            }

            if (!found)
            {
                Debug.Log("LoadVolumes.SetVisibilities: Kein VolumeAttribute gefunden!");
            }
        }

        public void NextFrame()
        {
            bool active = false;
            foreach (var volumeAttribute in volumeAttributes)
            {
                if (volumeAttribute.IsVisible())
                {
                    volumeAttribute.NextFrame();
                    active = true;
                }
            }

            if (active)
            {
                currentTimeStep++;
            }
        }


        public bool IsReadingBinary()
        {
            return isReadingBinary;
        }

        public void SetDataset(String newDatasetName)
        {
            if (dataSetPath != $"Assets/Datasets/{newDatasetName}")
            {
                dataSetPath = $"Assets/Datasets/{newDatasetName}";
                AddVolumeAttributes();
                referencedGameObject.GetComponent<LoadVolumes>().ChangeAttributeNames(this);
                GameObject.Find("TransferFunctionPanel").GetComponent<TransferFunctionPanel>().Start();
            }
        }

        public void RefreshCurrentState()
        {
            foreach (var volumeAttribute in volumeAttributes)
            {
                volumeAttribute.ClearBufferQueue();
                if (forward)
                {
                    volumeAttribute.BufferNextFrame(currentTimeStep - 1);
                }
                else
                {
                    volumeAttribute.BufferNextFrameReverse(currentTimeStep + 1);
                }
                NextFrame();
            }
        }

        public int GetCount()
        {
            int minCount = -1;
            foreach (var volumeAttribute in volumeAttributes)
            {
                if (minCount == -1 || minCount > volumeAttribute.GetCount())
                {
                    minCount = volumeAttribute.GetCount();
                }
            }
            return minCount;
        }
    }
}