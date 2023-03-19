using System;
using UnityEngine;

public class FollowingCube : MonoBehaviour
{
    GameObject xrOriginHand;
    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        xrOriginHand = GameObject.Find("RightHand");
    }

    // Update is called once per frame
    void Update()
    {
        float trackpadY = Input.GetAxis("Trackpad Y");
        if(Math.Abs(trackpadY - 1) < 0.001f)
        {
            Vector3 destination = xrOriginHand.transform.position;

            transform.position = Vector3.MoveTowards(transform.position, destination, trackpadY * Time.deltaTime * speed);
        }
    }
}