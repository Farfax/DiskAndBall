using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelligence
{


	public static class Neural
	{
		static NeuralNetwork activeNetwork;

		public static Random r = new Random(DateTime.Now.Millisecond);
		public static float learnRate = 0.01f;

		public static void CreateNetwork(int[] size, List<float[,]> weights)
		{
			activeNetwork = new NeuralNetwork(size, weights);

		}
		/*		public static void CreateNetwork(int[] size)
				{
					activeNetwork = new NeuralNetwork(size);

				}*/
		public static void Randomize()
		{
			foreach (NeuralLayer l in activeNetwork.GetLayers())
			{
				foreach (NeuralNode n in l.nodes)
					n.Randomize();
			}
		}

		public static NeuralNetwork GetNetwork()
		{
			return activeNetwork;
		}

		public static void SetWeight(int layer, int node, int node2, float newWeight)
		{
			if (node2 == activeNetwork.GetLayers()[layer].GetNodes()[node].weight.Length)
				activeNetwork.GetLayers()[layer].GetNodes()[node].bias = newWeight;

			else
				activeNetwork.GetLayers()[layer].GetNodes()[node].weight[node2] = newWeight;
		}

		public static void Train(InputData inputData, OutputData expectedOutputData)
		{
			List<float> list = new List<float>();
			list.AddRange(inputData.inputDiskRotationVector);
			list.AddRange(inputData.inputDiskRotationSpeedVector);
			list.AddRange(inputData.inputBallPositionVector);
			list.AddRange(inputData.inputBallSpeedVector);
			activeNetwork.Train(list.ToArray(), expectedOutputData.outputDiskRotationSpeedVector);
		}

		public static OutputData Evaluate(InputData inputData)
		{
			List<float> list = new List<float>();
			list.AddRange(inputData.inputDiskRotationVector);
			list.AddRange(inputData.inputDiskRotationSpeedVector);
			list.AddRange(inputData.inputBallPositionVector);
			list.AddRange(inputData.inputBallSpeedVector);

			/*	float[] output = activeNetwork.Evaluate(list.ToArray());
				int max = 0;
				for(int i = 1; i < output.Length; i++)
				{
					if (output[i] > output[max]) max = i;
				}
				float[] valueSet = new float[] { -3, 1, 0, 1, 3 };*/


			return new OutputData(activeNetwork.Evaluate(list.ToArray()));
		}

		public static float Sigmoid(float x)
		{
			return 2 / (1 + (float)Math.Exp(-2 * x)) - 1;
		}

		public static float TanhDer(float x)
		{
			float tan = (float)Math.Tanh(x);
			return 1 - tan * tan;
		}
	}

	public class NeuralNetwork
	{
		NeuralLayer[] layers;
		public int firstLayerSize;
		int timesTrained;
		public NeuralNetwork(int[] layerSizes, List<float[,]> weights)
		{
			layers = new NeuralLayer[layerSizes.Length - 1];
			firstLayerSize = layerSizes[0];
			for (int i = 1; i < layerSizes.Length; i++)
			{
				layers[i - 1] = new NeuralLayer(layerSizes[i], layerSizes[i - 1]);
				layers[i - 1].UpdateNodes(weights[i - 1]);
			}
			timesTrained = 1;
		}
		/*	public NeuralNetwork(int[] layerSizes)
			{
				layers = new NeuralLayer[layerSizes.Length - 1];
				firstLayerSize = layerSizes[0];
				for (int i = 1; i < layerSizes.Length; i++)
				{
					layers[i - 1] = new NeuralLayer(layerSizes[i], layerSizes[i - 1]);
				}
				timesTrained = 1;
			}*/
		public NeuralLayer[] GetLayers()
		{
			return layers;
		}
		public void Train(float[] inputData, float[] expectedOutputData)
		{
			//	for (int o = 0; o < 30; o++)
			{
				timesTrained++;
				List<float[]> forwardfeed = new List<float[]>(),
					rawForwardFeed = new List<float[]>();
				List<float[,]> newWeights = new List<float[,]>();
				float[] lastOutput = inputData;
				for (int i = 0; i < layers.Length; i++)
				{
					lastOutput = layers[i].Evaluate(lastOutput);
					forwardfeed.Add(lastOutput);
				}
				rawForwardFeed.Add(layers[0].EvaluateRaw(inputData));
				for (int i = 1; i < layers.Length; i++)
				{
					rawForwardFeed.Add(layers[i].EvaluateRaw(forwardfeed[i - 1]));
				}
				int currentLayer = layers.Length - 1, lastLayer = layers.Length - 1;
				//Last layer
				float[,] newW = new float[layers[currentLayer].nodes.Length, layers[currentLayer].nodes[0].weight.Length];
				for (int i = 0; i < layers[currentLayer].nodes.Length; i++)
				{
					NeuralNode node = layers[currentLayer].nodes[i];
					for (int j = 0; j < node.weight.Length; j++)
					{
						float newWeight = DeriativeOfCrossEntropy(expectedOutputData[i], forwardfeed[currentLayer][i])
							* Neural.TanhDer(rawForwardFeed[currentLayer][i])
							* forwardfeed[currentLayer - 1][j];
						newW[i, j] = node.weight[j] - Neural.learnRate * newWeight;
					}
				}
				newWeights.Insert(0, newW);
				//Rest
				currentLayer--;
				newW = new float[layers[currentLayer].nodes.Length, layers[currentLayer].nodes[0].weight.Length];
				//	List<float> errors = new List<float>();
				for (int i = 0; i < layers[currentLayer].nodes.Length; i++)
				{
					NeuralNode node = layers[currentLayer].nodes[i];
					for (int j = 0; j < node.weight.Length; j++)
					{
						float totalError = 0;
						for (int k = 0; k < layers[layers.Length - 1].nodes.Length; k++)
						{
							totalError += DeriativeOfCrossEntropy(expectedOutputData[k], forwardfeed[lastLayer][k])
								* Neural.TanhDer(rawForwardFeed[lastLayer][k])
								* layers[lastLayer].nodes[k].weight[i];
						}
						//		errors.Add(totalError);
						float newWeight = totalError
							* Neural.TanhDer(rawForwardFeed[currentLayer][i])
							* inputData[j]; //forwardfeed[currentLayer-1][j];
						newW[i, j] = node.weight[j] - Neural.learnRate * newWeight;
					}
				}
				newWeights.Insert(0, newW);
				/*	currentLayer--;
					newW = new float[layers[currentLayer].nodes.Length, layers[currentLayer].nodes[0].weight.Length];
					for (int i = 0; i < layers[currentLayer].nodes.Length; i++)
					{
						NeuralNode node = layers[currentLayer].nodes[i];
						for (int j = 0; j < node.weight.Length; j++)
						{
							float totalError = 0;
							for (int k = 0; k < layers[layers.Length - 1].nodes.Length; k++)
							{
								totalError += errors[k]
									* Neural.TanhDer(rawForwardFeed[currentLayer+1][k])
									* layers[currentLayer+1].nodes[k].weight[i];
							}
							float newWeight = totalError
								* Neural.TanhDer(rawForwardFeed[currentLayer][i])
								* inputData[j];
							newW[i, j] = node.weight[j]-Neural.learnRate * newWeight;
						}
					}
					newWeights.Insert(0, newW);*/

				for (int i = 0; i < newWeights.Count; i++)
				{
					newW = newWeights[i];
					layers[i].UpdateNodes(newW);
				}
			}
		}
		public float DeriativeOfCrossEntropy(float y, float nodeOut)
		{
			return (nodeOut - y);
			//	return -(y * (1 / nodeOut) + (1 - y) / (1 - nodeOut));
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
		public NeuralNode[] nodes;

		public NeuralLayer(int size, int previousSize)
		{
			nodes = new NeuralNode[size];
			for (int i = 0; i < size; i++)
			{
				nodes[i] = new NeuralNode(previousSize);
				//Testing
			}
		}
		public NeuralNode[] GetNodes()
		{
			return nodes;
		}
		public void UpdateNodes(float[,] weights)
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				nodes[i].UpdateWeights(weights, i);
				nodes[i].UpdateBias(weights[i, weights.GetLength(1) - 1]);
			}
		}
		public float[] EvaluateRaw(float[] inputData)
		{
			float[] output = new float[nodes.Length];
			int counter = 0;
			foreach (NeuralNode n in nodes)
			{
				output[counter] = n.EvaluateRaw(inputData);
				counter++;
			}
			return output;
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
		public float[] weight;
		public float bias;
		public NeuralNode(int size)
		{
			weight = new float[size];
		}
		public float[] GetWeights()
		{
			return weight;
		}
		public void UpdateWeights(float[,] newW, int row)
		{
			for (int i = 0; i < weight.Length; i++)
			{
				weight[i] = newW[row, i];
			}
		}
		public void UpdateBias(float b)
		{
			bias = b;
		}
		public void Randomize()
		{

			for (int i = 0; i < weight.Length; i++)
			{
				weight[i] = (float)Neural.r.NextDouble() / 2;
			}
		}
		public float EvaluateRaw(float[] inputData)
		{
			if (inputData.Length != weight.Length) throw new Exception();
			float output = 0;
			for (int i = 0; i < weight.Length; i++)
			{
				output += inputData[i] * weight[i];
			}
			output += bias;
			return output;
		}
		public float Evaluate(float[] inputData)
		{
			return EvaluateRaw(inputData);
			return (float)Math.Tanh(EvaluateRaw(inputData));
		}


	}
}
