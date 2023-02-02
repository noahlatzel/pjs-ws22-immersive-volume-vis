using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace JsonReader
{
    internal class Reader : MonoBehaviour
    {
        private static List<Embedding> ReadData()
        {
            const string filePath = @"./Assets/Scripts/JsonReader/plot1.json";

            var jsonContent = File.ReadAllText(filePath);

            var plotData = JsonConvert.DeserializeObject<Root>(jsonContent);

            var embeddings = plotData.embeddings;

            return embeddings;
        }

        /*
         * Returns list of layers (prs, tmp, v01, v02) with every layer containing list of all 7 simulation runs,
         * with each simulation run containing list of timesteps, with each timestep containing a Vector3 (the relevant data)
         */
        public static List<List<List<Vector3>>> GiveDataList()
        {
            var embeddings = ReadData();

            var dataList = new List<List<List<Vector3>>>();

            foreach (var t in embeddings)
            {
                var vecList = new List<List<Vector3>>();
                foreach (var nVectors in t.nVectors)
                {
                    var currVecList = ConvDoubleToVec(nVectors.value);

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
    }
}