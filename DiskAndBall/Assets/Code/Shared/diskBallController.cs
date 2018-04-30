using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intelligence;


public class diskBallController : MonoBehaviour
{

	public Transform disk, ball;
	public diskController diskCtrl;
	public ballController ballCtrl;
	public manualDiskControl manualDiskCtrl;

	public Vector3 diskStartPosition, ballStartPosition;


	void Start()
	{
		diskCtrl = disk.GetComponent<diskController>();
		ballCtrl = ball.GetComponent<ballController>();
		manualDiskCtrl = disk.GetComponent<manualDiskControl>();
		diskStartPosition = diskCtrl.GetPosition();
		ballStartPosition = ballCtrl.GetPosition();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) Restart();
	}

	public void Restart()
	{
		diskCtrl.SetPosition(diskStartPosition);
		diskCtrl.SetRotation(Vector3.zero);
		diskCtrl.SetAngularVelocity(Vector3.zero);
		manualDiskCtrl.SetManualControl(false);
		ballCtrl.SetPosition(ballStartPosition);
		ballCtrl.SetRotation(Vector3.zero);
		ballCtrl.SetVelocity(Vector3.zero);
		ballCtrl.SetAngularVelocity(Vector3.zero);
	}
}
