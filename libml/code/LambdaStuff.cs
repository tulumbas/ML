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
		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.All, MinimalNumberOfArguments = 1)]
		static IMLNode Lambda(IListNode args, IEvaluator environment)
		{
			return new LambdaNode(args.Left, args.Right);
		}

		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.All, MinimalNumberOfArguments = 2)]
		static IMLNode Defun(IListNode args, IEvaluator environment)
		{
			if (args.Left.NodeType != NodeTypes.Symbol)
			{
				throw new ArgumentException("Bad function name for DEFUN: " + SequenceFormatter.AsString(args.Left));
			}

			var funcName = ((IAtom)args.Left).Text;
			var lambdaList = ((IListNode)args.Right).Left;
			var body = ((IListNode)args.Right).Right;

			var context = LambdaNode.CreateLambdaExecutionContext(lambdaList, body);
			environment.Symbols.AddFunction(funcName, context);
			return args.Left;
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IMLNode Apply(IListNode args, IEvaluator code)
		{
			var argList = BasicFunctions.Check4List(args.Right, "APPLY");
			return code.ApplyCall(args.Left, argList.Left, null);
		}

		[BNodeFunc(MinimalNumberOfArguments = 1)]
		static IMLNode Funcall(IListNode args, IEvaluator code)
		{
			return code.ApplyCall(args.Left, args.Right, null);
		}
	}
}