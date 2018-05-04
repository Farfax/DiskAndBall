using System;
using System.Collections.Generic;

namespace FuzzyLib
{
// Use it when your principle is very simple (f.e: return value>0). Just give lambda/ function to the constructor.
    public class ShortPrinciple: AbstractPrinciple
    {
        private Func<InputContext, Output, Output> prince;

        public ShortPrinciple( Func<InputContext,Output, Output> func)
        {
            prince = func;
        }

        public Output apply(InputContext context, Output output)
        {
            return prince(context, output);
        }


    }

    public class DefaultPrinciple : AbstractPrinciple
    {

        public DefaultPrinciple()
        {
          
        }

        // Context is fuzzy value from graphic/physic, output is current response status.
        public Output apply(InputContext context, Output output)
        {
        // here is our Principle logic
            if(context.ballVelocity[0]>0.5)
            {
				;
            }
            if (context.ballPosition[0] == context.ballVelocity[0])
                output.paddleVelocity[0] = 1;

            if (context.ballVelocity[0] == 1)
                output.paddleVelocity[0] = 1;
            return output;
        }


    }

	//Usage, create InputContext object, then create DefaultLogic(context). Now u can get reponse object by use applyPrinciples().
    public class DefaultLogic
    {
        List<AbstractPrinciple> principles;

        InputContext context;

        public DefaultLogic(InputContext context) 
        {
            this.context = context;
            principles = new List<AbstractPrinciple>();
          //  principles.Add(new DefaultPrinciple((con, output) => { re}));
          // here is place for Principles definitions
        }
		public DefaultLogic(float[] ballVelocity, float[] ballPosition, float[] paddleRotation, float[] paddleRotationVelocity)
		{
			this.context = new InputContext(ballVelocity, ballPosition, paddleRotation, paddleRotationVelocity).convertToFuzzy();
			principles = new List<AbstractPrinciple>();
			//  principles.Add(new DefaultPrinciple((con, output) => { re}));
			// here is place for Principles definitions
		}

		public Output applyPrinciples()
        {
            Output output = new Output();

            foreach(var principle in principles)
            {
                output = principle.apply(context, output);
            }

            return output;
        }
    }
}
