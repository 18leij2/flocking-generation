using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlockScript : MonoBehaviour
{
    [Header("Values to Customize")]
    public int numBoids = 20;
    public bool trails = false;
    public bool flockCenterEnabled = true;
    public bool velMatchEnabled = true;
    public bool collAvoidEnabled = true;
    public bool wanderEnabled = true;

    [Header("Others")]
    public GameObject boidObject;
    public GameObject pellet;
    public List<Boid> boidlings;
    public float maxSpeed = 5f;
    public float neighborRadius = 5f;
    public float fcWeight = 1f; // flock centering weight
    public float vmWeight = 1f; // velocity matching weight
    public float caWeight = 1f; // collision avoidance weight
    public float wWeight = 1f; // wandering weight

    private int boidNum = 0;
    private float timer = 0f;
    private float interval = 0.5f;

    void Start()
    {
        boidlings = new List<Boid>();
        for (int i = 0; i < numBoids; i++)
        {
            createBoid();
        }

        boidNum = numBoids;
    }

    void Update()
    {
        // scatter boids
        if (Input.GetKeyDown(KeyCode.Space))
        {    
            foreach (var boider in boidlings)
            {
                Vector3 randomSpawn = new Vector3(Random.Range(-9f, 9f), Random.Range(-9f, 9f), Random.Range(-9f, 9f));
                boider.pos = randomSpawn;
            }
        }

        // boid number matching
        while (boidNum != numBoids)
        {
            if (boidNum > numBoids)
            {
                Boid removeBoid = boidlings[0];
                if (removeBoid != null)
                {
                    boidlings.RemoveAt(0);
                    Destroy(removeBoid.gameObject);
                }
                boidNum--;
            }

            if (boidNum < numBoids)
            {
                createBoid();
                boidNum++;
            }
        }

        // apply forces to each boid
        foreach (var boider in boidlings)
        {
            flockingForces(boider);

            // boundary detection
            if (boider.transform.position.x < -10f || boider.transform.position.x > 10f)
            {
                boider.vel.x = -boider.vel.x;
            }
            if (boider.transform.position.y < -10f || boider.transform.position.y > 10f)
            {
                boider.vel.y = -boider.vel.y;
            }
            if (boider.transform.position.z < -10f || boider.transform.position.z > 10f)
            {
                boider.vel.z = -boider.vel.z;
            }

            // trail generation
            if (trails)
            {
                timer += Time.deltaTime;
                if (timer >= interval)
                { 
                    Instantiate(pellet, boider.transform.position, Quaternion.identity);
                    timer = 0f;
                }
            }
        }
    }

    // code to apply the 4 forces: flock centering, velocity matching, collision avoidance, wandering
    void flockingForces(Boid boid)
    {
        // FLOCK CENTERING and VELOCITY MATCHING and COLLISION AVOIDANCE
        Vector3 flockForce = Vector3.zero;
        Vector3 velocityMatch = Vector3.zero;
        Vector3 collisionAvoidance = Vector3.zero;
        Vector3 randomWander = Vector3.zero;
        List<Boid> neighboringBoids = findNeighbors(boid, neighborRadius);
        float totalWeight = 0;
        // float collisionWeight = 0;
        foreach (Boid otherBoid in neighboringBoids)
        {
            // linear weighted flock centering, velocity matching, collision avoidance
            float dist = Vector3.Distance(boid.transform.position, otherBoid.transform.position);
            float weight = Mathf.Max(neighborRadius - dist, 0);
            totalWeight += weight;

            // FLOCK CENTERING
            // flockForce += weight * (otherBoid.transform.position - boid.transform.position);
            if (flockCenterEnabled) flockForce += otherBoid.transform.position;

            // VELOCITY MATCHING
            // velocityMatch += (weight) * (otherBoid.vel - boid.vel);
            if (velMatchEnabled) velocityMatch += otherBoid.vel;

            // COLLISION AVOIDANCE
            /*if (dist <= 2)
            {
                float currWeight = Mathf.Max(2 - dist, 0);
                collisionAvoidance += (currWeight) * (boid.transform.position - otherBoid.transform.position);
                collisionWeight += currWeight;
            }  */
            if (collAvoidEnabled) collisionAvoidance += (boid.transform.position - otherBoid.transform.position) / (dist * dist);
        }
        /*if (totalWeight > 0)
        {
            flockForce /= totalWeight;
            velocityMatch /= totalWeight;
        }
        if (collisionWeight > 0)
        {
            collisionAvoidance /= collisionWeight;
        }*/
        if (neighboringBoids.Count > 0)
        {
            if (flockCenterEnabled) flockForce = ((flockForce / neighboringBoids.Count) - boid.transform.position).normalized;
            if (velMatchEnabled) velocityMatch = (velocityMatch / neighboringBoids.Count).normalized;
        }
        
        // WANDERING
        if (wanderEnabled) randomWander = Random.insideUnitSphere;

        Vector3 newVel = boid.vel + flockForce * fcWeight + velocityMatch * vmWeight + collisionAvoidance * caWeight + randomWander * wWeight;
        boid.updateVelocity(newVel);
    }

    // helper method to find the neighbors for flock centering
    public List<Boid> findNeighbors(Boid boid, float radius)
    {
        List<Boid> returnNeighbors = new List<Boid>();

        foreach (Boid otherBoid in boidlings)
        {

            // if it's not the current boid and it's within range
            if ((otherBoid != boid) && (Vector3.Distance(boid.transform.position, otherBoid.transform.position) <= radius))
            {
                returnNeighbors.Add(otherBoid);
            }
        }

        return returnNeighbors;
    }

    // create the boidling
    void createBoid()
    {
        // random position within 18x18x18 square to fit within 20x20x20 borders
        Vector3 randomSpawn = new Vector3(Random.Range(-9f, 9f), Random.Range(-9f, 9f), Random.Range(-9f, 9f));

        GameObject newBoid = Instantiate(boidObject, randomSpawn, Random.rotation);
        Boid boidScript = newBoid.GetComponent<Boid>();
        boidlings.Add(boidScript);
    }
}
