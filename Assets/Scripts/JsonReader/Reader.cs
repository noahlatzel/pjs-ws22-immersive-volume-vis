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

        public List<Embedding> embeddings;
        void Start()
        {
            string filePath = @"./Assets/Scripts/JsonReader/plot1.json";

            string jsonContent = File.ReadAllText(filePath);

            var plotData = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonContent);

            embeddings = plotData.embeddings;

            // Embedding[] embArr = plotData.embeddings.ToArray();
            //
            // NVector[] nVecArr = embArr[0].nVectors.ToArray();
            //
            // List<double>[] ValArr = nVecArr[0].value.ToArray();
            //
            // string testoutput = plotData.embeddings.ToArray()[0].nVectors.ToArray()[0].value.ToArray()[0].ToString();
            //
            // Debug.Log("Erste Koordinate lautet: " + plotData.embeddings.ToArray()[0].nVectors.ToArray()[0].value.ToArray()[0].ToArray()[0].ToString());
        }
    }
}

