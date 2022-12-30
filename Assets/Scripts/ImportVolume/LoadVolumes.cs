using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityVolumeRendering;

public class LoadVolumes : MonoBehaviour
{
    [Tooltip("Specify the name of the dataset you want to use. The dataset needs to be stored in Assets/Datasets/")]
    public String datasetName = "Dataset1";

    [Tooltip("Show/Hide pressure volume.")]
    public bool pressure;
    
    [Tooltip("Show/Hide temperature volume.")]
    public bool temperature;
    
    [Tooltip("Show/Hide water volume.")]
    public bool water;
    
    [Tooltip("Show/Hide meteorite volume.")]
    public bool meteorite;
    
    private float timePassed = 0;
    
    [Tooltip("Start/Stop the animation.")]
    [SerializeField] private bool play;
    
    [Tooltip("Specify the frames per second of the animation.")]
    [SerializeField]
    private int timesPerSecond = 1;
    
    [Tooltip("Start/Stop the buffer.")]
    public bool isBuffering;
    
    // Manager class
    private VolumeManager volumeManager;
    
    // Load the first volume per attribute with the importer; guarantees
    // correct configuration of Material properties etc.
    // Start is called before the first frame update
    void Start()
    {
        // Create manager class
        volumeManager = new VolumeManager(datasetName);
        
        // Render volumes on start 
        RenderOnStart(volumeManager);
    }
    
    // Update is called once per frame
    void Update()
    {
        // Update volume visibility according to public variables
        volumeManager.SetVisibilities(new []{pressure, temperature, water, meteorite});
        
        // Only execute as often as specified in timesPerSecond
        float dur = 1f / timesPerSecond;
        timePassed += Time.deltaTime;
        
        if (timePassed >= dur)
        {
            timePassed -= dur;

            if (play)
            {
                volumeManager.NextFrame();
                Debug.Log("Next Frame!");
            }
        }
        
        if (!volumeManager.IsReadingBinary() && isBuffering)
        {
            // Start the binary reading thread
            //StartBufferThread("Assets/Datasets/Dataset1/Pressure_bin");
            
            var t = new Thread(() => volumeManager.BufferNextFrame());
            t.Start();
        }
    }

    private void RenderOnStart(VolumeManager volumeManagerObject)
    {
        // Create importer
        IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        
        foreach (VolumeAttribute volumeAttribute in volumeManagerObject.GetVolumeAttributes())
        {
            // Import dataset
            VolumeDataset dataset = importer.Import(volumeAttribute.GetFirstVolumePathForStart());
                
            // Create object from dataset
            VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                
            // Transform object and set hierarchy
            obj.transform.SetParent(transform);
            obj.transform.rotation = Quaternion.identity;
            obj.name = volumeAttribute.GetName();
                
            // Disable MeshRenderer
            obj.GetComponentInChildren<MeshRenderer>().enabled = false;
                
            // Save each Material property and MeshRenderer for later use
            volumeAttribute.SetMaterialReference(obj.GetComponentInChildren<MeshRenderer>().material);
            volumeAttribute.SetMeshRendererReference(obj.GetComponentInChildren<MeshRenderer>());
        }
        

    }
}

public class LimitedStack<T>
{
    private T[] items;
    private int top = 0;

    public LimitedStack(int capacity)
    {
        items = new T[capacity];
    }

    public void Push(T item)
    {
        items[top] = item;
        top = (top + 1) % items.Length;
    }

    public T Pop()
    {
        top = (items.Length + top - 1) % items.Length;
        return items[top];
    }
}


public class VolumeAttribute
{
    private readonly String name;
    private bool active;
    private readonly String[] filePaths;
    private Material material;
    private MeshRenderer meshRenderer;
    private readonly Queue<byte[]> bufferQueue;
    private readonly int count;
    private readonly String originalPath;

    public VolumeAttribute(String path)
    {
        name = new DirectoryInfo(path).Name;
        filePaths = Directory.GetFiles(path + "_bin/", "*.bin")
            .Where(filePath => !filePath.EndsWith("_sub.bin")).ToArray();
        count = filePaths.Length;
        originalPath = path;
        bufferQueue = new Queue<byte[]>();
    }

    public String GetVolumePath(int number)
    {
        return filePaths[number];
    }

    public String GetFirstVolumePathForStart()
    {
        return Directory.GetFiles(originalPath, "*.nii")[0];
    }

    public String GetName()
    {
        return name;
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
        // Set the current texture to the next texture in the buffer
        ((Texture3D) material.GetTexture("_DataTex")).SetPixelData(bufferQueue.Dequeue(), 0);
    }

    // Loads the binary from the specified path and stores the loaded byte array in a queue.
    // Loading the binary has to be separated from the main function because Unity functions classes
    // may not be called in other threads than the main thread but we want to load our data in a separate 
    // thread to reduce lag.
    public void BufferNextFrame(int currentTimeStep)
    {
        if (bufferQueue.Count < 5)
        {
            // Load pixelData from binary file at next position
            int nextVolumeToBuffer = (currentTimeStep + 1 + bufferQueue.Count) % count;
            byte[] pixelData = File.ReadAllBytes(filePaths[nextVolumeToBuffer]);
        
            // Store byte array in queue
            bufferQueue.Enqueue(pixelData);
        }
    }
}

public class VolumeManager
{
    private readonly String dataSetPath;
    private VolumeAttribute[] volumeAttributes;
    private int currentTimeStep;
    private bool isReadingBinary;

    public VolumeManager(String dataSetName)
    {
        dataSetPath = $"Assets/Datasets/{dataSetName}";
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

    public void SetVisibilities(bool[] visibilities)
    {
        for (var i = 0; i < visibilities.Length; i++)
        {
            volumeAttributes[i].SetVisibility(visibilities[i]);
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
            }

            active = true;
        }

        if (active)
        {
            currentTimeStep++;
        }
    }

    public void BufferNextFrame()
    {
        // Set the flag to indicate that the reading operation is in progress
        isReadingBinary = true;
        
        foreach (var volumeAttribute in volumeAttributes)
        {
            volumeAttribute.BufferNextFrame(currentTimeStep);
        }
                            
        // Clear the flag to indicate that the reading operation has completed
        isReadingBinary = false;
    }

    public bool IsReadingBinary()
    {
        return isReadingBinary;
    }
}