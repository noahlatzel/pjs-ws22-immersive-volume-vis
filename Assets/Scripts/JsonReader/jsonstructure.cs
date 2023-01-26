using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Unity.VisualScripting;
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

        public List<Vector2> convDoubleToVec()
        {
            List<Vector2> vectorList = new List<Vector2>();

            foreach (List<double> innerList in value)
            {
                vectorList.Add(new Vector2((float) innerList[0], (float)innerList[1]));
            }
            
            
            return vectorList;
        }
    }

    public class Root : MonoBehaviour
    {
        public string hash { get; set; }
        public List<Embedding> embeddings { get; set; }
    }
    
}
