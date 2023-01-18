using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JsonReader
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Embedding
    {
        public List<NVector> nVectors { get; set; }
        public List<double> eigenvalues { get; set; }
        public List<string> names { get; set; }
        public string field { get; set; }
    }

    public class NVector
    {
        public int key { get; set; }
        public List<List<double>> value { get; set; }
    }

    public class Root
    {
        public string hash { get; set; }
        public List<Embedding> embeddings { get; set; }
    }


}