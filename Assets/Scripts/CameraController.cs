using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform vehiclePos;
    Vector3 offset;

    public void SetUp() {
        vehiclePos.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(0, 0)).x + 3f, vehiclePos.position.y, 0);
        CarController carController = vehiclePos.gameObject.GetComponent<CarController>();
        carController.StartPos = vehiclePos.position;
        offset = transform.position - vehiclePos.position;
    }

    // Update is called once per frame
    void Update()
    {
        //카메라가 차량을 따라다님
        transform.position = vehiclePos.position + offset;  
    }
}
