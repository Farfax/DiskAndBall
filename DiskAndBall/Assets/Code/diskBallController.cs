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
	public manualBallControl manualBallCtrl;

	enum EvaluationMode
	{ None, Neural, Fuzzy, TrainNeural }
	EvaluationMode mode = EvaluationMode.None;
	bool pause = false;

	public Vector3 diskStartPosition, ballStartPosition;

	NeuralNetwork nn;
	public float learnRate = 0.01f;

	void Start()
	{
		diskCtrl = disk.GetComponent<diskController>();
		ballCtrl = ball.GetComponent<ballController>();
		manualDiskCtrl = disk.GetComponent<manualDiskControl>();
		manualBallCtrl = ball.GetComponent<manualBallControl>();
		diskStartPosition = diskCtrl.GetPosition();
		ballStartPosition = ballCtrl.GetPosition();
		List<float[,]> weights = new List<float[,]>();
		weights.Add(new float[,] { { 0,0,0,0,0,0,1,0,0,0,0,0}, { 0,0,0,0,0,0,0,0,1,0,0,0},
								   { 1,0,0,0,0,0,0,0,0,0,0,0}, { 0,0,1,0,0,0,0,0,0,0,0,0}});
		weights.Add(new float[,] { { 0,1,0,-1},
								   { 0,0,0,0},
								   { 1,0,-1,0}});
		//	Neural.CreateNetwork(new int[] { 3 * 4, 4, 3 }, weights);
		Neural.CreateNetwork(new int[] { 3 * 4, 20, 3 });
		Neural.Randomize();
		nn = Neural.GetNetwork();
		Neural.learnRate = learnRate;
		GenerateNetwork();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) { Restart(); SetMode(0); }
	}

	void FixedUpdate()
	{
		if (ballCtrl.GetPosition().y - diskCtrl.GetPosition().y < -5 ||
			Mathf.Abs(ballCtrl.GetPosition().x - diskCtrl.GetPosition().x) > 5 ||
			Mathf.Abs(ballCtrl.GetPosition().z - diskCtrl.GetPosition().z) > 5) Restart();
		switch (mode)
		{
			case EvaluationMode.Neural:
				OutputData neuralOutput = Neural.Evaluate(PrepareData());
				diskCtrl.SetAngularVelocity(FloatToVector3(neuralOutput.outputDiskRotationSpeedVector));
				break;
			case EvaluationMode.TrainNeural:
				SimpleAI(PrepareData());
				OutputData neuralOutputExp = new OutputData(Vector3ToFloat(diskCtrl.GetAngularVelocity()));
				if (diskCtrl.GetAngularVelocity() != Vector3.zero)
					Neural.Train(PrepareData(), neuralOutputExp);
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
		if (newmode == 1)
		{
			ballCtrl.Randomize();

		}
		if (newmode == 3)
		{
			//	manualDiskCtrl.SetManualControl(true);
			ballCtrl.Randomize();
		}
	}

	public void Pause()
	{
		pause = !pause;
	}

	public void GenerateNetwork()
	{
		List<float[,]> weights = new List<float[,]>();
		weights.Add(new float[,] { { 0,0,0,0,0,0,1,0,0,0,0,0,0}, { 0,0,0,0,0,0,0,0,-1,0,0,0,0},
								   { 0,0,0,-0.5f,0,0,0,0,0,1,0,0,0}, { 0,0,0,0,0,0.5f,0,0,0,0,0,-1,0}});
		weights.Add(new float[,] { { 0,0.1f,0,0.5f,0},
								   { 0,0,0,0,0},
								   { 0.1f,0,0.5f,0,0}});
		Neural.CreateNetwork(new int[] { 3 * 4+1, 5, 3 }, weights);
		nn = Neural.GetNetwork();
	}

	public void Restart()
	{
		pause = false;
		diskCtrl.SetPosition(diskStartPosition);
		diskCtrl.SetRotation(Vector3.zero);
		diskCtrl.SetAngularVelocity(Vector3.zero);
		manualDiskCtrl.SetManualControl(false);
		manualBallCtrl.SetManualControl(false);
		ballCtrl.SetPosition(ballStartPosition);
		ballCtrl.SetRotation(Vector3.zero);
		ballCtrl.SetVelocity(Vector3.zero);
		ballCtrl.SetAngularVelocity(Vector3.zero);
	}

	void SimpleAI(InputData input)
	{
		diskCtrl.SetAngularVelocity(new Vector3(-input.inputBallPositionVector[2]/10- input.inputBallSpeedVector[2]/2 + input.inputDiskRotationSpeedVector[2]/4, 0,
												input.inputBallPositionVector[0]/10 + input.inputBallSpeedVector[0]/2 - input.inputDiskRotationSpeedVector[0]/4) );
	}

	public void OnDrawGizmos()
	{
		if (nn != null)
		{
			int x = 0, y = 0, y1, lastYMax;
			for (int i = 0; i < nn.firstLayerSize; i++)
			{
				Gizmos.color = new Color(0.5f, 1, 1, 0.5f);
				Gizmos.DrawCube(new Vector3(x * 3, y - nn.firstLayerSize / 2, 0) + Vector3.right * 10, Vector3.one / 5);
				y++;
			}
			lastYMax = nn.firstLayerSize;
			x++;
			foreach (NeuralLayer l in nn.GetLayers())
			{
				y = 0;
				foreach (NeuralNode n in l.GetNodes())
				{
					Gizmos.color = new Color(0.5f, 1, 1, 0.5f);
					Gizmos.DrawCube(new Vector3(x * 3, y - l.GetNodes().Length / 2, 0) + Vector3.right * 10, Vector3.one / 10);
					y1 = 0;
					foreach (float f in n.GetWeights())
					{
						if (f > 1)
							Gizmos.color = new Color(1 - f / 50, 1 - f / 50, 1, 1);
						else if (f < 0)
							Gizmos.color = new Color(1 + f / 50, 0, 0, 1);
						else
							Gizmos.color = new Color(1, 1, 1, f);
						Gizmos.DrawLine(new Vector3(x * 3, y - l.GetNodes().Length / 2, 0) + Vector3.right * 10,
										new Vector3((x - 1) * 3, y1 - lastYMax / 2, 0) + Vector3.right * 10);
						y1++;
					}
					y++;
				}
				x++;
				lastYMax = l.GetNodes().Length;
			}
		}
	}
}
