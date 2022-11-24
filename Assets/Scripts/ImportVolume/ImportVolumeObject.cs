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
            volumeRenderedObjects.Add(obj);
        }

        renderedObject = volumeRenderedObjects[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (cycle)
        {
            if (counter == volumeRenderedObjects.Count() - 1)
                counter = 0;
            }
            else
            {
                counter++;
            }
            renderedObject = volumeRenderedObjects[counter];
        }
}