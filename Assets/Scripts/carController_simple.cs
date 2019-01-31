using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController_simple : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the info about the axles
    public float maxMotorTorque; // max torque to wheel
    public float maxSteeringAngle; // max turn angle to wheel

    public float antiRollFront = 5000f;
    public float antiRollBack = 5000f; // max force that the anti-roll bar can transfer among the springs (stiffness of anti-roll bar)

    public float hopForce = 1000;
    public float hopTorqueForce = 1000;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        axleInfos[1].leftWheel.ConfigureVehicleSubsteps(5f,12,15);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Hop();
    }

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis ("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis ("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
                StabilizerBars(antiRollFront, axleInfo);
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
                StabilizerBars(antiRollBack, axleInfo);
            }
        }
    }

    void Hop() {
        Vector3 tq = Random.onUnitSphere;
        rb.AddForce(Vector3.up * hopForce, ForceMode.Impulse);
        rb.AddTorque(tq * hopTorqueForce, ForceMode.Impulse);
    }

    void StabilizerBars(float antiRoll, AxleInfo axleInfo) {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = axleInfo.leftWheel.GetGroundHit(out hit);
        //vvv calculates ratio of compression by getting distance between the center of the wheel and the suspension origin and dividing by suspension distance
        if (groundedL)
            travelL = (-axleInfo.leftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheel.radius) / axleInfo.leftWheel.suspensionDistance;
        else
            travelL = 1.0f;

        bool groundedR = axleInfo.rightWheel.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-axleInfo.rightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheel.radius) / axleInfo.rightWheel.suspensionDistance;
        else
            travelR = 1.0f;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(axleInfo.leftWheel.transform.up * -antiRollForce, axleInfo.leftWheel.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(axleInfo.rightWheel.transform.up * -antiRollForce, axleInfo.rightWheel.transform.position);

    }
}

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this axle connected to a motor
    public bool steering; // is this axle connected to steering
}
