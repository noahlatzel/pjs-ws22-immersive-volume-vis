using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityVolumeRendering;

public class LoadVolumes : MonoBehaviour
{
    [Tooltip("Specify the name of the dataset you want to use. The dataset needs to be stored in Assets/Datasets/")]
    public String datasetName = "Dataset1";

    public String pressureDirectory = "Pressure";
    public String temperatureDirectory = "Temperature";
    public String waterDirectory = "Water";
    public String meteoriteDirectory = "Meteorite";

    public bool pressure;
    public bool temperature;
    public bool water;
    public bool meteorite;

    private Material[] materials = new Material[4];
    private MeshRenderer[] meshRenderers = new MeshRenderer[4];

    
    // Initialized in Start() with bufferSize
    // TODO Use Stack for future and Queue for past instead
    private int currentTimeStep = 0;
    private float timePassed = 0;
    
    [SerializeField] private bool play;
    
    [SerializeField]
    private int timesPerSecond = 1;
    
    public int bufferStackSize = 10;
    public int bufferQueueSize = 10;
    private LimitedStack<Texture3D> pastStack;
    private LimitedQueue<Texture3D> futureQueue;

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
        futureQueue = new LimitedQueue<Texture3D>(bufferQueueSize);
        pastStack = new LimitedStack<Texture3D>(bufferStackSize);
        
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
            if (play)
            {
                pastStack.Push((Texture3D)materials[0].GetTexture("_DataTex"));
                Texture3D nextTexture = futureQueue.Dequeue();
                materials[0].SetTexture("_DataTex", nextTexture);
                Debug.Log(futureQueue.GetCurrentTexture());
            }
        }
        
        if (isBuffering)
        {
            // Start the file buffering thread
            StartBufferThread("Assets/Datasets/Dataset1/Pressure_bin/");
        }
    }
    
    public Thread StartBufferThread(String path) {
        var t = new Thread(() => LoadFutureBuffer(path));
        t.Start();
        return t;
    }
    
    // Loads binaries in original size and in scaled down size.
    // Usage: - materials[0].SetTexture("_DataTex",LoadBinaryToTexture3D(300, 300, 300,
    //          "Assets/Datasets/Dataset1/Pressure_bin/prs_098.bin"));
    //        - materials[0].SetTexture("_DataTex",LoadBinaryToTexture3D(100, 100, 100,
    //          "Assets/Datasets/Dataset1/Pressure_bin/prs_098.bin"));
    // TODO Slight offset when rendering the volume, parts from the bottom are rendered at the top, needs fix
    Texture3D LoadBinaryToTexture3D(int width, int height, int depth, String path)
    {
        // Copied from Importer
        TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
        // Create Texture3D object
        Texture3D texture = new Texture3D(width, height, depth, texFormat, false);
        
        // Load pixelData from binary file
        byte[] pixelData = File.ReadAllBytes(path);
        
        // Set pixel data to loaded binary data with
        // mipLevel 0 according to implementation in IImageFileImporter.Import
        texture.SetPixelData(pixelData, 0);
        
        // Apply texture to load in GPU
        texture.Apply();
        
        return texture;
    }

    
    void LoadFutureBuffer(String path, bool scaled = false)
    {
        // Only buffer next texture if buffer not full
        if (!futureQueue.CheckBufferFull() || true)
        {
            // Set the flag to indicate that the buffering operation is in progress
            isBuffering = true;
            
            // Calculate path for binary TODO Later add scaled version
            String calcPath = path + futureQueue.GetCurrentTexture() + ".bin";
            
            // Quick fix TODO delete later
            calcPath = Directory.GetFiles(path, "*.bin").Where(path => !path.EndsWith("sub.bin")).ToArray()[futureQueue.GetCurrentTexture()];
            Debug.Log(calcPath);
                // Load bufferedTexture from binary TODO Later add scaled version
            Texture3D bufferedTexture = LoadBinaryToTexture3D(300, 300, 300, calcPath);
        
            // Increment current texture count before enqueueing so other threads can load the next texture
            futureQueue.IncrementCurrentTexture();
            futureQueue.IncrementAlreadyBuffered();
        
            // Enqueue bufferedTexture TODO Check order of textures (threading)
            futureQueue.Enqueue(bufferedTexture);
            
            // Clear the flag to indicate that the buffering operation has completed
            isBuffering = false;

            Debug.Log("Buffer loaded!");
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

public class LimitedQueue<T>
{
    private T[] items;
    private int first = 0;
    private int last = 0;
    private int currentTexture = 0;
    private int alreadyBuffered = 0;

    public LimitedQueue(int capacity)
    {
        items = new T[capacity];
    }

    public void Enqueue(T item)
    {
        items[last] = item;
        last = (last + 1) % items.Length;
        //currentTexture++; TODO review later
    }

    public T Dequeue()
    {
        int temp = first;
        first = (first + 1) % items.Length;
        alreadyBuffered--;
        return items[temp];
    }

    public int GetCurrentTexture()
    {
        return currentTexture;
    }
    
    public void IncrementCurrentTexture()
    {
        currentTexture = (currentTexture + 1) % 14;
    }

    public void IncrementAlreadyBuffered()
    {
        alreadyBuffered++;
    }

    public bool CheckBufferFull()
    {
        return alreadyBuffered >= items.Length;
    }
}