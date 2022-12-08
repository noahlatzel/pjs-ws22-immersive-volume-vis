using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class InteractionCubeFloating : MonoBehaviour
{
    private GameObject interactionCube;
    private Vector3 bobFrom;
    private Vector3 bobTo;
    private Vector3 bobbingDestination;
    protected float moveSpeed = 0.00008f;
    protected Vector3 scaleConst = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 offset = new Vector3(0,.25f, 0);
    private bool leftHandInArea;
    private bool rightHandInArea;
    private GameObject leftHand;
    private GameObject rightHand;
    private XRDirectInteractor leftHandComponent;
    private XRDirectInteractor rightHandComponent;
    protected bool previouslyGrabbed;
    
    // Start is called before the first frame update
    void Start()
    {
        interactionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        interactionCube.transform.SetParent(transform, true);
        interactionCube.name = "interactionCube";
        interactionCube.transform.localScale = scaleConst;
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
        leftHandComponent = leftHand.GetComponent<XRDirectInteractor>();
        rightHandComponent = rightHand.GetComponent<XRDirectInteractor>();
        bobFrom = interactionCube.transform.position;
        bobTo = interactionCube.transform.position + offset;
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
                /*upperScaleCube.GetComponent<MeshRenderer>().enabled = false;
                lowerScaleCube.GetComponent<MeshRenderer>().enabled = false;*/
                
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
                // upperScaleCube.GetComponent<MeshRenderer>().enabled = true;
                // lowerScaleCube.GetComponent<MeshRenderer>().enabled = true;
                
                interactionCube.transform.position =
                    Vector3.MoveTowards(interactionCube.transform.position, bobFrom, moveSpeed*20);
                
                // if interaction is grabbed by either hand, set previously grabbed to true
                if (leftHandComponent.isSelectActive ||
                    rightHandComponent.isSelectActive)
                {
                    
                    // Move cube along axis 
                    // float upperX = upperScaleCube.transform.localPosition.x;
                    // upperScaleCube.transform.localPosition = new Vector3(upperX, upperX, -upperX);
                    //
                    // float lowerX = lowerScaleCube.transform.localPosition.x;
                    // lowerScaleCube.transform.localPosition = new Vector3(lowerX, lowerX, -lowerX);
                    //
                    // float scaleOfCube = Mathf.Abs(upperX) + Mathf.Abs(lowerX) * 0.3f; // 0.3f scale of cube at start
                    //
                    // transform.localScale = scaleConst * scaleOfCube;
                    //
                    // volumeRenderer.transform.localScale *= scaleOfCube;

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
