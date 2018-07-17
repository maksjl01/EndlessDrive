using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlate : MonoBehaviour {
     
    public float spinSpeed = 1;
    public int spinDirection = 1;

    public Transform car;

    void Spin()
    {
        transform.Rotate(Vector3.up, (spinSpeed * spinDirection) * Time.deltaTime);
        car.RotateAround(transform.position, Vector3.up, (spinSpeed * spinDirection) * Time.deltaTime); 
    }

    private void Update()
    {
        Spin();
    }
}
