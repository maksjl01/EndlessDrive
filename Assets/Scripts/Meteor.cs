using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

    private const string METEOR_TAG = "Meteor";

    public float Mass;
    public float Damage;

    public GameObject marker;
    GameObject localMarker;

    private void Start()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = Mass;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        tag = METEOR_TAG;

        if (!GetComponent<Collider>())
            gameObject.AddComponent<MeshCollider>().convex = true;

        ProjectImageDown();

        StartCoroutine(DeadCheck());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Car")
        {
            Player.TakeDamage(Damage);
            print("car hit");
        } else if(collision.gameObject.tag == "Terrain")
        {
            StartCoroutine(DelayedDestroy(0.2f));
        }
    }

    IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(localMarker);
        Destroy(gameObject);
    }

    IEnumerator DeadCheck()
    {
        yield return new WaitForSeconds(9);
        if (gameObject)
        {
            Destroy(gameObject);
            Destroy(localMarker);
        }
    }

    void ProjectImageDown()
    {
        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(transform.position, Vector3.down * 10, out hit, 50))
        {
            localMarker = Instantiate(marker, hit.point, Quaternion.identity);
            StartCoroutine(Spin(localMarker));
        }
    }

    IEnumerator Spin(GameObject obj)
    {
        while (true)
        {
            obj.transform.Rotate(Vector3.up, 10);
            yield return null;
        }
    }
}
