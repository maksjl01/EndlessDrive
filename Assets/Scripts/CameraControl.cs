﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public GameObject target;
	public float radius;
	public float angle;
	public Vector3 offset;
	public float lerpSpeed;
	Vector3 speefRef;

	Vector3 targetPosition;
	Vector3 desiredPos;

	private void LateUpdate()
	{
		DesiredPos();
		transform.position = desiredPos;
		transform.LookAt(target.transform);
	}

	void DesiredPos()
	{
		targetPosition = target.transform.position;
		desiredPos = new Vector3((Mathf.Cos(angle) * radius), offset.y, (Mathf.Sin(angle) * radius));
		desiredPos += targetPosition;
		desiredPos += offset;
	}
}
