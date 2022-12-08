using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ScalingCubes : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject lowerScaleCube;
    private GameObject upperScaleCube;
    private GameObject interactionCube;
    private Vector3 scale = new Vector3(0.3f, 0.3f, 0.3f);
    
    void Start()
    {

        interactionCube = GameObject.Find("interactionCube");
        //create lower cube
        lowerScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerScaleCube.transform.SetParent(interactionCube.transform);
        lowerScaleCube.transform.localScale = scale;
        lowerScaleCube.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f);
        lowerScaleCube.GetComponent<MeshRenderer>().enabled = false;
        
        Renderer rendLowerScale = lowerScaleCube.GetComponent<Renderer>();
        rendLowerScale.material = Resources.Load<Material>("Indigo");
        lowerScaleCube.GetComponent<BoxCollider>().enabled = false;
        lowerScaleCube.AddComponent<Rigidbody>();
        lowerScaleCube.AddComponent<XRGrabInteractable>();
        lowerScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
        lowerScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        lowerScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = false;
        lowerScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        lowerScaleCube.GetComponent<Rigidbody>().useGravity = false;
        
        // create upper cube
        upperScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperScaleCube.transform.SetParent(interactionCube.transform);
        upperScaleCube.transform.localScale = scale;
        upperScaleCube.transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f);
        upperScaleCube.GetComponent<MeshRenderer>().enabled = false;
        
        Renderer rendUpperScale = upperScaleCube.GetComponent<Renderer>();
        rendUpperScale.material = Resources.Load<Material>("Indigo");
        upperScaleCube.GetComponent<BoxCollider>().enabled = false;
        upperScaleCube.AddComponent<Rigidbody>();
        upperScaleCube.AddComponent<XRGrabInteractable>();
        upperScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
        upperScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        upperScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = false;
        upperScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        upperScaleCube.GetComponent<Rigidbody>().useGravity = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

