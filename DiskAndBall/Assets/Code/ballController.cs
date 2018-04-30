using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballController : MonoBehaviour
{

	Rigidbody r;

	public bool outOfBounds = false;

	void Start()
	{
		r = GetComponent<Rigidbody>();

	}

	void Update()
	{

	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public Vector3 GetRotation()
	{
		return transform.eulerAngles;
	}

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

	public void SetRotation(Vector3 rot)
	{
		transform.eulerAngles = rot;

	}

	public Vector3 GetVelocity()
	{
		return r.velocity;
	}

	public void SetVelocity(Vector3 vel)
	{
		r.velocity = vel;
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
