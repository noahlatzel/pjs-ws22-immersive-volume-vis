using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class InteractionCubeFloating : MonoBehaviour
{
    private GameObject interactionCube;
    private Vector3 bobFrom;
    private Vector3 bobTo;
    private Vector3 bobbingDestination;
    float moveSpeed = 0.00008f;
    private Vector3 offset = new Vector3(0,.25f, 0);
    private bool leftHandInArea;
    private bool rightHandInArea;
    private GameObject leftHand;
    private GameObject rightHand;
    
    // Start is called before the first frame update
    void Start()
    {
        interactionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);


        interactionCube.transform.SetParent(transform, true);
        interactionCube.name = "interactionCube";
        interactionCube.transform.localScale = new Vector3(0.1f, 0.1f * 4 / 3, 0.1f);
        interactionCube.transform.localPosition = new Vector3(0, transform.localPosition.y + transform.localScale.y / 2 + interactionCube.transform.localScale.y / 2, 0f);
        interactionCube.AddComponent<Rigidbody>();
        interactionCube.AddComponent<XRGrabInteractable>();
        interactionCube.AddComponent<XRSingleGrabFreeTransformer>();
        interactionCube.GetComponent<XRGrabInteractable>().movementType =
            XRBaseInteractable.MovementType.Instantaneous;
        interactionCube.GetComponent<XRGrabInteractable>().retainTransformParent = false;
        interactionCube.GetComponent<Rigidbody>().useGravity = false;

        Renderer rendCube = interactionCube.GetComponent<Renderer>();
        rendCube.material = Resources.Load<Material>("Red");
        
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
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
        bool previouslyGrabbed = false;
        
        if (!handsInArea)
        {
            
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
        else
        {
            if (leftHand.GetComponent<XRDirectInteractor>().isSelectActive &&
                rightHand.GetComponent<XRDirectInteractor>().isSelectActive)
            {
                previouslyGrabbed = true;
            }

            if (previouslyGrabbed)
            {
                interactionCube.transform.position =
                    Vector3.MoveTowards(interactionCube.transform.position, bobFrom, moveSpeed * 3);
                if (interactionCube.transform.position == bobFrom)
                {
                    previouslyGrabbed = false;
                }
            }
            if (interactionCube.transform.rotation != Quaternion.identity)
            {
                interactionCube.transform.rotation =
                    Quaternion.RotateTowards(interactionCube.transform.rotation, Quaternion.identity, 1.5f);
            }
        }
       
    }
}
