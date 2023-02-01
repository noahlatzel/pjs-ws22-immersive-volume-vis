using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace JsonReader
{
    class Reader : MonoBehaviour
    {

        // public List<Embedding> embeddings;
        // public static List<List<Vector3>> allVecList = new List<List<Vector3>>();
        // void Start()
        // {
        //     string filePath = @"./Assets/Scripts/JsonReader/plot1.json";
        //
        //     string jsonContent = File.ReadAllText(filePath);
        //
        //     var plotData = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonContent);
        //
        //     embeddings = plotData.embeddings;
        //     
        //     makePRSList();
        //     
        //     Debug.Log(allVecList.ToArray()[0].ToArray()[0].x);
        //
        //     // Embedding[] embArr = plotData.embeddings.ToArray();
        //     //
        //     // NVector[] nVecArr = embArr[0].nVectors.ToArray();
        //     //
        //     // List<double>[] ValArr = nVecArr[0].value.ToArray();
        //     //
        //     // string testoutput = plotData.embeddings.ToArray()[0].nVectors.ToArray()[0].value.ToArray()[0].ToString();
        //     //
        //     // Debug.Log("Erste Koordinate lautet: " + plotData.embeddings.ToArray()[0].nVectors.ToArray()[0].value.ToArray()[0].ToArray()[0].ToString());
        // }

        public static List<List<Vector3>> giveVals()
        {

            string filePath = @"./Assets/Scripts/JsonReader/plot1.json";

            string jsonContent = File.ReadAllText(filePath);

            var plotData = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonContent);
            
            List<List<Vector3>> vecList = new List<List<Vector3>>();

            List<Embedding> embeds = plotData.embeddings;
            
            foreach (NVector nVec in embeds[0].nVectors)
            {
                List<Vector3> currVecList = nVec.convDoubleToVec();
                   
                vecList.Add(currVecList);
            }

            return vecList;
        }
        
        // public void makePRSList()
        // {
        //     foreach (NVector nVec in embeddings[0].nVectors)
        //     {
        //         List<Vector3> currVecList = nVec.convDoubleToVec();
        //            
        //         allVecList.Add(currVecList);
        //     }
        //         
        // }
        
        
    }
}