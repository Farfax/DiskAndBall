using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Intelligence;


public class diskBallController : MonoBehaviour
{

	public Transform disk, ball;
	public diskController diskCtrl;
	public ballController ballCtrl;
	public manualDiskControl manualDiskCtrl;
	public manualBallControl manualBallCtrl;

	public Text netName;
	public Slider[] biasSliders;

	enum EvaluationMode
	{ None, Neural, Fuzzy, TrainNeural }
	EvaluationMode mode = EvaluationMode.None;
	bool pause = false;

	public Vector3 diskStartPosition, ballStartPosition, cameraRotation;
	float cameraDistance = 14;
	Vector2 lastMouse;

	NeuralNetwork nn;
	public float learnRate = 0.01f;

	int selectedNet = -1, maxNet = -1, selectedBias = 0;
	List<string> netNames = new List<string>();
	List<int[]> netsSizes = new List<int[]>();
	List<List<float[,]>> netsWeights = new List<List<float[,]>>();

	void Start()
	{
		diskCtrl = disk.GetComponent<diskController>();
		ballCtrl = ball.GetComponent<ballController>();
		manualDiskCtrl = disk.GetComponent<manualDiskControl>();
		manualBallCtrl = ball.GetComponent<manualBallControl>();
		diskStartPosition = diskCtrl.GetPosition();
		ballStartPosition = ballCtrl.GetPosition();
		CreateLineMaterial();
		//		Neural.learnRate = learnRate;
		List<float[,]> weights = new List<float[,]>();

		weights.Add(new float[,] { { 0,0,0,0,0,0,0,0,0,0,0,0,0.0f}, { 0,0,0,0,0,0,0,0,0,0,0,0,0},
								   { 0,0,0,0,0,0,0,0,0,0,0,0,0.0f}, { 0,0,0,0,0,0,0,0,0,0,0,0,0}});
		weights.Add(new float[,] { { 0,0,0,0,0},
								   { 0,0,0,0,0},
								   { 0,0,0,0,0}});
		NewNet("Empty", new int[] { 3 * 4, 4, 3 }, weights);
		weights = new List<float[,]>();
		weights.Add(new float[,] { { 0,0,0,0,0,0,1,0,0,0,0,0,0.0f}, { 0,0,0,0,0,0,0,0,-1,0,0,0,0},
								   { 0,0,-0.3f,-0.0f,0,0,0,0,0,1,0,0,0.0f}, { -0.3f,0,0,0,0,0.0f,0,0,0,0,0,-1,0}});
		weights.Add(new float[,] { { 0,0.6f,0,1f,0},
								   { 0,0,0,0,0},
								   { 0.6f,0,1f,0,0}});
		NewNet("Centering", new int[] { 3 * 4, 4, 3 }, weights);
		weights = new List<float[,]>();
		weights.Add(new float[,] { { 0,0,0,0,0,0,0.8f,0,0,0,0,0.4f,0.0f}, { 0,0,0,0,0,0,0,0,-0.8f,0.4f,0,0,0},
								   { 0,0,-0.3f,-0.0f,0,0,0,0,0,0,0,0,0.0f}, { -0.3f,0,0,0,0,0.0f,0,0,0,0,0,0,0}});
		weights.Add(new float[,] { { 0,.5f,0,1f,0},
								   { 0,0,0,0,0},
								   { .5f,0,1f,0,0}});
		NewNet("Looping", new int[] { 3 * 4, 4, 3 }, weights);
		weights = new List<float[,]>();
		weights.Add(new float[,] { { 0,0,0,0,0,0,1,0,0,0,0,0,1.0f}, { 0,0,0,0,0,0,0,0,-1,0,0,0,1.0f},
								   { 0,0,-0.3f,-0.0f,0,0,0,0,0,1,0,0,0.5f}, { -0.3f,0,0,0,0,0.0f,0,0,0,0,0,-1,0.5f}});
		weights.Add(new float[,] { { 0,0.6f,0,1f,0},
								   { 0,0,0,0,0},
								   { 0.6f,0,1f,0,0}});
		NewNet("OnEdge", new int[] { 3 * 4, 4, 3 }, weights);

		ChangeNet(true);

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) { Restart(); SetMode(0); }
		if (Input.GetMouseButton(1))
		{
			cameraDistance -= Input.mouseScrollDelta.y;
			cameraRotation += new Vector3(-Input.mousePosition.y + lastMouse.y, Input.mousePosition.x - lastMouse.x, 0);
			Camera.main.transform.position = Vector3.zero;
			Camera.main.transform.eulerAngles = cameraRotation;
			Camera.main.transform.position = -Camera.main.transform.forward * cameraDistance + Vector3.up * 3;
		}
		lastMouse = Input.mousePosition;
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
			case EvaluationMode.Fuzzy:

				break;
				/*		case EvaluationMode.TrainNeural:
							OutputData neuralOutputExp = new OutputData(Vector3ToFloat(diskCtrl.GetAngularVelocity()));
							if (diskCtrl.GetAngularVelocity() != Vector3.zero)
								Neural.Train(PrepareData(), neuralOutputExp);
							break;*/
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
		//	ballCtrl.Randomize();

		}
		if (newmode == 3)
		{
			//	manualDiskCtrl.SetManualControl(true);
			//ballCtrl.Randomize();
		}
	}
	public void NewSimulation()
	{
		if(mode>0) ballCtrl.Randomize();
	}
	public void BiasUpdate(int i)
	{
		selectedBias = i;
		Neural.SetWeight(0, selectedBias, Neural.GetNetwork().firstLayerSize, biasSliders[selectedBias].value);
	}

	/*	public void Pause()
		{
			pause = !pause;
		}*/

	public void NewNet(string str, int[] s, List<float[,]> w)
	{
		netNames.Add(str);
		netsSizes.Add(s);
		netsWeights.Add(w);
		maxNet++;
	}

	public void ChangeNet(bool next)
	{
		selectedNet += next ? 1 : -1;
		if (selectedNet < 0) selectedNet = maxNet;
		if (selectedNet > maxNet) selectedNet = 0;

		Neural.CreateNetwork(netsSizes[selectedNet], netsWeights[selectedNet]);
		nn = Neural.GetNetwork();

		netName.text = "Network Name\n" + netNames[selectedNet];
		for (int i = 0; i < 4; i++)
		{
			biasSliders[i].value = nn.GetLayers()[0].GetNodes()[i].bias;
		}
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
		ball.GetComponent<TrailRenderer>().Clear();

	}

	void SimpleAI(InputData input)
	{
		diskCtrl.SetAngularVelocity(new Vector3(-input.inputBallPositionVector[2] / 10 - input.inputBallSpeedVector[2] / 2 + input.inputDiskRotationSpeedVector[2] / 4, 0,
												input.inputBallPositionVector[0] / 10 + input.inputBallSpeedVector[0] / 2 - input.inputDiskRotationSpeedVector[0] / 4));
	}

	Material lineMaterial;
	void CreateLineMaterial()
	{
		if (!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}

	public void OnRenderObject()
	{
		lineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix);
		GL.MultMatrix(Matrix4x4.Rotate(Quaternion.Euler(Vector3.right * 180)) * Matrix4x4.Translate(Vector3.back * 15));

		if (nn != null)
		{
			int x = 0, y = 0, y1, lastYMax;
			for (int i = 0; i < nn.firstLayerSize; i++)
			{
				GL.Begin(GL.TRIANGLES);
				GL.Color(new Color(0.5f, 1, 1, 0.5f));
				GL.Vertex3(x * 6 - 6 - 0.2f, 0.2f + y - nn.firstLayerSize / 2, 7);
				GL.Vertex3(x * 6 - 6, y - nn.firstLayerSize / 2, 7);
				GL.Vertex3(x * 6 - 6 - 0.2f, -0.2f + y - nn.firstLayerSize / 2, 7);
				GL.End();
				y++;
			}
			lastYMax = nn.firstLayerSize;
			x++;
			foreach (NeuralLayer l in nn.GetLayers())
			{
				y = 0;
				foreach (NeuralNode n in l.GetNodes())
				{
					GL.Begin(GL.TRIANGLES);
					GL.Color(new Color(0.5f, 1, 1, 0.5f));
					GL.Vertex3(x * 6 - 6 - 0.2f, 0.2f + y - l.GetNodes().Length / 2, 7);
					GL.Vertex3(x * 6 - 6, y - l.GetNodes().Length / 2, 7);
					GL.Vertex3(x * 6 - 6 - 0.2f, -0.2f + y - l.GetNodes().Length / 2, 7);
					GL.End();
					y1 = 0;
					foreach (float f in n.GetWeights())
					{
						GL.Begin(GL.LINES);
						if (f > 1)
							GL.Color(new Color(1 - f / 50, 1 - f / 50, 1, 1));
						else if (f < 0)
							GL.Color(new Color(1 + f / 50, 0, 0, 1));
						else
							GL.Color(new Color(1, 1, 1, f));
						GL.Vertex3(x * 6 - 6, y - l.GetNodes().Length / 2, 7);
						GL.Vertex3((x - 1) * 6 - 6, y1 - lastYMax / 2, 7);
						GL.End();
						y1++;
					}
					GL.Begin(GL.LINES);
					if (n.bias >= 0)
						GL.Color(new Color(1, 1, 1, n.bias));
					else if (n.bias < 0)
						GL.Color(new Color(-n.bias, 0, 0, 1));
					GL.Vertex3(x * 6 - 6, y - l.GetNodes().Length / 2, 7);
					GL.Vertex3((x - 1) * 6 - 6, y1 - lastYMax / 2, 7);
					GL.End();
					y++;
				}
				x++;
				lastYMax = l.GetNodes().Length;
			}
		}

		GL.PopMatrix();
	}
}
