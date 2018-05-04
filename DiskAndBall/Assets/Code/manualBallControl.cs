using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manualBallControl : MonoBehaviour {


	ballController ballCtrl;
	public bool enabledControl = false;
	public float multiplier = 1;
	

	void Start()
	{
		ballCtrl = GetComponent<ballController>();
	}

	void Update()
	{
		if (enabledControl &&(Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical")!=0))
		{
			ballCtrl.SetVelocity(new Vector3(
				Input.GetAxis("Horizontal"),
				Input.GetAxis("Vertical"),
				0)*multiplier);
		}
		else
		{
			
		}
		if (Input.GetKeyDown(KeyCode.N)) enabledControl = !enabledControl;

	}

	public void SetManualControl(bool enable)
	{
		enabledControl = enable;
	}
}
