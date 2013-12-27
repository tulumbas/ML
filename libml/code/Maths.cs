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
		static IMLNode Add(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "-")]
		static IMLNode Minus(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "*")]
		static IMLNode Multiply(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(Alias = "/")]
		static IMLNode Divide(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IMLNode Mod(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}
	}
}
