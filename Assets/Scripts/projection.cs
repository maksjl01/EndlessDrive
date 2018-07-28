using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projection : MonoBehaviour {

	void Project()
    {
        Debug.DrawRay(transform.position, Vector3.down * 30, Color.blue, 10);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 30))
        {
            print("Hit");
            hit.collider.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_UVX", hit.textureCoord.x);
            hit.collider.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_UVY", hit.textureCoord.y);

        }
        Debug.Log(hit.collider);
    }

    private void Update()
    {
        Project();
    }
}
