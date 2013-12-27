using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	[FunctionSets]
	class LambdaStuff
	{		
		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.All)]
		static IBNode Lambda(IBListNode args, IEvaluator environment)
		{
			if(args == null)
			{
				throw new ArgumentException("LAMBDA can't be called with no args");
			}

			if(args.Left.NodeType != NodeTypes.List && args.Left.NodeType != NodeTypes.NIL)
			{
				var errorText = ((IBAtom) args.Left).Text;
				throw new ArgumentException("LAMDA list should be a list, not: " + errorText);
			}

			var lambdaList = BNodeWalker.Walk(args.Left);
			var paramNames = new List<string>();
			foreach (var item in lambdaList)
			{
				if (item.Left.NodeType == NodeTypes.Symbol)
				{
					paramNames.Add(((IBAtom)item.Left).Text);
				}
				else
				{
					throw new ArgumentException("Argument not a symbol: " + item.Left.ToString());
				}
			}

			//var expr = System.Linq.Expressions.Expression<Action>.Lambda<Action>(

			var context = new FunctionExecutionContext
			{
				NumberOfArguments = paramNames.Count,
			};

			return new LambdaNode(paramNames, args.Right, context);							
		}
	}
}
