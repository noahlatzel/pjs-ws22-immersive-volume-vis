using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsonReader
{
    class Reader
    {
        static void Main(string[] args)
        {
            string filePath = @"G:\Uni\Projektseminar\JsonReader\JsonReader\JsonReader\plot1.json";

            string jsonContent = File.ReadAllText(filePath);

            var plotData = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(jsonContent);

            Embedding[] embArr = plotData.embeddings.ToArray();

            NVector[] nVecArr = embArr[0].nVectors.ToArray();

            List<double>[] ValArr = nVecArr[0].value.ToArray();
            
            

            Console.WriteLine(plotData.embeddings.ToArray()[0].nVectors.ToArray()[0].value.ToArray()[0].ToString());
        }
    }
}