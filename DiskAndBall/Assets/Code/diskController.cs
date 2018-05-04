using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diskController : MonoBehaviour {
	
	Rigidbody r;

	void Start () {
		r = GetComponent<Rigidbody>();
	}
	
	void Update () {
		
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public Vector3 GetRotation()
	{
		Vector3 rot = transform.eulerAngles;
		if (rot.x > 180) rot.x -= 360;
		if (rot.y > 180) rot.y-= 360;
		if (rot.z > 180) rot.z -= 360;
		return rot;
	}

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	public void SetRotation(Vector3 rot)
	{
		transform.eulerAngles=rot;
	}
	public Vector3 GetAngularVelocity()
	{
		return r.angularVelocity;
	}

	public void SetAngularVelocity(Vector3 vel)
	{
		r.angularVelocity = vel;
	}
}
