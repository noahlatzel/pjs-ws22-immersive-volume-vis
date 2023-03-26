using Nifti.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Preprocesser
{
    public class VolumeDataset
    {
        public string filePath;

        // Flattened 3D array of data sample values.
        public float[] data;

        public int dimX, dimY, dimZ;

        public float scaleX = 1.0f;
        public float scaleY = 1.0f;
        public float scaleZ = 1.0f;

        public float volumeScale;

        public string datasetName;

        private float minDataValue = float.MaxValue;
        private float maxDataValue = float.MinValue;

        public static VolumeDataset Import(string filePath)
        {
            Nifti.NET.Nifti niftiFile = NiftiFile.Read(filePath);
            if (niftiFile == null)
            {
                return null;
            }
            int numDimensions = niftiFile.Header.dim[0];
            if (numDimensions > 3)
            {
                return null;
            }
            int dimX = niftiFile.Header.dim[1];
            int dimY = niftiFile.Header.dim[2];
            int dimZ = niftiFile.Header.dim[3];
            float[] pixelData = niftiFile.ToSingleArray();

            Vector3 pixdim = new Vector3(niftiFile.Header.pixdim[1], niftiFile.Header.pixdim[2], niftiFile.Header.pixdim[3]);
            Vector3 size = new Vector3(dimX * pixdim.X, dimY * pixdim.Y, dimZ * pixdim.Z);

            // Create dataset
            VolumeDataset volumeDataset = new VolumeDataset();
            volumeDataset.data = pixelData;
            volumeDataset.dimX = dimX;
            volumeDataset.dimY = dimY;
            volumeDataset.dimZ = dimZ;
            volumeDataset.datasetName = "test";
            volumeDataset.filePath = filePath;
            volumeDataset.scaleX = size.X;
            volumeDataset.scaleY = size.Y;
            volumeDataset.scaleZ = size.Z;

            volumeDataset.FixDimensions();

            return volumeDataset;
        }

        public float GetMinDataValue()
        {
            if (minDataValue == float.MaxValue)
                CalculateValueBounds();
            return minDataValue;
        }

        public float GetMaxDataValue()
        {
            if (maxDataValue == float.MinValue)
                CalculateValueBounds();
            return maxDataValue;
        }

        /// <summary>
        /// Ensures that the dataset is not too large.
        /// </summary>
        public void FixDimensions()
        {
            int MAX_DIM = 2048; // 3D texture max size. See: https://docs.unity3d.com/Manual/class-Texture3D.html

            while (Math.Max(dimX, Math.Max(dimY, dimZ)) > MAX_DIM)
            {
                DownScaleData();
            }
        }

        /// <summary>
        /// Downscales the data by averaging 8 voxels per each new voxel,
        /// and replaces downscaled data with the original data
        /// </summary>
        public void DownScaleData()
        {
            int halfDimX = dimX / 2 + dimX % 2;
            int halfDimY = dimY / 2 + dimY % 2;
            int halfDimZ = dimZ / 2 + dimZ % 2;
            float[] downScaledData = new float[halfDimX * halfDimY * halfDimZ];

            for (int x = 0; x < halfDimX; x++)
            {
                for (int y = 0; y < halfDimY; y++)
                {
                    for (int z = 0; z < halfDimZ; z++)
                    {
                        downScaledData[x + y * halfDimX + z * (halfDimX * halfDimY)] = (float)Math.Round(GetAvgerageVoxelValues(x * 2, y * 2, z * 2));
                    }
                }
            }

            //Update data & data dimensions
            data = downScaledData;
            dimX = halfDimX;
            dimY = halfDimY;
            dimZ = halfDimZ;
        }

        private void CalculateValueBounds()
        {
            minDataValue = float.MaxValue;
            maxDataValue = float.MinValue;

            if (data != null)
            {
                for (int i = 0; i < dimX * dimY * dimZ; i++)
                {
                    float val = data[i];
                    minDataValue = Math.Min(minDataValue, val);
                    maxDataValue = Math.Max(maxDataValue, val);
                }
            }
        }

        public ushort[] CreateTextureInternal()
        {

            float minValue = GetMinDataValue();
            float maxValue = GetMaxDataValue();
            float maxRange = maxValue - minValue;

            bool isHalfFloat = true;
            try
            {
                if (isHalfFloat)
                {
                    ushort[] pixelBytes = new ushort[data.Length];
                    for (int iData = 0; iData < data.Length; iData++)
                    {
                        pixelBytes[iData] = FloatToHalf((data[iData] - minValue) / maxRange);
                    }
                    /*for (int iData = 0; iData < data.Length; iData++)
                    {
                        if (FloatToHalf((float)(data[iData] - minValue) / maxRange) != 0) {
                            Console.WriteLine($"{iData}: {(float)(data[iData] - minValue) / maxRange} post: {FloatToHalf((float)(data[iData] - minValue) / maxRange, true)}");
                        }
                    }*/

                    return pixelBytes;
                }
                else
                {
                    ushort[] pixelBytes = new ushort[data.Length];
                    for (int iData = 0; iData < data.Length; iData++)
                        pixelBytes[iData] = (ushort)((float)(data[iData] - minValue) / maxRange);
                    return pixelBytes;
                }
            }
            catch (OutOfMemoryException)
            {
                ushort[] pixelBytes = new ushort[data.Length];
                return pixelBytes;
            }
        }

        public byte[] CreateTextureInternalBytes()
        {

            float minValue = GetMinDataValue();
            float maxValue = GetMaxDataValue();
            float maxRange = maxValue - minValue;

            bool isHalfFloat = true;
            try
            {
                if (isHalfFloat)
                {
                    int byteCount = data.Length * 2;
                    byte[] byteArray = new byte[byteCount];
                    for (int iData = 0; iData < data.Length; iData++) {
                        byteArray[iData * 2] = (byte)(FloatToHalf((data[iData] - minValue) / maxRange) & 0xFF);
                        byteArray[iData * 2 + 1] = (byte)((FloatToHalf((data[iData] - minValue) / maxRange) >> 8) & 0xFF);
                    }
                    return byteArray;
                }
                else
                {
                    
                    int byteCount = data.Length * 2;
                    byte[] byteArray = new byte[byteCount];
                    /*for (int iData = 0; iData < data.Length; iData++) {
                        byteArray[iData * 2] = ((data[iData] - minValue) / maxRange);
                        byteArray[iData * 2 + 1] = ( (byte)((data[iData] - minValue) / maxRange) >> 8);
                    }
                    TODO: Fix function but not necessary at the moment, fix similar to isHalfFloat variant
                    */ 
                    return byteArray;
                }
            }
            catch (OutOfMemoryException)
            {
                int byteCount = data.Length * 2;
                byte[] byteArray = new byte[byteCount];
                return byteArray;
            }
        }

        public void WriteTextureInternalBytes(Stream outputStream)
        {
            float minValue = GetMinDataValue();
            float maxValue = GetMaxDataValue();
            float maxRange = maxValue - minValue;

            bool isHalfFloat = true;
            try
            {
                if (isHalfFloat)
                {
                    using (BinaryWriter writer = new BinaryWriter(outputStream))
                    {
                        for (int iData = 0; iData < data.Length; iData++)
                        {
                            ushort halfFloatValue = FloatToHalf((data[iData] - minValue) / maxRange);
                            writer.Write(halfFloatValue);
                        }
                    }
                }
                else
                {
                    using (BinaryWriter writer = new BinaryWriter(outputStream))
                    {
                        for (int iData = 0; iData < data.Length; iData++)
                        {
                            ushort ushortValue = (ushort)(((data[iData] - minValue) / maxRange) * ushort.MaxValue);
                            writer.Write(ushortValue);
                        }
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                using (BinaryWriter writer = new BinaryWriter(outputStream))
                {
                    for (int iData = 0; iData < data.Length; iData++)
                    {
                        ushort ushortValue = (ushort)(((data[iData] - minValue) / maxRange) * ushort.MaxValue);
                        writer.Write(ushortValue);
                    }
                }
            }
        }

        public float GetAvgerageVoxelValues(int x, int y, int z)
        {
            // if a dimension length is not an even number
            bool xC = x + 1 == dimX;
            bool yC = y + 1 == dimY;
            bool zC = z + 1 == dimZ;

            //if expression can only be true on the edges of the texture
            if (xC || yC || zC)
            {
                if (!xC && yC && zC) return (GetData(x, y, z) + GetData(x + 1, y, z)) / 2.0f;
                else if (xC && !yC && zC) return (GetData(x, y, z) + GetData(x, y + 1, z)) / 2.0f;
                else if (xC && yC && !zC) return (GetData(x, y, z) + GetData(x, y, z + 1)) / 2.0f;
                else if (!xC && !yC && zC) return (GetData(x, y, z) + GetData(x + 1, y, z) + GetData(x, y + 1, z) + GetData(x + 1, y + 1, z)) / 4.0f;
                else if (!xC && yC && !zC) return (GetData(x, y, z) + GetData(x + 1, y, z) + GetData(x, y, z + 1) + GetData(x + 1, y, z + 1)) / 4.0f;
                else if (xC && !yC && !zC) return (GetData(x, y, z) + GetData(x, y + 1, z) + GetData(x, y, z + 1) + GetData(x, y + 1, z + 1)) / 4.0f;
                else return GetData(x, y, z); // if xC && yC && zC
            }
            return (GetData(x, y, z) + GetData(x + 1, y, z) + GetData(x, y + 1, z) + GetData(x + 1, y + 1, z)
                    + GetData(x, y, z + 1) + GetData(x, y + 1, z + 1) + GetData(x + 1, y, z + 1) + GetData(x + 1, y + 1, z + 1)) / 8.0f;
        }

        public float GetData(int x, int y, int z)
        {
            return data[x + y * dimX + z * (dimX * dimY)];
        }

        public static ushort FloatToHalf(float value, bool debug = false)
        {
            uint floatBits = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            uint sign = (floatBits >> 16) & 0x8000;
            int exponent = ((int)(floatBits >> 23) & 0xFF) - 127;
            int mantissa = (int)floatBits & 0x007FFFFF;

            // Handle special cases
            if (exponent == 128)
            {
                if (mantissa != 0)
                {
                    // NaN
                    return (ushort)(sign | 0x7FFF);
                }
                else
                {
                    // Infinity
                    return (ushort)(sign | 0x7C00);
                }
            }
            else if (exponent == -127)
            {
                if (mantissa == 0)
                {
                    // Zero (preserve negative zero)
                    return (ushort)sign;
                }
                else
                {
                    // Denormalized number, handle it directly
                    mantissa >>= (126 - 15); // Right shift mantissa to get denormalized half precision
                    return (ushort)(sign | mantissa);
                }
            }

            if (exponent + 15 >= 31)
            {
                // Overflow, return maximum representable number
                return (ushort)(sign | 0x7BFF);
            }
            else if (exponent + 15 <= 0)
            {
                if (debug)
                {
                    //PrintFloatBinaryRepresentation(value);
                    Console.WriteLine("HERE");
                }
                // Underflow or denormalized
                if (exponent + 15 + 10 <= 0)
                {
                    // Too small, flush to zero
                    return (ushort)sign;
                }

                // Denormalized half-precision value
                mantissa |= 0x00800000;
                int shift = 1 - exponent;
                int round = (1 << (shift - 1));
                mantissa += round;
                ushort halfMantissa = (ushort)(mantissa >> shift);
                halfMantissa <<= 2;
                return (ushort)(sign | halfMantissa);
            }
            else
            {
                // Normalized half-precision value
                ushort halfExponent = (ushort)((exponent + 15) << 10);
                ushort halfMantissa = (ushort)(mantissa >> 13);
                return (ushort)(sign | halfExponent | halfMantissa);
            }
        }
        static void PrintFloatBinaryRepresentation(float value)
        {
            // Create a byte array and copy the bits of the float value into it
            byte[] byteArray = BitConverter.GetBytes(value);

            // Iterate over the bytes in the array, printing their binary representation
            foreach (byte b in byteArray)
            {
                for (int i = 7; i >= 0; i--)
                {
                    Console.Write((b >> i) & 1);
                }
            }
        }

        static void PrintUShortBinaryRepresentation(ushort value)
        {
            for (int i = 15; i >= 0; i--)
            {
                Console.Write((value >> i) & 1);
            }
        }

    }
}
