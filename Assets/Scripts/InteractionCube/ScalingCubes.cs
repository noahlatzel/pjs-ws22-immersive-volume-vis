using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class ScalingCubes : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject lowerScaleCube;
    private GameObject upperScaleCube;
    private GameObject interactionCube;
    private GameObject volumeRenderer;
    private Vector3 scale = new Vector3(0.3f, 0.3f, 0.3f);
    private float initialDistance;

    private MeshRenderer lowerScaleCubeMeshRenderer;
    private MeshRenderer upperScaleCubeMeshRenderer;

    private BoxCollider lowerScaleCollider;
    private BoxCollider upperScaleCollider;

    protected Vector3 scaleConst = new Vector3(0.1f, 0.1f, 0.1f);

    // initalize hands
    private bool leftHandInArea;
    private bool rightHandInArea;
    private GameObject leftHand;
    private GameObject rightHand;
    private XRDirectInteractor leftHandComponent;
    private XRDirectInteractor rightHandComponent;
    private bool handsInArea;

    // interactionCube data
    private Vector3 initialScaleInteractionCube;
    private BoxCollider interactionCubeCollider;

    void Start()
    {

        interactionCube = GameObject.Find("interactionCube");
        initialScaleInteractionCube = interactionCube.transform.localScale;
        interactionCubeCollider = interactionCube.GetComponent<BoxCollider>();

        volumeRenderer = GameObject.Find("VolumeScriptStarter (empty)");

        //create lower cube
        lowerScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowerScaleCube.name = "lowerScaleCube";
        lowerScaleCube.transform.SetParent(interactionCube.transform);
        lowerScaleCube.transform.localScale = scale;
        lowerScaleCube.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f) * 2;
        lowerScaleCube.GetComponent<MeshRenderer>().enabled = false;
        
        Renderer rendLowerScale = lowerScaleCube.GetComponent<Renderer>();
        rendLowerScale.material = Resources.Load<Material>("Indigo");
        lowerScaleCollider = lowerScaleCube.GetComponent<BoxCollider>();
        lowerScaleCollider.enabled = false;
        lowerScaleCube.AddComponent<Rigidbody>();
        lowerScaleCube.AddComponent<XRGrabInteractable>();
        lowerScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
        lowerScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        lowerScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = true;
        lowerScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        lowerScaleCube.GetComponent<Rigidbody>().useGravity = false;
        
        // create upper cube
        upperScaleCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        upperScaleCube.name = "upperScaleCube";
        upperScaleCube.transform.SetParent(interactionCube.transform);
        upperScaleCube.transform.localScale = scale;
        upperScaleCube.transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f) * 2;
        //upperScaleCube.transform.position = new Vector3(interactionCube.transform.position.x + interactionCube.transform.localScale.x, interactionCube.transform.position.y + interactionCube.transform.localScale.y, interactionCube.transform.position.z - interactionCube.transform.localScale.z);
        upperScaleCube.GetComponent<MeshRenderer>().enabled = false;
        
        Renderer rendUpperScale = upperScaleCube.GetComponent<Renderer>();
        rendUpperScale.material = Resources.Load<Material>("Indigo");
        upperScaleCollider = upperScaleCube.GetComponent<BoxCollider>();
        upperScaleCollider.enabled = false;
        upperScaleCube.AddComponent<Rigidbody>();
        upperScaleCube.AddComponent<XRGrabInteractable>();
        upperScaleCube.AddComponent<XRSingleGrabFreeTransformer>();
        upperScaleCube.GetComponent<XRGrabInteractable>().movementType = XRBaseInteractable.MovementType.Instantaneous;
        upperScaleCube.GetComponent<XRGrabInteractable>().retainTransformParent = true;
        upperScaleCube.GetComponent<XRGrabInteractable>().throwOnDetach = false;
        upperScaleCube.GetComponent<Rigidbody>().useGravity = false;
        
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        leftHandComponent = leftHand.GetComponent<XRDirectInteractor>();
        rightHandComponent = rightHand.GetComponent<XRDirectInteractor>();

        lowerScaleCubeMeshRenderer = lowerScaleCube.GetComponent<MeshRenderer>();
        upperScaleCubeMeshRenderer = upperScaleCube.GetComponent<MeshRenderer>();

        initialDistance = Vector3.Distance(lowerScaleCube.transform.position, upperScaleCube.transform.position);
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
        
        handsInArea = leftHandInArea || rightHandInArea;

        if (handsInArea)
        {
            //display scaling cubes
            upperScaleCubeMeshRenderer.enabled = true;
            lowerScaleCubeMeshRenderer.enabled = true;

            upperScaleCube.transform.rotation = Quaternion.RotateTowards(upperScaleCube.transform.rotation, Quaternion.identity, 2.5f);
            lowerScaleCube.transform.rotation = Quaternion.RotateTowards(lowerScaleCube.transform.rotation, Quaternion.identity, 2.5f);

            upperScaleCollider.enabled = true;
            lowerScaleCollider.enabled = true;


            //enable interactionCube
            if (!interactionCubeCollider.enabled)
            {
                interactionCubeCollider.enabled = true;
            }

        }
        else 
        {
            if (!leftHandComponent.isSelectActive && !rightHandComponent.isSelectActive)
            {
                //disable scaling cubes
                upperScaleCubeMeshRenderer.enabled = false;
                lowerScaleCubeMeshRenderer.enabled = false;
                upperScaleCollider.enabled = false;
                lowerScaleCollider.enabled = false;

                float scaleOfCube = Vector3.Distance(upperScaleCube.transform.localPosition, lowerScaleCube.transform.localPosition) / initialDistance;
                Debug.Log(scaleOfCube);
                upperScaleCube.transform.localPosition = new Vector3(0.5f, 0.5f, -0.5f) * 2;
                lowerScaleCube.transform.localPosition = new Vector3(-0.5f, -0.5f, 0.5f) * 2;

                upperScaleCube.transform.rotation = Quaternion.identity;
                lowerScaleCube.transform.rotation = Quaternion.identity;
            }
        }

        // if interaction is grabbed by both hands, set previously grabbed to true
        // Both hands are grabbing -> must grab scaleCubes or rotationCubes
        if (leftHandComponent.isSelectActive &&
            rightHandComponent.isSelectActive && handsInArea)
        {

            interactionCubeCollider.enabled = false;


            // Move cube along axis 

            float scaleOfCube = Vector3.Distance(upperScaleCube.transform.localPosition, lowerScaleCube.transform.localPosition) / initialDistance;

            Debug.Log("scaleOfCube:" + scaleOfCube);
            Debug.Log("initialDistance:" + initialDistance);


            interactionCube.transform.localScale = initialScaleInteractionCube * scaleOfCube;

            volumeRenderer.transform.localScale = new Vector3(1, 1, 1) * scaleOfCube;
        }
        else
        {
            // Only one hand is grabbing -> must grab interactionCube
            //if (leftHandComponent.isSelectActive ^
            //rightHandComponent.isSelectActive) {
            //    upperScaleCubeMeshRenderer.enabled = false;
            //    lowerScaleCubeMeshRenderer.enabled = false;
            //}
        }
    }
}

