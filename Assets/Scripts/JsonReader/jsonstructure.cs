using System.Collections.Generic;
using UnityEngine;

namespace JsonReader
{
    public class Embedding : MonoBehaviour
    {
        public List<NVector> nVectors { get; set; }
        public List<double> eigenvalues { get; set; }
        public List<string> names { get; set; }
        public string field { get; set; }
    }

    public class NVector : MonoBehaviour
    {
        public int key { get; set; }
        public List<List<double>> value { get; set; }
    }

    public class Root : MonoBehaviour
    {
        public string hash { get; set; }
        public List<Embedding> embeddings { get; set; }
    }
}