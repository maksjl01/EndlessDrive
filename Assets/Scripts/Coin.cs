using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    private const string COIN_TAG = "Coin";

    private float p_spinSpeed = 1;
    private int p_spinDirection = 1;
    private float p_ossillationHeight = 1;

    public float SpinSpeed { set { p_spinSpeed = value; } }
    public int SpinDirection { set { p_spinDirection = value; } }
    public float OssilationHeight { set { p_ossillationHeight = value; } }
    public float OssilationStartHeight;

    private float p_t;

    private float startYPos;

    private void Start()
    {
        tag = COIN_TAG;
        startYPos = transform.position.y;
        GetComponent<Collider>().isTrigger = true;
    }

    void Spin()
    {
        p_t += Time.deltaTime;
        transform.position = new Vector3(transform.position.x , (startYPos + OssilationStartHeight) + (Mathf.Sin(p_t) * p_ossillationHeight), transform.position.z);
        transform.Rotate(Vector3.up, p_spinSpeed * p_spinDirection);
    }

    private void Update()
    {
        Spin();
    }
}
