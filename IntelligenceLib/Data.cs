using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelligence
{
	public static class Data
	{
	}

	public struct InputData
	{
		float[] inputDiskRotationVector;
		float[] inputDiskRotationSpeedVector;
		float[] inputBallPositionVector;
		float[] inputBallSpeedVector;
		public InputData(float[] Dr, float[] Ds, float[] Bp, float[] Bs)
		{
			inputDiskRotationVector = Dr;
			inputDiskRotationSpeedVector = Ds;
			inputBallPositionVector = Bp;
			inputBallSpeedVector = Bs;
		}
	}
	public struct OutputData
	{
		float[] outputDiskRotationSpeedVector;
		public OutputData(float x, float y, float z)
		{
			outputDiskRotationSpeedVector = new float[3] { x, y, z };
		}
	}
}
