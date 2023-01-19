using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace JsonReader
{
    public class CreatePlot : MonoBehaviour
    {
        LineRenderer lineRenderer;
        
        // Start is called before the first frame update
        void Start()
        {
            
            //obtain embeddings
            Reader reader = GetComponent<Reader>();
            Embedding[] embeddings = reader.embeddings.ToArray();

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = embeddings[0].nVectors.ToArray()[0].value.ToArray().Length;
            
            Debug.Log(lineRenderer.positionCount);
        }

        // Update is called once per frame
        void Update()
        {
            

        }
    }
}