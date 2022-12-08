using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class InteractionCubeFloating : MonoBehaviour
{
    private GameObject interactionCube;
    private GameObject upperScaleCube;
    private GameObject lowerScaleCube;
    private Vector3 bobFrom;
    private Vector3 bobTo;
    private Vector3 bobbingDestination;
    float moveSpeed = 0.00008f;
    private Vector3 offset = new Vector3(0,.25f, 0);
    private bool leftHandInArea;
    private bool rightHandInArea;
    private GameObject leftHand;
    private GameObject rightHand;
    private GameObject volumeRenderer;
    bool previouslyGrabbed;
    
    // Start is called before the first frame update
    void Start()
    {
        interactionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        interactionCube.transform.SetParent(transform, true);
        interactionCube.name = "interactionCube";
        interactionCube.transform.localScale = new Vector3(1, 1 , 1);
        interactionCube.transform.localPosition = new Vector3(0, transform.localPosition.y + transform.localScale.y / 2 + interactionCube.transform.localScale.y / 2, 0f);
        interactionCube.GetComponent<BoxCollider>().enabled = true;
        interactionCube.AddComponent<Rigidbody>();
        interactionCube.AddComponent<XRGrabInteractable>();
        interactionCube.AddComponent<XRSingleGrabFreeTransformer>();
        interactionCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        interactionCube.GetComponent<XRGrabInteractable>().retainTransformParent = false;
        interactionCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        interactionCube.GetComponent<Rigidbody>().useGravity = false;
        
        Destroy(GetComponent<Rigidbody>());

        Renderer rendCube = interactionCube.GetComponent<Renderer>();
        rendCube.material = Resources.Load<Material>("Red");
        
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        bobFrom = interactionCube.transform.position;
        bobTo = interactionCube.transform.position + offset;
        
        const float scaleConst = 0.7f;
        //create lower cube
        lowerScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerScaleCube.transform.SetParent(interactionCube.transform);
        lowerScaleCube.transform.localScale = new Vector3(scaleConst, scaleConst, scaleConst);
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
        upperScaleCube.transform.localScale = new Vector3(scaleConst, scaleConst, scaleConst);
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
        
        volumeRenderer = GameObject.Find("VolumeScriptStarter (empty)");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftHandPos = leftHand.transform.position;
        Vector3 rightHandPos = rightHand.transform.position;
        Vector3 curPos = transform.position;
        Vector3 curScale = transform.localScale;
        
        leftHandInArea =
            Mathf.Abs(leftHandPos.x - curPos.x) < curScale.x / 2 &&
            Mathf.Abs(leftHandPos.z - curPos.z) < curScale.z / 2 && leftHandPos.y - curPos.y > curScale.y / 2;
        rightHandInArea =
            Mathf.Abs(rightHandPos.x - curPos.x) < curScale.x / 2 &&
            Mathf.Abs(rightHandPos.z - curPos.z) < curScale.z / 2 && rightHandPos.y - curPos.y > curScale.y / 2;
        
        bool handsInArea = leftHandInArea || rightHandInArea;
        
        //reset cube position
        if (previouslyGrabbed)
        {
            interactionCube.transform.position =
                Vector3.MoveTowards(interactionCube.transform.position, bobFrom, moveSpeed * 30000 * Time.deltaTime);
            if (interactionCube.transform.position == bobFrom)
            {
                if (interactionCube.transform.rotation != Quaternion.identity)
                {
                    interactionCube.transform.rotation =
                        Quaternion.RotateTowards(interactionCube.transform.rotation, Quaternion.identity, 1.5f);
                }
                else
                {
                    previouslyGrabbed = false;
                }
            }
        }
        else
        {
            if (!handsInArea)
            {
                upperScaleCube.GetComponent<MeshRenderer>().enabled = false;
                lowerScaleCube.GetComponent<MeshRenderer>().enabled = false;
                
                if (interactionCube.transform.position == bobFrom)
                {
                    bobbingDestination = bobTo;
                }
                if (interactionCube.transform.position == bobTo)
                {
                    bobbingDestination = bobFrom;
                }
        
                interactionCube.transform.Rotate(new Vector3(0.2f, 0, 0.2f), Space.World);
                interactionCube.transform.position = Vector3.MoveTowards(interactionCube.transform.position, bobbingDestination, moveSpeed);
            }
            else // hands are now within the 'table-area'
            {
                // display scaling cubes
                upperScaleCube.GetComponent<MeshRenderer>().enabled = true;
                lowerScaleCube.GetComponent<MeshRenderer>().enabled = true;
                
                interactionCube.transform.position =
                    Vector3.MoveTowards(interactionCube.transform.position, bobFrom, moveSpeed*20);
                
                // if interaction is grabbed by either hand, set previously grabbed to true
                if (leftHand.GetComponent<XRDirectInteractor>().isSelectActive ||
                    rightHand.GetComponent<XRDirectInteractor>().isSelectActive)
                {
                    /*
                    // Move cube along axis 
                    float upperX = upperScaleCube.transform.localPosition.x;
                    upperScaleCube.transform.localPosition = new Vector3(upperX, upperX, -upperX);
                    
                    float lowerX = lowerScaleCube.transform.localPosition.x;
                    lowerScaleCube.transform.localPosition = new Vector3(lowerX, lowerX, -lowerX);

                    float scaleOfCube = Mathf.Abs(upperX) + Mathf.Abs(lowerX) * 0.3f; // 0.3f scale of cube at start

                    transform.localScale = new Vector3(scaleOfCube, scaleOfCube, scaleOfCube);

                    volumeRenderer.transform.localScale = new Vector3(scaleOfCube, scaleOfCube, scaleOfCube);*/

                    previouslyGrabbed = true;
                }
                else
                {
                    
                }
            
                if (interactionCube.transform.rotation != Quaternion.identity)
                {
                    interactionCube.transform.rotation =
                        Quaternion.RotateTowards(interactionCube.transform.rotation, Quaternion.identity, 2.5f);
                }
            }
        }
    }
}
