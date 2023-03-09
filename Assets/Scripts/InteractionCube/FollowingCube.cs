using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCube : MonoBehaviour
{
    GameObject xrOrigin;
    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        xrOrigin = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        float trackpadY = Input.GetAxis("Trackpad Y");
        if(trackpadY == 1)
        {
            Vector3 destination = xrOrigin.transform.position;

            transform.position = Vector3.MoveTowards(transform.position, destination, trackpadY * Time.deltaTime * speed);
        }
    }
}