using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 vel;
    public Vector3 pos;
    public bool trail;
    
    private float maxSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        vel = transform.forward * 2;
    }

    // Update is called once per frame
    void Update()
    {
        pos += vel * Time.deltaTime; // move the boid forward
        transform.position = pos;

        // make sure it's always facing direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(vel.normalized); // get the rotation based on velocity
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // smooth rotation
    }

    public void updateVelocity(Vector3 newVelocity)
    {
        vel = Vector3.ClampMagnitude(newVelocity, maxSpeed);  // clamp velocity
        if (vel.magnitude < 2f)
        {
            vel = vel.normalized * 2; // if velocity magnitude is <2, make it 2
        }
    }
}
