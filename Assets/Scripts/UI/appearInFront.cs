using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class appearInFront : MonoBehaviour
{
    private GameObject mainCam;

    public InputActionProperty showMenu;
    Quaternion newRot;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");

        newRot = mainCam.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {       
        if (showMenu.action.WasPressedThisFrame())
        {
            Quaternion posRot = new Quaternion(gameObject.transform.rotation.x, mainCam.transform.rotation.y, gameObject.transform.rotation.z, mainCam.transform.rotation.w);
            Vector3 newPos = ((posRot * Vector3.forward * 2) + mainCam.transform.position);
            newPos.y = mainCam.transform.position.y - 0.3f;
        
            //Debug.Log(" w:" + mainCam.transform.rotation.w + " x:" + mainCam.transform.rotation.x + " y:" + mainCam.transform.rotation.y + " z:" + mainCam.transform.rotation.z);

            gameObject.transform.position = newPos;
            newRot = new Quaternion(gameObject.transform.rotation.x, mainCam.transform.rotation.y, gameObject.transform.rotation.z, mainCam.transform.rotation.w);
        }
        
        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, newRot, 5f);
    }
}
