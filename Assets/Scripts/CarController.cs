using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarController : MonoBehaviour
{
    public int bounceForce = 10;
    public int bounceTorqueForce = 10;

    Rigidbody rb;
    Transform chassis;
    Vector3[] wheelPos = new Vector3[4];

    int layerMask;

    void Start()
    {
        layerMask = 1 << 2;
        layerMask = ~layerMask;

        rb = GetComponent<Rigidbody>();
        chassis = gameObject.transform.GetChild(0);

        float halfCarWidth = chassis.localScale.x / 2;
        float halfCarLength = chassis.localScale.z / 2;
        float halfCarHeight = chassis.localScale.y / 2;

        //Array of wheel raycast origin points based on scale of car collider
        //Front left wheel:
        wheelPos[0] = new Vector3 (-halfCarWidth, -halfCarHeight + .1f, halfCarLength);
        //Front right:
        wheelPos[1] = new Vector3 (halfCarWidth, -halfCarHeight + .1f, halfCarLength);
        //Back left:
        wheelPos[2] = new Vector3 (-halfCarWidth, -halfCarHeight + .1f, -halfCarLength);
        //Back right:
        wheelPos[3] = new Vector3 (halfCarWidth, -halfCarHeight + .1f, -halfCarLength);
    }

    void Update()
    {
        if (Input.GetButtonDown ("Jump")) {
            Bounce();
        }
    }

    void FixedUpdate()
    {
        foreach (Vector3 thePos in wheelPos)
        {
            //gotta switch this loop to a for loop so i could use it in other spots too)

            //Debug.DrawRay((transform.position - transform.up * .5f) + (transform.right * thePos.x) + (transform.forward * thePos.y), -transform.up * .5f, Color.red);
            /*Debug.DrawRay(
                transform.position +
                (transform.forward * thePos.z) +
                (transform.right * thePos.x) +
                (transform.up * thePos.y)
            , -transform.up * .5f, Color.red);
            */
            Vector3 startPos = transform.position +
                (transform.forward * thePos.z) +
                (transform.right * thePos.x) +
                (transform.up * thePos.y);

            RaycastHit hit;
            if (Physics.Raycast(startPos, -transform.up, out hit, 1.1f, layerMask, QueryTriggerInteraction.Ignore)) {
                Debug.DrawLine(startPos, hit.point, Color.green);
                print("hit!");
            } else {
                Debug.DrawRay(startPos, -transform.up, Color.red);
            }
        }
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Test");
        foreach (Vector3 thePos in wheelPos)
        {
            Handles.Label(thePos, "gotem");
        }
    }
    
    void Bounce() {
        Vector3 tq = Random.onUnitSphere;
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        rb.AddTorque(tq * bounceTorqueForce, ForceMode.Impulse);
    }
}
