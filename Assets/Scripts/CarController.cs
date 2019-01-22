using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarController : MonoBehaviour
{
    public int hopForce = 10;
    public int hopTorqueForce = 10;
    public float suspensionForce = 3;

    Rigidbody rb;
    Transform chassis;

    Vector3[] wheelLocalPos = new Vector3[4];
    Vector3[] wheelWorldPos = new Vector3[4];
    float[] wheelCompress = new float[4];

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
        wheelLocalPos[0] = new Vector3 (-halfCarWidth, -halfCarHeight + .1f, halfCarLength);
        //Front right:
        wheelLocalPos[1] = new Vector3 (halfCarWidth, -halfCarHeight + .1f, halfCarLength);
        //Back left:
        wheelLocalPos[2] = new Vector3 (-halfCarWidth, -halfCarHeight + .1f, -halfCarLength);
        //Back right:
        wheelLocalPos[3] = new Vector3 (halfCarWidth, -halfCarHeight + .1f, -halfCarLength);
    }

    void Update()
    {
        if (Input.GetButtonDown ("Jump")) {
            Hop();
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            //place start point of each ray relative to location and orientation of the car
            Vector3 startPos = transform.position +
                (transform.forward * wheelLocalPos[i].z) +
                (transform.right * wheelLocalPos[i].x) +
                (transform.up * wheelLocalPos[i].y);

            //save start point in another array for the gizmos
            wheelWorldPos[i] = startPos;

            RaycastHit hit;
            if (Physics.Raycast(startPos, -transform.up, out hit, 1.1f, layerMask, QueryTriggerInteraction.Ignore)) {
                Debug.DrawLine(startPos, hit.point, Color.green);
                //calculate wheel compression percent based on clamped distance between ray and hit point (adjusted for the .1f overlap with the collider)
                //may want to change that clamp01 to a lerp later so I can have variable suspension height
                wheelCompress[i] = 1f - Mathf.Clamp01(Vector3.Distance(startPos, hit.point) - .1f);

                Suspend(startPos, wheelCompress[i]);

            } else {
                Debug.DrawRay(startPos, -transform.up, Color.red);
                wheelCompress[i] = 0f;
            }
        }
    }

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Test");
        for (int i = 0; i < 4; i++)
        {
            Handles.Label(wheelWorldPos[i], wheelCompress[i].ToString());
        }
    }
    
    void Hop() {
        Vector3 tq = Random.onUnitSphere;
        rb.AddForce(Vector3.up * hopForce, ForceMode.Impulse);
        rb.AddTorque(tq * hopTorqueForce, ForceMode.Impulse);
    }

    void Suspend(Vector3 forcePos, float compressPercent) {
        rb.AddForceAtPosition(Vector3.up * suspensionForce * compressPercent, forcePos, ForceMode.Acceleration);
    }
}
