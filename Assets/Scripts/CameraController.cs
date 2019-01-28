using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject theCar;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = theCar.transform.position;
    }
}
