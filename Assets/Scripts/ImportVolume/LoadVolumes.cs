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

    public bool forward = true;
    
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
        volumeManager.SetUsingScale(useScaledVersion);
        
        // Only execute as often as specified in timesPerSecond
        float dur = 1f / timesPerSecond;
        timePassed += Time.deltaTime;
        
        if (timePassed >= dur)
        {
            timePassed -= dur;
            if (play)
            {
                if (forward)
                {
                    // Render the next frame through Volume Manager
                    volumeManager.NextFrame();
                }
                else
                {
                    volumeManager.PreviousFrame();
                }
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
            obj.transform.localPosition = Vector3.zero;
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

    public int Count() {
        return items.Length;
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
    private readonly LimitedStack<Texture3D> bufferStack;
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
        bufferStack = new LimitedStack<Texture3D>(10);

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

            // Save current Texture3D to bufferStack (for PreviousFrame())
            bufferStack.Push((Texture3D) material.GetTexture("_DataTex"));

            // Set pixel data from bufferQueue
            newTexture.SetPixelData(bufferQueue.Dequeue(), 0);
            
            // WIP
            /*var allBytes = newTexture.GetPixelData<Color32>(0);
            int surroundWidth = 0;
            int textureSplits = 10;
            //int chunkSize = allBytes.Length / textureSplits;
            //var result = allBytes
            //    .Select((x, i) => new { Index = i, Value = x })
            //    .GroupBy(x => x.Index / chunkSize)
            //    .Select(x => x.Select(v => v.Value).ToArray())
            //    .ToArray();
            //for (int i = 0; i < result.Length; i++) {
            //    Texture3D splitTexture = new Texture3D(newTexture.width, newTexture.height, newTexture.depth / textureSplits, texFormat, false);
            //    splitTexture.SetPixelData(result[i], 0);
            //    material.SetTexture("_DataTex" + i, splitTexture);
            //    splitTexture.Apply();
            //}
            
            // Shader: dataTexPos.z = 1/12 + 10 * dataTexPos.z / 12;
            for (int i = 0; i < textureSplits; i++)
            {
                int surroundWidthFactor = i == 0 || i == 9 ? 1 : 2;
                Texture3D splitTexture = new Texture3D(newTexture.width, newTexture.height, newTexture.depth / textureSplits + surroundWidthFactor * surroundWidth, texFormat, false);
                int preOffset = i == 0 ? 0 : newTexture.height * newTexture.width * surroundWidth;
                int pastOffset = i == 9 ? 0 : newTexture.height * newTexture.width * surroundWidth;
                Color32[] pixelData = allBytes.Skip(allBytes.Length / textureSplits * i - preOffset).Take(allBytes.Length / textureSplits + pastOffset + preOffset).ToArray();
                splitTexture.SetPixelData(pixelData, 0);
                material.SetTexture("_DataTex" + i, splitTexture);
                splitTexture.Apply();
            }*/
            
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", newTexture);

            // Upload new texture to GPU -> major bottleneck, can not be called async/in coroutine/ in
            // a separate thread
            newTexture.Apply();
        }
    }

    public void PreviousFrame()
    {
        // Check if textures are buffered
        if (bufferStack.Count() > 0)
        {
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", bufferStack.Pop());
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

    public void PreviousFrame()
    {
        bool active = false;
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.IsVisible())
            {
                volumeAttribute.PreviousFrame();
                active = true;
            }
        }

        if (active)
        {
            currentTimeStep--;
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