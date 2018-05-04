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

	public void Randomize()
	{
		SetPosition(new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 3f), Random.Range(-1f, 1f)));
		SetVelocity((GetPosition() - Vector3.up * GetPosition().y) * Random.Range(0f, 2f));
	}

	public Vector3 GetPosition()
	{
		return r.position;
	}

	public Vector3 GetRotation()
	{
		return transform.eulerAngles;
	}

	public void SetPosition(Vector3 pos)
	{
		r.MovePosition(pos);
		//transform.position = pos;
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
