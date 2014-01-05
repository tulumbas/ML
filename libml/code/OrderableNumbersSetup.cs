using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using ml.core;
using ml.core.numbers;

namespace ml.code
{
	[FunctionSets(SetupMethod = "SetupComparisons")]
	class OrderableNumbersSetup
	{
		public static void SetupComparisons(ISymbolStorage storage, INodeFactory builder)
		{
			var op = new Dictionary<string, Func<OrderableNumber, OrderableNumber, bool>>();
			//op.Add("=", (x, y) => x == y);
			//op.Add("!=", (x, y) => x != y);
			op.Add("<", (x, y) => y.IsGreater(x));
			op.Add("<=", (x, y) => y.IsGreater(x) || x == y);
			op.Add(">", (x, y) => x.IsGreater(y));
			op.Add(">=", (x, y) => x.IsGreater(y) || x == y);

			var mInfo = typeof(OrderableNumbersSetup).GetMethod("CompareNumbers", BindingFlags.Static | BindingFlags.NonPublic);
			foreach (var item in op)
			{
				var fe = FunctionExecutionContext.CreateCompiled(GetDelegate(mInfo, item.Value));
				fe.MinimalNumberOfArguments = 2;
				storage.AddFunction(item.Key, fe);
			}

		}

		private static Func<IListNode, IEvaluator, IMLNode> GetDelegate(
			MethodInfo basicFunc,
			Func<OrderableNumber, OrderableNumber, bool> predicate)
		{			
			var args = Expression.Parameter(typeof(IListNode), "args");
			var code = Expression.Parameter(typeof(IEvaluator), "code");			
			var test = Expression.Constant(predicate);
			var exp = Expression.Lambda<Func<IListNode, IEvaluator, IMLNode>>(
				Expression.Call(basicFunc, args, code, test),
				args, code);
			return exp.Compile();
		}

		private static IMLNode CompareNumbers(IListNode args, IEvaluator code,
			Func<OrderableNumber, OrderableNumber, bool> predicate)
		{
			var arg1 = OrderableNumber.Check4OrderableNumber(args.Left);
			foreach (var node in BNodeWalker.Walk(args).Skip(1))
			{
				var arg = OrderableNumber.Check4OrderableNumber(node.Left);
				if (!predicate(arg1, arg))
				{
					return code.NIL;
				}
			}
			return code.Builder.GetT();
		}

		public static void SetupComparisonsUsingExpressions(ISymbolStorage storage, INodeFactory builder)
		{
			var convertInfo = typeof(OrderableNumber).GetMethod("Check4OrderableNumber", BindingFlags.Public | BindingFlags.Static);

			// parameters
			var args = Expression.Parameter(typeof(IListNode), "args");
			var code = Expression.Parameter(typeof(IEvaluator), "code");
			var result = Expression.Parameter(typeof(IMLNode), "result");

			// local vars
			var nil = Expression.Constant(builder.GetNIL());
			var t = Expression.Constant(builder.GetT());

			var node = Expression.Variable(typeof(IMLNode), "node");
			var numL = Expression.Variable(typeof(OrderableNumber), "numL");
			var numR = Expression.Variable(typeof(OrderableNumber), "numR");
			var stepForward = Expression.Assign(node,
				Expression.Property(Expression.Convert(node, typeof(IListNode)), "Right"));
			var getNumber = Expression.Call(convertInfo,
				Expression.Property(Expression.Convert(node, typeof(IListNode)), "Left"));

			var label = Expression.Label();

			var loop = Expression.Loop(
				Expression.IfThenElse(
					Expression.Property(node, "IsNil"),
					Expression.Block(
						Expression.Assign(result, t),
						Expression.Break(label)),
					Expression.Block(
						Expression.Assign(numR, getNumber),
						Expression.IfThen(
							Expression.GreaterThanOrEqual(numL, numR),
							Expression.Block(
								Expression.Assign(result, nil),
								Expression.Break(label)
							)),
						Expression.Assign(numL, numR),
						stepForward)),
				label);

			var mainBody =
				Expression.Block(
					new[] { result, node, numL, numR },
					Expression.Assign(result, args),
					Expression.Assign(node, args),
					Expression.Assign(numL, getNumber),
					stepForward,
					loop,					
					result
					);

			var fe = FunctionExecutionContext.CreateCompiled(Expression.Lambda<Func<IListNode, IEvaluator, IMLNode>>(mainBody, args, code).Compile());
			fe.MinimalNumberOfArguments = 2;

			storage.AddFunction("<", fe);
		}
	}
}
