using System;
using System.Collections.Generic;
using System.Text;

namespace FuzzyLib
{
    public class InputContext
    {
        private static readonly Tuple<float, float> ballSpeed = new Tuple<float, float>(-50, 50);
        private static readonly Tuple<float, float> paddleSpeed = new Tuple<float, float>(-50, 50);
        private static readonly Tuple<float, float> paddleRotationRange = new Tuple<float, float>(-180, 180);
        private static readonly Tuple<float, float> ballPositionRange = new Tuple<float, float>(-5,5);
        
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

        private float[] convertVector(Tuple<float, float> range, float[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] += range.Item1;
                vector[i] /= 2*range.Item1 + range.Item2;
            }

            return vector;
        }
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
