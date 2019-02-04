using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform theCar;
    public Transform camRig;
    public Transform camFocus;
    public Camera theCamera;

    public float rigOffset = -10f;
    public float camHeight = 4f;

    Transform carFocus;

    void Start()
    {
        carFocus = theCar.GetChild(0);
        RigOffset();
        CamOffset();
    }

    void FixedUpdate()
    {
        RootFollow();
        Focus();
        CamFocus();
    }

    void RootFollow()
    {
        //follow car's position
        Vector3 _carPos = new Vector3 (theCar.position.x, 0, theCar.position.z);
        transform.position = _carPos;
    }

    void RigOffset()
    {
        //offset from root by a specified amount
        Vector3 _rigPos = new Vector3 (0, 0, rigOffset);
        transform.localPosition = _rigPos;
    }

    void Focus()
    {
        //follow CAR's focal point
        camFocus.position = carFocus.position;
    }

    void CamOffset()
    {
        //offset from rig
        Vector3 _camPos = new Vector3 (0, camHeight, 0);
        transform.position = _camPos;
    }

    void CamFocus()
    {
        //look at focal point
        Vector3 _lookDirection = camFocus.position - theCamera.transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);

        theCamera.transform.rotation = _rot;
    }

}
