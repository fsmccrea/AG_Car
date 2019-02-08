using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform theCar;
    public Transform camRig;
    public Transform camFocus;
    public Camera theCamera;

    public float posSmoothTime = 10f;
    public float rotSmoothTime = 10f;

    public Vector3 camOffset = new Vector3 (0, 4f, -10f);

    Transform carFocus;

    void Start()
    {
        carFocus = theCar.GetChild(0);
    //    Offset();
    }

    void LateUpdate()
    {
        RootFollow();
        Offset();
        CamFocus();
    }

    void RootFollow()
    {
        //follow car's position
        Vector3 _carPos = new Vector3 (theCar.position.x, 0, theCar.position.z);
        
        //quaternion facing car's forward direction (exclusively)
        Quaternion _forward = Quaternion.Euler(Vector3.Scale(theCar.rotation.eulerAngles, Vector3.up));

        transform.position = Vector3.Lerp (transform.position, _carPos, Time.deltaTime * posSmoothTime);//Vector3.Lerp (transform.position, _carPos, Time.deltaTime);

        transform.rotation = Quaternion.Lerp (transform.rotation, _forward, Time.deltaTime * rotSmoothTime);
    }

    void Offset()
    {
        //offset from root by a camOffset amount
    //    Vector3 _rigPos = new Vector3 (0, camHeight, rigOffset);
    //    transform.localPosition = _rigPos;
        Vector3 _rigPos = transform.position +
            (transform.forward * camOffset.z) +
            (transform.right * camOffset.x) +
            (transform.up * camOffset.y);

        //offset from rig
    //    Vector3 _camPos = new Vector3 (0, camHeight, 0);
    //    transform.position = _camPos;
    }

    void CamFocus()
    {
        //follow CAR's focal point
        camFocus.position = carFocus.position;

        //look at focal point
        Vector3 _lookDirection = camFocus.position - theCamera.transform.position;
        Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);

        theCamera.transform.rotation = _rot;
    }

}
