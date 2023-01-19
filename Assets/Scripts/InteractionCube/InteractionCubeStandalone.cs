using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class InteractionCubeStandalone : MonoBehaviour
{
    private GameObject leftHand;
    private GameObject rightHand;
    private XRDirectInteractor leftHandComponent;
    private XRDirectInteractor rightHandComponent;
    
    private GameObject lowerScaleCube;
    private GameObject upperScaleCube;
    private Vector3 scalingCubesSize = new Vector3(0.3f, 0.3f, 0.3f);
    private bool scalingCubesCreated = false;
    private float initialDistance;
    private Vector3 initialScaleInteractionCube;
    private GameObject volumeRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        leftHandComponent = leftHand.GetComponent<XRDirectInteractor>();
        rightHandComponent = rightHand.GetComponent<XRDirectInteractor>();
        
        initialDistance = Vector3.Distance(lowerScaleCube.transform.position, upperScaleCube.transform.position);
        initialScaleInteractionCube = transform.localScale;
        volumeRenderer = GameObject.Find("RenderedVolume");
    }

    // Update is called once per frame
    void Update()
    {
        RotateCube();
        ManageScalingCubes();
    }

    void RotateCube()
    {
        Debug.Log("Stopped Rotating Cube!");
        if (!IsHandNearCube())
        {   
            Debug.Log("Rotating Cube!");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, 1.5f);
        }
    }

    bool IsHandNearCube()
    {
        Vector3 curPos = transform.position;
        Vector3 leftHandPos = leftHand.transform.position;
        Vector3 rightHandPos = rightHand.transform.position;
        float distance = 0.5f;
        
        bool leftHandInArea =
            Mathf.Abs(leftHandPos.x - curPos.x) < distance &&
            Mathf.Abs(leftHandPos.z - curPos.z) < distance && leftHandPos.y - curPos.y > distance;
        bool rightHandInArea =
            Mathf.Abs(rightHandPos.x - curPos.x) < distance &&
            Mathf.Abs(rightHandPos.z - curPos.z) < distance && rightHandPos.y - curPos.y > distance;
        
        return leftHandInArea || rightHandInArea;
    }
    
    bool AreHandsNearCube()
    {
        Vector3 curPos = transform.position;
        Vector3 leftHandPos = leftHand.transform.position;
        Vector3 rightHandPos = rightHand.transform.position;
        float distance = 0.5f;
        
        bool leftHandInArea =
            Mathf.Abs(leftHandPos.x - curPos.x) < distance &&
            Mathf.Abs(leftHandPos.z - curPos.z) < distance && leftHandPos.y - curPos.y > distance;
        bool rightHandInArea =
            Mathf.Abs(rightHandPos.x - curPos.x) < distance &&
            Mathf.Abs(rightHandPos.z - curPos.z) < distance && rightHandPos.y - curPos.y > distance;
        
        return leftHandInArea && rightHandInArea;
    }

    void ManageScalingCubes()
    {
        if (AreHandsNearCube())
        {
            Debug.Log("Both hands near cube!");
            CreateScalingCubes();
            ScaleInteractionCube();
        }
        else
        {
            DestroyScalingCubes();
        }
    }

    void CreateScalingCubes()
    {
        if (!scalingCubesCreated)
        {
            lowerScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lowerScaleCube.name = "lowerScaleCube";
            lowerScaleCube.transform.SetParent(transform);
            lowerScaleCube.transform.localScale = scalingCubesSize;
            lowerScaleCube.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f) * 2;
            
            Renderer rendLowerScale = lowerScaleCube.GetComponent<Renderer>();
            rendLowerScale.material = Resources.Load<Material>("Indigo");
            lowerScaleCube.AddComponent<Rigidbody>();
            lowerScaleCube.AddComponent<XRGrabInteractable>();
            lowerScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
            lowerScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
            lowerScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = true;
            lowerScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
            lowerScaleCube.GetComponent<Rigidbody>().useGravity = false;
            
            upperScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            upperScaleCube.name = "upperScaleCube";
            upperScaleCube.transform.SetParent(transform);
            upperScaleCube.transform.localScale = scalingCubesSize;
            upperScaleCube.transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f) * 2;
            
            Renderer rendUpperScale = upperScaleCube.GetComponent<Renderer>();
            rendUpperScale.material = Resources.Load<Material>("Indigo");
            upperScaleCube.AddComponent<Rigidbody>();
            upperScaleCube.AddComponent<XRGrabInteractable>();
            upperScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
            upperScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
            upperScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = true;
            upperScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
            upperScaleCube.GetComponent<Rigidbody>().useGravity = false;

            scalingCubesCreated = true;
        }
    }

    void DestroyScalingCubes()
    {
        if (scalingCubesCreated)
        {
            Destroy(upperScaleCube);
            Destroy(lowerScaleCube);

            scalingCubesCreated = false;
        }
    }

    void ScaleInteractionCube()
    {
        float scaleOfCube = Vector3.Distance(upperScaleCube.transform.localPosition, lowerScaleCube.transform.localPosition) / initialDistance;
        transform.localScale = initialScaleInteractionCube * scaleOfCube;
        
        volumeRenderer.transform.localScale = new Vector3(1, 1, 1) * scaleOfCube;
    }
}
