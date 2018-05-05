using System;
using System.Collections.Generic;
using System.Text;

namespace FuzzyLib
{
    public class InputContext
    {
		//Fuck U Unity
		//   private static readonly Tuple<float, float> ballSpeed = new Tuple<float, float>(-50, 50);
		//  private static readonly Tuple<float, float> paddleSpeed = new Tuple<float, float>(-50, 50);
		//  private static readonly Tuple<float, float> paddleRotationRange = new Tuple<float, float>(-180, 180);
		// private static readonly Tuple<float, float> ballPositionRange = new Tuple<float, float>(-5,5);
		private static readonly float ballSpeed = 50;
		private static readonly float paddleSpeed = 50; 
		private static readonly float paddleRotationRange = 180; 
		private static readonly float ballPositionRange = 5;  

		public float[] ballVelocity { get; }
        public float[] ballPosition { get; }
        public float[] paddleRotation { get; }
        public float[] paddleRotationVelocity { get;}

        public InputContext(float[] ballVelocity = null, float[] ballPosition = null, float[] paddleRotation = null , float[] paddleRotationVelocity = null)
        {
            this.ballVelocity = ballVelocity ?? new float[3];
            this.ballPosition = ballPosition ?? new float[3];
            this.paddleRotation = paddleRotation ?? new float[3];
            this.paddleRotationVelocity = paddleRotationVelocity ?? new float[3];
        }

        private float[] convertVector(float range, float[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
				vector[i] /= range;
            }

            return vector;
        }

		// After conversion we have direct ( - or + ) and 'fuzzy strengh' 0-1 when 1 is max distance from point (0,0).
        public InputContext convertToFuzzy()
        {
            return new InputContext(convertVector(ballSpeed, ballVelocity),convertVector(ballPositionRange, ballPosition), convertVector(paddleRotationRange, paddleRotation), convertVector(paddleSpeed, paddleRotationVelocity));   
        }
    }

    public class Output
    {
        public float[] paddleVelocity { get; set;  }

        public Output(float[] paddleVelocity= null)
        {
            this.paddleVelocity = paddleVelocity ?? new float[3];
        }
    }

    interface AbstractPrinciple
    {
    Output apply(InputContext context, Output output);
    }
}
