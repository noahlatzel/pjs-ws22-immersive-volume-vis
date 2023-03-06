using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
	public GameObject leftTeleportation;
	public GameObject rightTeleportation;
	public GameObject rightGrab;
	public GameObject leftGrab;
	
	public InputActionProperty leftActivate;
	public InputActionProperty rightActivate;

	
	void Update()
	{
		bool isInArea = GameObject.Find("InteractionCubeStandalone").GetComponent<InteractionCubeStandalone>().AreHandsNearCube();
		leftTeleportation.SetActive(!isInArea && leftActivate.action.ReadValue<float>() > 0.1f);
		rightTeleportation.SetActive(!isInArea && rightActivate.action.ReadValue<float>() > 0.1f);
		leftGrab.SetActive(!isInArea);
		rightGrab.SetActive(!isInArea);
	}
}