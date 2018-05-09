using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelligence
{
	public static class Data
	{
	}

	public struct InputData
	{
		public float[] inputDiskRotationVector;
		public float[] inputDiskRotationSpeedVector;
		public float[] inputBallPositionVector;
		public float[] inputBallSpeedVector;
		public InputData(float[] Dr, float[] Ds, float[] Bp, float[] Bs)
		{
			inputDiskRotationVector = Dr;
			inputDiskRotationSpeedVector = Ds;
			inputBallPositionVector = Bp;
			inputBallSpeedVector = Bs;
		}
		public float[] Collapse()
		{
			List<float> list = new List<float>(inputDiskRotationVector);
			list.AddRange(inputDiskRotationSpeedVector);
			list.AddRange(inputBallPositionVector);
			list.AddRange(inputBallSpeedVector);
			return list.ToArray();
		}
	}
	public struct OutputData
	{
		public float[] outputDiskRotationSpeedVector;
		public OutputData(float[] rotSpeed)
		{
			outputDiskRotationSpeedVector = rotSpeed;
		}
	}
}
