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

	private InteractionCubeStandalone interactionCube;
	private bool interactionCubeExists = false;
	
	void Start()
	{
		if (GameObject.Find("InteractionCubeStandalone") != null)
		{
			if (GameObject.Find("InteractionCubeStandalone").GetComponent<InteractionCubeStandalone>() != null)
			{
				interactionCube = GameObject.Find("InteractionCubeStandalone")
					.GetComponent<InteractionCubeStandalone>();
				interactionCubeExists = true;
			}
		}
		
		
	}

	void Update()
	{
		bool isInArea = false;
		
		if (interactionCubeExists)
		{
			isInArea = interactionCube.AreHandsNearCube();
		}
		leftTeleportation.SetActive(!isInArea && /*leftActivate.action.ReadValue<float>() > 0.1f &&*/ leftCancel.action.ReadValue<float>() == 0);
		rightTeleportation.SetActive(!isInArea && /*rightActivate.action.ReadValue<float>() > 0.1f &&*/ rightCancel.action.ReadValue<float>() == 0);
		//leftGrab.SetActive(!isInArea);
		//rightGrab.SetActive(!isInArea);
	}
}