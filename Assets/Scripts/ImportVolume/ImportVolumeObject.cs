using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityVolumeRendering;

public class ImportVolumeObject : MonoBehaviour
{
    IImageFileImporter importer;

    private VolumeRenderedObject renderedObject;
    private List<VolumeRenderedObject> volumeRenderedObjects = new List<VolumeRenderedObject>();
    Vector3 volumePosition = new Vector3(2.5f, 0.75f, 2.5f);
    private int counter = 0;
    [SerializeField]
    private bool cycle = false;

    // Start is called before the first frame update
    void Start2()
    {
        importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        VolumeDataset dataset = importer.Import("Assets/DataFiles/Testing/tev_051.nii");
        renderedObject = VolumeObjectFactory.CreateObject(dataset);
        renderedObject.transform.position = volumePosition;
    }

    void Start()
    {
        string[] fileEntries = Directory.GetFiles("Assets/DataFiles/Testing/", "*.nii");
        importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        foreach (string filePath in fileEntries)
        {
            VolumeDataset dataset = importer.Import(filePath);
            VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
            obj.transform.position = volumePosition;
            obj.GetComponentInChildren<MeshRenderer>().enabled = false;
            volumeRenderedObjects.Add(obj);
        }

        volumeRenderedObjects[0].GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    // Update is called once per frame
    private int countFrames;

    void Update()
    {
        if (countFrames % 20 == 0)
        {
            if (cycle)
            {
                volumeRenderedObjects[counter].GetComponentInChildren<MeshRenderer>().enabled = false;
                if (counter == volumeRenderedObjects.Count() - 1)
                {
                    counter = 0;
                }
                else
                {
                    counter++;
                }
                volumeRenderedObjects[counter].GetComponentInChildren<MeshRenderer>().enabled = true;
            }
        }
        countFrames++;
    }
}