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
		static IMLNode Lambda(IListNode args, IEvaluator environment)
		{
			if(args == null)
			{
				throw new ArgumentException("LAMBDA can't be called with no args");
			}

			return new LambdaNode(args.Left, args.Right);							
		}

		[BNodeFunc(ArgumentQuoting=ArgumentQuotingTypes.All)]
		static IMLNode Defun(IListNode args, IEvaluator environment)
		{
			if (args == null)
			{
				throw new ArgumentException("DEFUN can't be called with no args");
			}

			if (args.Left.NodeType != NodeTypes.Symbol)
			{
				throw new ArgumentException("Bad function name for DEFUN: " + SequenceFormatter.AsString(args.Left));
			}

			if (args.Right.NodeType != NodeTypes.List)
			{
				throw new ArgumentException("No lambda list for DEFUN: " + SequenceFormatter.AsString(args.Right));
			}

			var funcName = ((IAtom)args.Left).Text;
			var lambdaList = ((IListNode)args.Right).Left;
			var body = ((IListNode)args.Right).Right;

			var context = LambdaNode.CreateLambdaExecutionContext(lambdaList, body);
			SymbolStorage.Symbols.AddFunction(funcName, context);
			return args.Left;
		}
	}
}
