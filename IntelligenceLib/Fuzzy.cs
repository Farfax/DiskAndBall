using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelligence
{
	public static class FuzzyLogic
	{
		public static Fuzzy fX = new Fuzzy(), fZ = new Fuzzy();

		public static OutputData Evaluate(InputData inputData)
		{
			return new OutputData(new float[] { fX.Evaluate(inputData), 0, fZ.Evaluate(inputData) });
		}
		public static float AND(float a, float b)
		{
			return Math.Min(a, b);
		}
		public static float OR(float a, float b)
		{
			return Math.Max(a, b);
		}
		public static float NOT(float a)
		{
			return 1 - a;
		}
		public static void AddRule(Rule r, bool z)//z or x output
		{
			if (z) fZ.AddRule(r);
			else fX.AddRule(r);
		}
	}

	public class Fuzzy
	{

		List<Rule> rules;

		public Fuzzy()
		{
			rules = new List<Rule>();
		}

		public void AddRule(Rule r)
		{
			rules.Add(r);
		}

		public float Evaluate(InputData inputData)
		{
			float[] data = inputData.Collapse();
			float top=0, bottom=0;
			foreach(Rule r in rules)
			{
				float top2 = 0,bottom2 = 0;
				for(int i = 0; i < data.Length; i++)
				{
					if(r.usedFunctions[i]!= 0)
					{
						top2 += MembershipFunctions.functionCollection[(int)r.usedFunctions[i]](data[i])* Math.Abs(data[i]);
						bottom2 +=MembershipFunctions.functionCollection[(int)r.usedFunctions[i]](data[i]);
					}
				}
				top += top2 * r.weight;
				bottom += bottom2 ;
			}
			if (bottom == 0) return 0;
			return top/bottom;
		}
	}
	public struct Rule
	{
		public int[] usedFunctions;//for inputs, ordered;
		public float weight;
	}

	public enum MemberFunctionName
	{
		paddleRotationNegativeBig=0,
		paddleRotationNegativeSmall=1,
		paddleRotationZero=2,
		paddleRotationPositiveSmall=3,
		paddleRotationPositiveBig=4,

		paddleAngularNegativeBig=5,
		paddleAngularNegativeSmall=6,
		paddleAngularZero=7,
		paddleAngularPositiveSmall=8,
		paddleAngularPositiveBig=9,

		ballPositionNegativeBig=10,
		ballPositionNegativeSmall=11,
		ballPositionZero=12,
		ballPositionPositiveSmall=13,
		ballPositionPositiveBig=14,

		ballSpeedNegativeBig=15,
		ballSpeedNegativeSmall=16,
		ballSpeedZero=17,
		ballSpeedPositiveSmall=18,
		ballSpeedPositiveBig=19,
		none=20
	}

	public static class MembershipFunctions
	{
		public static Func<float, float>[] functionCollection = new Func<float, float>[] { PaddleRotationNegativeBig, PaddleRotationNegativeSmall, PaddleRotationZero, PaddleRotationPositiveSmall, PaddleRotationPositiveBig,
		PaddleAngularNegativeBig, PaddleAngularNegativeSmall, PaddleAngularZero, PaddleAngularPositiveSmall, PaddleAngularPositiveBig,
		BallPositionNegativeBig,BallPositionNegativeSmall,BallPositionZero,BallPositionPositiveSmall,BallPositionPositiveBig,
		BallVelocityNegativeBig,BallVelocityNegativeSmall,BallVelocityZero,BallVelocityPositiveSmall,BallVelocityPositiveBig};

		public static float zoClamp(float f)
		{
			return Math.Min(1, Math.Max(0, f));
		}
		public static float PaddleRotationNegativeBig(float input)
		{
			return PaddleRotationPositiveBig(-input);
		}
		public static float PaddleRotationNegativeSmall(float input)
		{
			return PaddleRotationPositiveSmall(-input);
		}
		public static float PaddleRotationZero(float input)
		{
			return zoClamp(1 - input * input );
		}
		public static float PaddleRotationPositiveSmall(float input)
		{
			if (input < 0) return 0;
			return FuzzyLogic.NOT(FuzzyLogic.OR(PaddleRotationZero(input), PaddleRotationPositiveBig(input)));
		}
		public static float PaddleRotationPositiveBig(float input)
		{
			if (input > 20) return 1;
			return zoClamp(1 - ((input - 20) * (input - 20)) / 300);
		}


		public static float PaddleAngularNegativeBig(float input)
		{
			return PaddleAngularPositiveBig(-input);
		}
		public static float PaddleAngularNegativeSmall(float input)
		{
			return PaddleAngularPositiveSmall(-input);
		}
		public static float PaddleAngularZero(float input)
		{
			return zoClamp(1 - input * input * 4);
		}
		public static float PaddleAngularPositiveSmall(float input)
		{
			if (input < 0) return 0;
			return FuzzyLogic.NOT(FuzzyLogic.OR(PaddleAngularZero(input), PaddleAngularPositiveBig(input)));
		}
		public static float PaddleAngularPositiveBig(float input)
		{
			if (input > 5) return 1;
			return zoClamp(1 - ((input - 5) * (input - 5)) / 15);
		}


		public static float BallPositionNegativeBig(float input)
		{
			return BallPositionPositiveBig(-input);
		}
		public static float BallPositionNegativeSmall(float input)
		{
			return BallPositionPositiveSmall(-input);
		}
		public static float BallPositionZero(float input)
		{
			return zoClamp(1 - input * input * 7);
		}
		public static float BallPositionPositiveSmall(float input)
		{
			if (input < 0) return 0;
			return FuzzyLogic.NOT(FuzzyLogic.OR(BallPositionZero(input), BallPositionPositiveBig(input)));
		}
		public static float BallPositionPositiveBig(float input)
		{
			if (input > 3) return 1;
			return zoClamp(1 - ((input - 3) * (input - 3)) / 4);
		}


		public static float BallVelocityNegativeBig(float input)
		{
			return BallVelocityPositiveBig(-input);
		}
		public static float BallVelocityNegativeSmall(float input)
		{
			return BallVelocityPositiveSmall(-input);
		}
		public static float BallVelocityZero(float input)
		{
			return zoClamp(1 - input * input * 10);
		}
		public static float BallVelocityPositiveSmall(float input)
		{
			if (input < 0) return 0;
			return FuzzyLogic.NOT(FuzzyLogic.OR(BallVelocityZero(input), BallVelocityPositiveBig(input)));
		}
		public static float BallVelocityPositiveBig(float input)
		{
			if (input > 2) return 1;
			return zoClamp(1 - ((input - 2) * (input - 2)) / 2);
		}
	}
}
