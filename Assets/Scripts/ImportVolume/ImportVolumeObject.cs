using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityVolumeRendering;

public class ImportVolumeObject : MonoBehaviour
{   
    IImageFileImporter importer;

    private VolumeRenderedObject obj1;
    private VolumeRenderedObject obj2;

    private bool cycle = false;
    // Start is called before the first frame update
    void Start()
    {
        importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        VolumeDataset dataset = importer.Import("Assets/DataFiles/Testing/temperature_01132.nii");
        obj1 = VolumeObjectFactory.CreateObject(dataset);
        obj1.transform.position = new Vector3(2, 1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!cycle) {
            VolumeDataset dataset = importer.Import("Assets/DataFiles/Testing/temperature_01132.nii");
            obj2 = VolumeObjectFactory.CreateObject(dataset);
            Destroy(obj1.gameObject);
            cycle = true;
        }
        else
        {
            VolumeDataset dataset = importer.Import("Assets/DataFiles/Testing/temperature_01132.nii");
            obj1 = VolumeObjectFactory.CreateObject(dataset);
            Destroy(obj2.gameObject);
            cycle = false;
        }*/
    }
}