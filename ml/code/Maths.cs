using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	[FunctionSets]
	class Maths
	{
		[BNodeFunc(Alias = "+")]
		static IBNode Add(IBListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "-")]
		static IBNode Minus(IBListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "*")]
		static IBNode Multiply(IBListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "/")]
		static IBNode Divide(IBListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IBNode Mod(IBListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}
	}
}
