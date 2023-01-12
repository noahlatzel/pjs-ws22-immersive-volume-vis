using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
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
    private float timePassedBuffer = 0;
    
    [Tooltip("Start/Stop the animation.")]
    [SerializeField] private bool play;
    
    [Tooltip("Specify the frames per second of the animation.")]
    [SerializeField]
    private int timesPerSecond = 1;
    
    [Tooltip("Start/Stop the buffer.")]
    public bool isBuffering;

    public bool useScaledVersion;
    
    [Tooltip("Adjust the buffer speed (buffer per second)")]
    public int bufferSpeed = 5;

    // Manager class
    private VolumeManager volumeManager;
    
    // command buffer
    private CommandBuffer commandBuffer;
    private bool isPaused;

    // Load the first volume per attribute with the importer; guarantees
    // correct configuration of Material properties etc.
    // Start is called before the first frame update
    void Start()
    {
        // Create manager class
        volumeManager = new VolumeManager(datasetName);
        
        // Render volumes on start 
        RenderOnStart(volumeManager);

        commandBuffer = new CommandBuffer();
        commandBuffer.name = "Load Texture3D";
        
        // execute command buffer
        Camera.main.AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
        
        // Invoke ChangeTexture() method repeatedly every second
        InvokeRepeating("ChangeTexture", 1, 1);
    }

    public void Pause()
    {
        isPaused = true;
        CancelInvoke();
    }

    public void Resume()
    {
        isPaused = false;
        InvokeRepeating("ChangeTexture", 1, 1);
    }

    private void ChangeTexture()
    {
        if (!isPaused)
        {
            Texture3D nextTexture = volumeManager.GetVolumeAttributes()[0].GetNextTexture();
            // update command buffer with new texture
            commandBuffer.SetGlobalTexture("_DataTex", nextTexture);
            volumeManager.GetVolumeAttributes()[0].GetMaterialReference().SetTexture("_DataTex", nextTexture);
        }
    }

    private void OnDestroy()
    {
        Camera.main.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        // Update volume visibility according to public variables
        volumeManager.SetVisibilities(new []{pressure, temperature, water, meteorite});
        volumeManager.SetUsingScale(useScaledVersion);
        
        // Only execute as often as specified in timesPerSecond
        float dur = 1f / timesPerSecond;
        timePassed += Time.deltaTime;
        
        if (timePassed >= dur)
        {
            timePassed -= dur;
            if (play)
            {
                // Render the next frame through Volume Manager
                volumeManager.NextFrame();
            }
        }
        
        // Limit speed of loading the buffer to reduce lag
        float durBuffer = 1f / bufferSpeed;
        timePassedBuffer += Time.deltaTime;
        if (timePassedBuffer >= durBuffer)
        {
            timePassedBuffer -= durBuffer;

            // Check if buffering is checked and Volume Manager is not reading
            if (!volumeManager.IsReadingBinary() && isBuffering)
            {
                // Start the binary reading thread
                var t = new Thread(volumeManager.BufferNextFrame);
                t.Start();
            }
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
    private readonly T[] items;
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
    private readonly String[] filePathsScaled;
    private Material material;
    private MeshRenderer meshRenderer;
    private readonly Queue<byte[]> bufferQueue;
    private readonly int count;
    private readonly String originalPath;
    private bool usingScaled = false;
    private readonly Assembly dll;

    public VolumeAttribute(String path)
    {
        name = new DirectoryInfo(path).Name;
        filePaths = Directory.GetFiles(path + "_bin/", "*.bin")
            .Where(filePath => !filePath.EndsWith("_sub.bin")).ToArray();
        filePathsScaled = Directory.GetFiles(path + "_bin/", "*_sub.bin");
        count = filePaths.Length;
        originalPath = path;
        bufferQueue = new Queue<byte[]>();
        
        // Load the DLL into the Assembly object
        dll = Assembly.LoadFrom("Assets/DLLs/ThreadedBinaryReader.dll");
    }

    private String GetVolumePath(int number)
    {
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
            // Get correct texture format analogue to the Volume Importer
            TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
            // Set the current texture to the next texture in the buffer depended on usingScaled
            Texture3D newTexture = usingScaled ? new Texture3D(100, 100, 100, texFormat, false) : 
                new Texture3D(300, 300, 300, texFormat, false);
        
            // Set pixel data from bufferQueue
            newTexture.SetPixelData(bufferQueue.Dequeue(), 0);
        
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", newTexture);
        
            // Upload new texture to GPU -> major bottleneck, can not be called async/in coroutine/ in
            // a separate thread
            newTexture.Apply();
        }
    }

    public Texture3D GetNextTexture()
    {
        // Get correct texture format analogue to the Volume Importer
        TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
        // Set the current texture to the next texture in the buffer depended on usingScaled
        Texture3D newTexture = usingScaled ? new Texture3D(100, 100, 100, texFormat, false) : 
            new Texture3D(300, 300, 300, texFormat, false);
        
        // Set pixel data from bufferQueue
        newTexture.SetPixelData(bufferQueue.Dequeue(), 0);

        return newTexture;
    }

    public Material GetMaterialReference()
    {
        return material;
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
                .Invoke(null, new object[] { GetVolumePath(nextVolumeToBuffer), bufferQueue });
        }
    }
    
    public void SetUsingScale(bool usage)
    {
        // Check if scale was changed during runtime
        if (usingScaled != usage)
        {
            // Clear bufferQueue during runtime
            bufferQueue.Clear();
        }
        usingScaled = usage;
    }
}

public class VolumeManager
{
    private readonly String dataSetPath;
    private VolumeAttribute[] volumeAttributes;
    private int currentTimeStep;
    private bool isReadingBinary;
    private bool usingScaled = false;

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
                active = true;
            }
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

    public void SetUsingScale(bool usage)
    {
        usingScaled = usage;
        foreach (var volumeAttribute in volumeAttributes)
        {
            volumeAttribute.SetUsingScale(usingScaled);
        }
    }
}