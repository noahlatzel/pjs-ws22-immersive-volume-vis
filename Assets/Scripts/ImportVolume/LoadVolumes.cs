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
    public String datasetName = "yB11";

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
    [SerializeField] public bool play;
    
    [Tooltip("Specify the frames per second of the animation.")]
    [SerializeField]
    public int timesPerSecond = 1;
    
    [Tooltip("Start/Stop the buffer.")]
    public bool isBuffering;

    public bool useScaledVersion;
    
    [Tooltip("Adjust the buffer speed (buffer per second)")]
    public int bufferSpeed = 5;

    public bool forward = true;
    public int timestep = 0;
    
    // Manager class
    public VolumeManager volumeManager;

    // Load the first volume per attribute with the importer; guarantees
    // correct configuration of Material properties etc.
    // Start is called before the first frame update
    void Start()
    {
        // Create manager class
        volumeManager = new VolumeManager(datasetName);

        // Render volumes on start 
        RenderOnStart(volumeManager);

        Debug.Log(volumeManager.IsReadingBinary());
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
                volumeManager.SetForward(forward);
                if (forward)
                {
                    // Render the next frame through Volume Manager
                    volumeManager.NextFrame();
                    timestep = (timestep + 1) % 10;
                }
                else
                {
                    volumeManager.PreviousFrame();
                    timestep = (timestep - 1) % 10;
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

    public void SetFrame(int timeStep)
    {
        volumeManager.SetFrame(timeStep);
        timestep = timeStep;
    }

    public int getCount()
    {
        int count = 0;
        if (volumeManager.GetVolumeAttributes().Length > 0)
        {
            count = volumeManager.GetVolumeAttributes()[0].GetCount();

            foreach (var volAtt in volumeManager.GetVolumeAttributes())
            {
                int currCount = volAtt.GetCount();
                if (currCount < count)
                {
                    count = currCount;
                }
            }
        }

        return count;
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
        if (number < 0)
        {
            number = count + number;
        }

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

    public int GetCount()
    {
        return count;
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

    public void NextFrame(int numberOfFramesToSkip)
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

            for (int j = 1; j < numberOfFramesToSkip; j++)
            {
                if (bufferQueue.Count > 2)
                {
                    bufferQueue.Dequeue();
                }
            }

            // Set pixel data from bufferQueue
            newTexture.SetPixelData(bufferQueue.Dequeue(), 0);
            
            
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", newTexture);

            // Upload new texture to GPU -> major bottleneck, can not be called async/in coroutine/ in
            // a separate thread
            newTexture.Apply();
        }
    }

    public void SetFrame(int timestep)
    {
        // Get correct texture format analogue to the Volume Importer
        TextureFormat texFormat = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf) ? TextureFormat.RHalf : TextureFormat.RFloat;
        
        // Set the current texture to the next texture in the buffer depended on usingScaled
        Texture3D newTexture = usingScaled ? new Texture3D(100, 100, 100, texFormat, false) : 
            new Texture3D(300, 300, 300, texFormat, false);

        // Load pixelData from binary file at given position
        int volumeToRender = Math.Max(0,Math.Min(timestep,count));

        Debug.Log("SetFrame: Aktueller Timestep intern: " + volumeToRender);
        
        // Set pixel data from File
        newTexture.SetPixelData(File.ReadAllBytes(GetVolumePath(volumeToRender)), 0);
        
        // Set _DataTex texture of material to newly loaded texture
        material.SetTexture("_DataTex", newTexture);

        // Upload new texture to GPU -> major bottleneck, can not be called async/in coroutine/ in
        // a separate thread
        newTexture.Apply();
    }

    public void PreviousFrame()
    {
        // Check if textures are buffered
        /*if (bufferStack.Count() > 0)
        {
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", bufferStack.Pop());
        }
        else
        {
            NextFrame();
        }*/
        NextFrame();
    }
    
    public void PreviousFrame(int numberOfFramesToSkip)
    {
        // Check if textures are buffered
        /*if (bufferStack.Count() > 0)
        {
            // Set _DataTex texture of material to newly loaded texture
            material.SetTexture("_DataTex", bufferStack.Pop());
        }
        else
        {
            NextFrame();
        }*/
        NextFrame(numberOfFramesToSkip);
    }

    public void ClearBufferQueue()
    {
        bufferQueue.Clear();
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
    
    // Loads the binary from the specified path and stores the loaded byte array in a queue.
    // Loading the binary has to be separated from the main function because Unity functions classes
    // may not be called in other threads than the main thread but we want to load our data in a separate 
    // thread to reduce lag.
    public void BufferNextFrameReverse(int currentTimeStep)
    {
        // Restrict buffer size
        if (bufferQueue.Count <= 10)
        {
            // Load pixelData from binary file at next position
            int nextVolumeToBuffer = (currentTimeStep - 1 - bufferQueue.Count) % count;
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
            
            // Clear bufferStack during runtime
            while (bufferStack.Count() > 0)
            {
                bufferStack.Pop();
            }
        }
        usingScaled = usage;
    }
}

public class VolumeManager
{
    private String dataSetPath;
    private VolumeAttribute[] volumeAttributes;
    private int currentTimeStep;
    private bool isReadingBinary;
    private bool usingScaled = false;
    private bool forward = true;

    public VolumeManager(String dataSetName)
    {
        dataSetPath = $"Assets/Datasets/{dataSetName}";
        AddVolumeAttributes();
    }

    public void SetForward(bool direction)
    {
        if (forward != direction)
        {
            foreach (var volumeAttribute in volumeAttributes)
            {
                volumeAttribute.ClearBufferQueue();
            }
        }

        forward = direction;
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

    public void SetFrame(int timeStep)
    {
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.IsVisible())
            {
                volumeAttribute.SetFrame(timeStep);
            }
        }
        
        currentTimeStep = timeStep;
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
    
    public void NextFrame(int framesToSkip)
    {
        bool active = false;
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.IsVisible())
            {
                volumeAttribute.NextFrame(framesToSkip);
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
    
    public void PreviousFrame(int framesToSkip)
    {
        bool active = false;
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.IsVisible())
            {
                volumeAttribute.PreviousFrame(framesToSkip);
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
            if (forward)
            {
                volumeAttribute.BufferNextFrame(currentTimeStep);
            }
            else
            {
                volumeAttribute.BufferNextFrameReverse(currentTimeStep);
            }
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
        foreach (var volumeAttribute in volumeAttributes)
        {
            volumeAttribute.SetUsingScale(usingScaled);
        }
        if (usingScaled != usage)
        {
            RefreshCurrentState();
        }
        usingScaled = usage;
    }

    public void SetDataset(String newDatasetName)
    {
        if (dataSetPath != $"Assets/Datasets/{newDatasetName}")
        {
            dataSetPath = $"Assets/Datasets/{newDatasetName}";
            currentTimeStep = 0;
            RefreshCurrentState();
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

    public void SetPressure(bool visibility)
    {
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.GetName() == "Pressure" || volumeAttribute.GetName() == "prs")
            {
                if (visibility != volumeAttribute.IsVisible())
                {
                    volumeAttribute.SetVisibility(visibility);
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
        }
    }
    
    public void SetTemperature(bool visibility)
    {
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.GetName() == "Temperature" || volumeAttribute.GetName() == "tev")
            {
                if (visibility != volumeAttribute.IsVisible())
                {
                    volumeAttribute.SetVisibility(visibility);
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
        }
    }
    
    public void SetWater(bool visibility)
    {
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.GetName() == "Water" || volumeAttribute.GetName() == "v02")
            {
                if (visibility != volumeAttribute.IsVisible())
                {
                    volumeAttribute.SetVisibility(visibility);
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
        }
    }
    
    public void SetMeteorite(bool visibility)
    {
        foreach (var volumeAttribute in volumeAttributes)
        {
            if (volumeAttribute.GetName() == "Meteorite" || volumeAttribute.GetName() == "v03")
            {
                if (visibility != volumeAttribute.IsVisible()) {
                    volumeAttribute.SetVisibility(visibility);
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
        }
    }
}