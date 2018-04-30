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

	enum EvaluationMode
	{	None, Neural, Fuzzy, TrainNeural}
	EvaluationMode mode = EvaluationMode.None;
	bool pause = false;

	public Vector3 diskStartPosition, ballStartPosition;


	void Start()
	{
		diskCtrl = disk.GetComponent<diskController>();
		ballCtrl = ball.GetComponent<ballController>();
		manualDiskCtrl = disk.GetComponent<manualDiskControl>();
		diskStartPosition = diskCtrl.GetPosition();
		ballStartPosition = ballCtrl.GetPosition();
		Neural.CreateNetwork();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) Restart();
	}

	void FixedUpdate()
	{
		if (Vector3.Distance(ballCtrl.GetPosition(), diskCtrl.GetPosition()) > 5) Restart();
		switch (mode)
		{
			case EvaluationMode.Neural:
				OutputData neuralOutput = Neural.Evaluate(PrepareData());
				diskCtrl.SetAngularVelocity(FloatToVector3(neuralOutput.outputDiskRotationSpeedVector));
				break;

		}
	}

	InputData PrepareData()
	{
		return new InputData(Vector3ToFloat(diskCtrl.GetRotation()),
							 Vector3ToFloat(diskCtrl.GetAngularVelocity()),
							 Vector3ToFloat(ballCtrl.GetPosition()),
							 Vector3ToFloat(ballCtrl.GetVelocity()));
	}

	float[] Vector3ToFloat(Vector3 v)
	{
		return new float[] { v.x, v.y, v.z };
	}
	Vector3 FloatToVector3(float[] f)
	{
		return new Vector3(f[0], f[1], f[2]);
	}

	public void SetMode(int newmode)
	{
		mode = (EvaluationMode)newmode;
	}

	public void Pause()
	{
		pause = !pause;
	}

	public void Restart()
	{
		pause = false;
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
