using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelligence
{
	public static class Neural
	{


		public static OutputData Evaluate(InputData inputData)
		{
			//Evaluation

			return new OutputData(0, 0, 0);
		}
	}

	public class NeuralNetwork
	{
		NeuralLayer[] layers;
	}
	public class NeuralLayer
	{
		NeuralNode[] nodes;
	}
	public class NeuralNode
	{
		double[] weight;
	}
}
