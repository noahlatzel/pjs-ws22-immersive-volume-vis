using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class appearInFront : MonoBehaviour
{
    private GameObject uiCanvas;
    
    private GameObject mainCam;
    //private GameObject camOffset;
    
    //private GameObject leftHand;
    //private GameObject rightHand;
    //private XRDirectInteractor leftHandComponent;
    //private XRDirectInteractor rightHandComponent;

    public InputActionProperty showMenu;
    
    
    // Start is called before the first frame update
    void Start()
    {
        uiCanvas = GameObject.Find("uiCanvas");
     
        camOffset = GameObject.Find("Camera Offset");
        mainCam = GameObject.Find("Main Camera");
        
        
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        leftHandComponent = leftHand.GetComponent<XRDirectInteractor>();
        rightHandComponent = rightHand.GetComponent<XRDirectInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (showMenu.action.WasPressedThisFrame())
        //{
            //bool leftHandActive = leftHandComponent.isSelectActive;
            //bool rightHandActive = rightHandComponent.isSelectActive;
        
        
            Quaternion newRot = new Quaternion(uiCanvas.transform.rotation.x, mainCam.transform.rotation.y, uiCanvas.transform.rotation.z, mainCam.transform.rotation.w);
            Vector3 newPos = ((newRot * Vector3.forward * 3) + mainCam.transform.position);
            newPos.y = 1f;
        
            //Debug.Log(" w:" + mainCam.transform.rotation.w + " x:" + mainCam.transform.rotation.x + " y:" + mainCam.transform.rotation.y + " z:" + mainCam.transform.rotation.z);

            uiCanvas.transform.position = newPos;
            uiCanvas.transform.rotation = Quaternion.RotateTowards(uiCanvas.transform.rotation, newRot, 5f);
        //}
    }
}
