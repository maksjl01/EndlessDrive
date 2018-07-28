using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlate : MonoBehaviour {
     
    public float spinSpeed = 1;
    public int spinDirection = 1;

    void Spin()
    {
        transform.Rotate(Vector3.up, (spinSpeed * spinDirection) * Time.deltaTime);
    }

    public void ResetRotation(Vector3 camDirection)
    {
        transform.rotation = Quaternion.LookRotation(-camDirection);
    }

    private void Update()
    {
        Spin();
    }
}
