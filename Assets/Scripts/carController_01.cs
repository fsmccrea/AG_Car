using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController_01 : MonoBehaviour
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
        axleInfos[1].leftWheelW.ConfigureVehicleSubsteps(5f,12,15);

        rb.centerOfMass += new Vector3 (0, -.5f, 1.0f);
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
                axleInfo.leftWheelW.steerAngle = steering;
                axleInfo.rightWheelW.steerAngle = steering;
                StabilizerBars(antiRollFront, axleInfo);
            }
            if (axleInfo.motor) {
                axleInfo.leftWheelW.motorTorque = motor;
                axleInfo.rightWheelW.motorTorque = motor;
                StabilizerBars(antiRollBack, axleInfo);
            }
            UpdateWheelPose(axleInfo.leftWheelT.transform, axleInfo.leftWheelW);
            UpdateWheelPose(axleInfo.rightWheelT.transform, axleInfo.rightWheelW);
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

        bool groundedL = axleInfo.leftWheelW.GetGroundHit(out hit);
        //vvv calculates ratio of compression by getting distance between the center of the wheel and the suspension origin and dividing by suspension distance
        if (groundedL)
            travelL = (-axleInfo.leftWheelW.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheelW.radius) / axleInfo.leftWheelW.suspensionDistance;
        else
            travelL = 1.0f;

        bool groundedR = axleInfo.rightWheelW.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-axleInfo.rightWheelW.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheelW.radius) / axleInfo.rightWheelW.suspensionDistance;
        else
            travelR = 1.0f;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(axleInfo.leftWheelW.transform.up * -antiRollForce, axleInfo.leftWheelW.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(axleInfo.rightWheelW.transform.up * antiRollForce, axleInfo.rightWheelW.transform.position);

    }

    private void UpdateWheelPose(Transform _mesh, WheelCollider _collider) {
        Vector3 _pos = _mesh.position;
        Quaternion _quat = _mesh.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _mesh.position = _pos;
        _mesh.rotation = _quat;
    }
}

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheelW;
    public WheelCollider rightWheelW;
    public Transform leftWheelT;
    public Transform rightWheelT;
    public bool motor; // is this axle connected to a motor
    public bool steering; // is this axle connected to steering
}
