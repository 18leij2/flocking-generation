using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailScript : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * rotationSpeed;
        float zRotation = Mathf.Sin(time) * 15f;
        Quaternion parentRotation = transform.parent.rotation;
        transform.rotation = parentRotation * Quaternion.Euler(-50f, 0f, zRotation);
    }
}
