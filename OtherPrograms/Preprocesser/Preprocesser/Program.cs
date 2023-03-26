using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Preprocesser {
    public class Program {

        private readonly Queue<Preprocesser.ProcessElementStruct> processingQueue = new();
        int counter = 0;
        int initialCount;
        private readonly Stopwatch stopwatch = new Stopwatch();
        // Get all dataset paths
        string folder = Directory.GetParent(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)) + @"\Datasets\";

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting preprocessing!");
            Program program = new Program();
            string[] dataSetPaths = Directory.GetDirectories(program.folder);
            foreach (String dataSetPath in dataSetPaths)
            {
                // Get all paths to the (four) volume attributes for each dataset
                string[] volumeAttributePaths = Directory.GetDirectories(dataSetPath).Where(path => !path.EndsWith("_bin")).ToArray();

                foreach (String volumeAttributePath in volumeAttributePaths)
                {
                    // Set path to store the binary files in separate directory
                    // @ forces the String to be interpreted verbatim
                    String binaryPath = volumeAttributePath + @"_bin/";
                    if (!Directory.Exists(binaryPath))
                    {
                        Directory.CreateDirectory(binaryPath);
                        Console.WriteLine("Missing directory " + binaryPath + " successfully created.");
                    }

                    // Get all paths to .nii files for the given attribute
                    string[] fileEntries = Directory.GetFiles(volumeAttributePath, "*.nii");

                    foreach (String file in fileEntries)
                    {
                        // Extract fileName from path
                        String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";

                        if (!File.Exists(binaryPath + fileName) /*|| !File.Exists(binaryPath + Path.GetFileNameWithoutExtension(file) + "_9.bin")*/)
                        {
                            ProcessElementStruct processElement = new ProcessElementStruct
                            {
                                binaryPath = binaryPath,
                                fileEntry = file
                            };

                            program.processingQueue.Enqueue(processElement);
                        }
                    }
                }
            }
            program.initialCount = program.processingQueue.Count;
            program.ProcessVolumes();
        }

        void ProcessVolumes()
        {
            object syncLock = new object();

            int maxConcurrentThreads = Environment.ProcessorCount; 
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentThreads);

            List<Task> tasks = new List<Task>();

            foreach (var processElementStruct in processingQueue)
            {
                semaphore.Wait();

                Task task = Task.Run(() =>
                {
                    if (!stopwatch.IsRunning)
                    {
                        stopwatch.Start();
                    }

                    PreprocessVolume(processElementStruct);

                    lock (syncLock)
                    {
                        counter++;
                        TimeSpan timePerVolume = stopwatch.Elapsed / counter;
                        TimeSpan remainingTime = timePerVolume * (processingQueue.Count - counter);
                        DateTime finishedUntil = DateTime.Now.Add(remainingTime);
                        Console.WriteLine($"Processed {counter}/{initialCount} volumes in {stopwatch.Elapsed:hh\\:mm\\:ss} | time per volume {timePerVolume:mm\\:ss\\.fff} | approx. remaining time: {remainingTime:hh\\:mm\\:ss} | finished until {finishedUntil:HH:mm:ss}");
                    }
                    semaphore.Release();
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
        }

        void PreprocessVolumeOld(ProcessElementStruct processElementStruct)
        {
            String file = processElementStruct.fileEntry;
            String binaryPath = processElementStruct.binaryPath;

            // Extract fileName from path
            //String fileName = Path.GetFileNameWithoutExtension(file) + "_ushort.bin"; // + "ushort" for ushort variant
            String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";

            // Only convert to binary if it has not been converted already
            if (!File.Exists(binaryPath + fileName))
            {
                // Convert .nii to dataset and create object from it
                VolumeDataset dataset = VolumeDataset.Import(file);

                // Save pixel data to binary file
                byte[] pixelByteData = dataset.CreateTextureInternalBytes();
                File.WriteAllBytes(binaryPath + fileName, pixelByteData);

                //ushort[] pixelData = dataset.CreateTextureInternal();
                //WriteShorts(pixelData, binaryPath + fileName);
            }
        }

        void PreprocessVolume(ProcessElementStruct processElementStruct)
        {
            String file = processElementStruct.fileEntry;
            String binaryPath = processElementStruct.binaryPath;

            String fileName = Path.GetFileNameWithoutExtension(file) + ".bin";

            if (!File.Exists(binaryPath + fileName))
            {
                VolumeDataset dataset = VolumeDataset.Import(file);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    dataset.WriteTextureInternalBytes(memoryStream);
                    byte[] pixelByteData = memoryStream.ToArray();
                    File.WriteAllBytes(binaryPath + fileName, pixelByteData);
                }
            }
        }


        static void WriteShorts(ushort[] values, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (short value in values)
                    {
                        bw.Write(value);
                    }
                }
            }
        }
        private ushort[] ReadUShortArray(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File not found at the specified path.", path);
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                int numberOfUShorts = (int)(fileStream.Length / sizeof(ushort));
                ushort[] ushortArray = new ushort[numberOfUShorts];

                for (int i = 0; i < numberOfUShorts; i++)
                {
                    ushortArray[i] = binaryReader.ReadUInt16();
                }

                return ushortArray;
            }
        }
    }
}



