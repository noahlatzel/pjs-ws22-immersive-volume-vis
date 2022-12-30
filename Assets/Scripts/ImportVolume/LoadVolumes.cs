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
    
    [Tooltip("Specify the name of the pressure directory. The directory needs to be stored in Assets/Datasets/<DatasetName>")]
    public String pressureDirectory = "Pressure";
    
    [Tooltip("Specify the name of the temperature directory. The directory needs to be stored in Assets/Datasets/<DatasetName>")]
    public String temperatureDirectory = "Temperature";
    
    [Tooltip("Specify the name of the water directory. The directory needs to be stored in Assets/Datasets/<DatasetName>")]
    public String waterDirectory = "Water";
    
    [Tooltip("Specify the name of the meteorite directory. The directory needs to be stored in Assets/Datasets/<DatasetName>")]
    public String meteoriteDirectory = "Meteorite";
    
    [Tooltip("Show/Hide pressure volume.")]
    public bool pressure;
    
    [Tooltip("Show/Hide temperature volume.")]
    public bool temperature;
    
    [Tooltip("Show/Hide water volume.")]
    public bool water;
    
    [Tooltip("Show/Hide meteorite volume.")]
    public bool meteorite;

    private Material[] materials = new Material[4];
    private MeshRenderer[] meshRenderers = new MeshRenderer[4];

    
    // Initialized in Start() with bufferSize
    // TODO Use Stack for future and Queue for past instead
    private int currentTimeStep = 0;
    private float timePassed = 0;
    
    [Tooltip("Start/Stop the animation.")]
    [SerializeField] private bool play;
    
    [Tooltip("Specify the frames per second of the animation.")]
    [SerializeField]
    private int timesPerSecond = 1;
    
    public int bufferStackSize = 10;
    public int bufferSize = 10;
    private LimitedStack<Texture3D> pastStack;

    private Queue<byte[]> binaryBuffer;
    private bool isReadingBinary;
    
    [Tooltip("Start/Stop the buffer.")]
    public bool isBuffering;

    // Load the first volume per attribute with the importer guarantee
    // correct configuration of Material properties etc.
    // Start is called before the first frame update
    void Start()
    {
        // Generate array of all directories to use it in for-loop
        String[] directories = { temperatureDirectory, pressureDirectory, waterDirectory, meteoriteDirectory};
        String[] volumeAttributeNames = { "Temperature", "Pressure", "Water", "Meteorite"};
        String datasetPath = "Assets/Datasets/";

        // Create importer
        IImageFileImporter importer = ImporterFactory.CreateImageFileImporter(ImageFileFormat.NIFTI);
        
        // Initialize buffers
        pastStack = new LimitedStack<Texture3D>(bufferStackSize);
        binaryBuffer = new Queue<byte[]>();
        
        for (int i = 0; i < directories.Length; i++)
        {
            // Get first volume
            String[] volumes = Directory.GetFiles(datasetPath + datasetName + "/" + directories[i], "*.nii");

            if (volumes.Length > 0)
            {
                // Import dataset
                VolumeDataset dataset = importer.Import(volumes[0]);
                
                // Create object from dataset
                VolumeRenderedObject obj = VolumeObjectFactory.CreateObject(dataset);
                
                // Transform object and set hierarchy
                obj.transform.SetParent(transform);
                obj.transform.rotation = Quaternion.identity;
                obj.name = volumeAttributeNames[i];
                
                // Disable MeshRenderer
                obj.GetComponentInChildren<MeshRenderer>().enabled = false;
                
                // Save each Material property and MeshRenderer for later use
                materials[i] = obj.GetComponentInChildren<MeshRenderer>().material;
                meshRenderers[i] = obj.GetComponentInChildren<MeshRenderer>();
            }
        }
    }

    // TODO Load binaries at runtime and use buffer DropOutStack?
    // Update is called once per frame
    void Update()
    {
        // Update volume visibility according to public variables
        meshRenderers[0].enabled = pressure;
        meshRenderers[1].enabled = temperature;
        meshRenderers[2].enabled = water;
        meshRenderers[3].enabled = meteorite;
        
        // Only execute as often as specified in timesPerSecond
        float dur = 1f / timesPerSecond;
        timePassed += Time.deltaTime;
        
        if (timePassed >= dur)
        {
            timePassed -= dur;
            if (play && binaryBuffer.Count > 0)
            {
                // Set the current texture to the next texture in the buffer
                ((Texture3D) materials[0].GetTexture("_DataTex")).SetPixelData(binaryBuffer.Dequeue(), 0);
                
                // Increase current time step. Start from beginning if it reached the end.
                // Get filepath of all volumes 
                String[] filePaths = Directory.GetFiles("Assets/Datasets/Dataset1/Pressure_bin", "*.bin").Where(filepath => !filepath.EndsWith("sub.bin")).ToArray();
                currentTimeStep = (currentTimeStep + 1) % filePaths.Length;
            }
        }
        
        if (!isReadingBinary && isBuffering && binaryBuffer.Count < bufferSize)
        {
            // Start the binary reading thread
            StartBufferThread("Assets/Datasets/Dataset1/Pressure_bin");
        }
    }
    
    // Loads the binary from the specified path and stores the loaded byte array in a queue.
    // Loading the binary has to be separated from the main function because Unity functions classes
    // may not be called in other threads than the main thread but we want to load our data in a separate 
    // thread to reduce lag.
    private void LoadAndStoreBinary(String path)
    {
        Debug.Log("Started thread!");
        
        // Set the flag to indicate that the reading operation is in progress
        isReadingBinary = true;
        
        // Get filepath of all volumes 
        String[] filePaths = Directory.GetFiles(path, "*.bin").Where(filepath => !filepath.EndsWith("sub.bin")).ToArray();
        
        // Load pixelData from binary file at next position
        int nextVolumeToBuffer = (currentTimeStep + 1 + binaryBuffer.Count) % filePaths.Length;
        byte[] pixelData = File.ReadAllBytes(filePaths[nextVolumeToBuffer]);
        
        // Store byte array in queue
        binaryBuffer.Enqueue(pixelData);
        
        // Clear the flag to indicate that the reading operation has completed
        isReadingBinary = false;
        Debug.Log("Ended thread!");
    }

    private Thread StartBufferThread(String path) {
        var t = new Thread(() => LoadAndStoreBinary(path));
        t.Start();
        return t;
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
    private String name;
    private bool active;
    private String[] filePaths;
    private Material material;
    private MeshRenderer meshRenderer;
    private int count;
}