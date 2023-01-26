using UnityEngine;

public class TrackpadRotation : MonoBehaviour
{
    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float trackpadX = Input.GetAxis("Trackpad X");

        transform.Rotate(0, trackpadX * speed * Time.deltaTime, 0, Space.Self);
    }
}
