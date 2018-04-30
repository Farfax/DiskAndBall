using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelligence
{
	public static class Neural
	{
		static NeuralNetwork activeNetwork;

		public static void CreateNetwork()
		{
			activeNetwork = new NeuralNetwork(new int[] { 3 * 4, 10, 3 });

		}

		public static OutputData Evaluate(InputData inputData)
		{
			List<float> list = new List<float>();
			list.AddRange(inputData.inputDiskRotationVector);
			list.AddRange(inputData.inputDiskRotationSpeedVector);
			list.AddRange(inputData.inputBallPositionVector);
			list.AddRange(inputData.inputBallSpeedVector);
			
			return new OutputData(activeNetwork.Evaluate(list.ToArray()));
		}
	}

	public class NeuralNetwork
	{
		NeuralLayer[] layers;

		public NeuralNetwork(int[] layerSizes)
		{
			layers = new NeuralLayer[layerSizes.Length-1];
			for (int i = 1; i < layerSizes.Length; i++)
			{
				layers[i-1] = new NeuralLayer(layerSizes[i], layerSizes[i - 1]);
			}
		}
		public float[] Evaluate(float[] inputData)
		{
			float[] layerOutputs = inputData;
			foreach (NeuralLayer l in layers)
			{
				layerOutputs = l.Evaluate(layerOutputs);
			}
			return layerOutputs;
		}
	}
	public class NeuralLayer
	{
		NeuralNode[] nodes;

		public NeuralLayer(int size, int previousSize)
		{
			nodes = new NeuralNode[size];
			for (int i = 0; i < size; i++)
			{
				nodes[i] = new NeuralNode(previousSize);
				nodes[i].Randomize();//Testing
			}
		}
		public float[] Evaluate(float[] inputData)
		{
			float[] output = new float[nodes.Length];
			int counter = 0;
			foreach (NeuralNode n in nodes)
			{
				output[counter] = n.Evaluate(inputData);
				counter++;
			}
			return output;
		}
	}
	public class NeuralNode
	{
		float[] weight;

		public NeuralNode(int size)
		{
			weight = new float[size];
		}
		public void Randomize()
		{
			Random r = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < weight.Length; i++)
			{
				weight[i] = ((float)r.NextDouble()*2-1);
			}
		}
		public float Evaluate(float[] inputData)
		{
			if (inputData.Length != weight.Length) throw new Exception();
			float output = 0;
			for (int i = 0; i < weight.Length; i++)
			{
				output += inputData[i] * weight[i];
			}
			return (float)Math.Tanh(output);
		}

		public float Sigmoid(float x)
		{
			return 2 / (1 + (float)Math.Exp(-2 * x)) - 1;
		}
	}
}
