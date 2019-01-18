using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int bounceForce = 10;
    public int bounceTorqueForce = 10;

    Rigidbody rb;
    Transform chassis;
    Vector3[] wheelPos = new Vector3[4];

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        chassis = gameObject.transform.GetChild(0);

        float halfCarWidth = chassis.localScale.x / 2;
        float halfCarLength = chassis.localScale.z / 2;
        float halfCarHeight = chassis.localScale.y / 2;

        //Array of wheel raycast origin points based on scale of car collider
        //Front left wheel:
        wheelPos[0] = new Vector3 (-halfCarWidth, -halfCarHeight, halfCarLength);
        //Front right:
        wheelPos[1] = new Vector3 (halfCarWidth, -halfCarHeight, halfCarLength);
        //Back left:
        wheelPos[2] = new Vector3 (-halfCarWidth, -halfCarHeight, -halfCarLength);
        //Back right:
        wheelPos[3] = new Vector3 (halfCarWidth, -halfCarHeight, -halfCarLength);
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
            //Debug.DrawRay((transform.position - transform.up * .5f) + (transform.right * thePos.x) + (transform.forward * thePos.y), -transform.up * .5f, Color.red);
            Debug.DrawRay(
                transform.position +
                (transform.forward * thePos.z) +
                (transform.right * thePos.x) +
                (transform.up * thePos.y)
            , -transform.up * .5f, Color.red);
        }
    }
    
    void Bounce() {
        Vector3 tq = Random.onUnitSphere;
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        rb.AddTorque(tq * bounceTorqueForce, ForceMode.Impulse);
    }
}
