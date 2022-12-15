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
    private readonly Vector3 volumePosition = new Vector3(0f, 2f, 10f);
    private int counter;
    private float timePassed;

    [SerializeField]
    private bool running;
    
    [SerializeField]
    private int timesPerSecond = 1;

    [SerializeField] private int firstVolume = 1;
    [SerializeField] private int lastVolume = 10;

    // Start is called before the first frame update
    void Start()
    {
        string[] fileEntries = Directory.GetFiles("Assets/DataFiles/Testing/", "*.nii");
        importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        
        lastVolume = lastVolume > fileEntries.Length ? fileEntries.Length : lastVolume;
        for (int i = firstVolume - 1; i < lastVolume ; i++)     // i = firstVolume - 1 because array starts at index 0
        {
            VolumeDataset dataset = importer.Import(fileEntries[i]);                // ~1.9s per Object
            VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);   // ~2.8s per Object
            obj.transform.SetParent(transform);
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponentInChildren<MeshRenderer>().enabled = false;
            volumeRenderedObjects.Add(obj);
        }

        volumeRenderedObjects[0].GetComponentInChildren<MeshRenderer>().enabled = true;
        transform.position = volumePosition;
        timePassed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float dur = 1f / timesPerSecond;
        timePassed += Time.deltaTime;
        if (timePassed >= dur)
        {
            timePassed -= dur;
            if (running)
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
    }
}