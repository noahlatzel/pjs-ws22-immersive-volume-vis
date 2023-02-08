using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace PlotScripts_Volumetric
{
    internal class Reader : MonoBehaviour
    {
        private static List<Embedding> ReadData()
        {
            const string filePath = @"./Assets/Scripts/PlotScripts Volumetric/plot1.json";

            var jsonContent = File.ReadAllText(filePath);

            var plotData = JsonConvert.DeserializeObject<Root>(jsonContent);

            var embeddings = plotData.embeddings;

            return embeddings;
        }

        /*
         * Returns list of layers (prs, tmp, v01, v02) with every layer containing list of all 7 simulation runs,
         * with each simulation run containing list of timesteps, with each timestep containing a Vector3 (the relevant data)
         */
        public static List<List<List<Vector3>>> GiveDataList(int dimension)
        {
            var embeddings = ReadData();

            var dataList = new List<List<List<Vector3>>>();

            foreach (var t in embeddings)
            {
                var vecList = new List<List<Vector3>>();
                foreach (var nVectors in t.nVectors)
                {
                    var currVecList = PreprocessList(nVectors.value, dimension);
                    
                    // Debug.Log(currVecList[0].x + ", " + currVecList[0].y + ", " + currVecList[0].z);

                    vecList.Add(currVecList);
                }

                dataList.Add(vecList);
            }

            return dataList;
        }

        /*
         * Converts a list of triplets of doubles (as lists with 3 elements each) to a list of Vector3
         */
        public static List<Vector3> ConvDoubleToVec(List<List<double>> value)
        {
            var vectorList = new List<Vector3>();

            foreach (var innerList in value)
                vectorList.Add(new Vector3((float)innerList[0], (float)innerList[1], (float)innerList[2]));

            return vectorList;
        }

        public static List<Vector3> ChangeDimension(List<Vector3> valList, int dimension)
        {
            var outList = new List<Vector3>();
            
             for (int i = 0; i < valList.Count; i++)
            {
                switch (dimension)
                {
                    case 1:
                        // Debug.Log("Case 1");
                        outList.Add(new Vector3(valList[i].x, i/100f, 0));
                        break;
                    case 2:
                        // Debug.Log("Case 2");
                        outList.Add(new Vector3(valList[i].x, valList[i].y, 0));
                        break;
                    case 3:
                        // Debug.Log("Case 3");
                        outList.Add(new Vector3(valList[i].x, valList[i].y, valList[i].z));
                        break;
                    default:
                        Debug.LogError("Reader.ChangeDimension: Invalid dimension");
                        break;
                }
            }

            return outList;
        }

        public static List<Vector3> PreprocessList(List<List<double>> value, int dimension)
        {
            var vectorList = ConvDoubleToVec(value);
            vectorList = ChangeDimension(vectorList, dimension);

            return vectorList;
        }
    }
}