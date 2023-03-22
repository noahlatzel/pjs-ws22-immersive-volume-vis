using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
	public GameObject leftTeleportation;
	public GameObject rightTeleportation;
	//public GameObject rightGrab;
	//public GameObject leftGrab;
	
	public InputActionProperty leftActivate;
	public InputActionProperty rightActivate;

	public InputActionProperty leftCancel;
	public InputActionProperty rightCancel;
	
	public XRRayInteractor leftRay;
	public XRRayInteractor rightRay;

	// private InteractionCubeStandalone interactionCube;
	// private bool interactionCubeExists = false;
	
	// void Start()
	// {
	// 	if (GameObject.Find("InteractionCubeStandalone") != null)
	// 	{
	// 		if (GameObject.Find("InteractionCubeStandalone").GetComponent<InteractionCubeStandalone>() != null)
	// 		{
	// 			interactionCube = GameObject.Find("InteractionCubeStandalone")
	// 				.GetComponent<InteractionCubeStandalone>();
	// 			interactionCubeExists = true;
	// 		}
	// 	}
	// 	
	// 	
	// }

	void Update()
	{
		// bool isInArea = false;
		//
		// if (interactionCubeExists)
		// {
		// 	isInArea = interactionCube.AreHandsNearCube();
		// }
		// leftTeleportation.SetActive(!isInArea && /*leftActivate.action.ReadValue<float>() > 0.1f &&*/ leftCancel.action.ReadValue<float>() == 0);
		// rightTeleportation.SetActive(!isInArea && /*rightActivate.action.ReadValue<float>() > 0.1f &&*/ rightCancel.action.ReadValue<float>() == 0);
		//leftGrab.SetActive(!isInArea);
		//rightGrab.SetActive(!isInArea);
		
		bool isLeftRayHovering = leftRay.TryGetHitInfo(out Vector3 leftPos, out Vector3 leftNormal, out int leftNumber, out bool leftValid);
		leftTeleportation.SetActive(!isLeftRayHovering && leftCancel.action.ReadValue<float>() == 0 && leftActivate.action.ReadValue<float>() > 0.1f);
			
		bool isRightRayHovering = rightRay.TryGetHitInfo(out Vector3 rightPos, out Vector3 rightNormal, out int rightNumber, out bool rightValid);
		rightTeleportation.SetActive(!isRightRayHovering && rightCancel.action.ReadValue<float>() == 0 && rightActivate.action.ReadValue<float>() > 0.1f);
	}
}