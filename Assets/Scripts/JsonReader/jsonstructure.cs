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

        public List<Vector3> convDoubleToVec()
        {
            List<Vector3> vectorList = new List<Vector3>();

            foreach (List<double> innerList in value)
            {
                // vectorList.Add(new Vector3((float) innerList[0], (float)innerList[1], (float)innerList[2]));
                vectorList.Add(new Vector3((float) innerList[0], (float)innerList[1], 0f));
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
