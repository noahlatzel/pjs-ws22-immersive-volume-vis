using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBlink : MonoBehaviour
{
    [Tooltip("Assign sphere.")] public GameObject sphere;

    private Material sphMat;

    private float currAlpha = 0.5f;

    private bool up = true;
    
    // Start is called before the first frame update
    void Start()
    {
        sphMat = sphere.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (currAlpha is > 0.1f and < 1f)
        {
            if(up)
            {
                currAlpha =+ 0.001f;
            }
            else
            {
                currAlpha =- 0.001f; 
            }
        }
        else
        {
            up = !up;
        }
        sphMat.SetColor("_BaseColor", new Color(0f, 1f, 0f, currAlpha));
    }
}
