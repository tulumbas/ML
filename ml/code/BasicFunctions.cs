using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	[FunctionSets]
	class BasicFunctions
	{
		#region Main functions
		[BNodeFunc(NumberOfArguments = 1)]
		static IBNode Car(IBListNode args, IEvaluator environmnet)
		{
			if (args.Left.IsNIL)
			{
				return environmnet.Builder.GetNIL();
			}
			var list = Check4List(args.Left, "car");
			return list.Left;
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IBNode Cdr(IBListNode args, IEvaluator environmnet)
		{
			if (args.Left.IsNIL)
			{
				return environmnet.Builder.GetNIL();
			}
			var list = Check4List(args.Left, "car");
			return list.Right.IsNIL ? environmnet.Builder.GetNIL() as IBNode : list.Right;
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IBNode Cons(IBListNode args, IEvaluator environmnet)
		{
			var a = args.Left;
			var b = ((args.Right) as IBListNode).Left;
			var listBuilder = environmnet.Builder.CreateListBuilder(a);
			//var list = listBuilder.GetList() as IBListNode;

			if (!b.IsNIL)
			{
				var list2 = Check4List(b, "cons");
				//return Builder.Merge(list, list2);
				listBuilder.Merge(list2);
			}
			return listBuilder.GetList();
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IBNode Eq(IBListNode args, IEvaluator environmnet)
		{
			var argA = args.Left;
			var argB = (args.Right as IBListNode).Left;
			if (!argA.IsAtom || !argB.IsAtom)
			{
				return environmnet.Builder.GetNIL();
			}

			if (argA.IsNIL && argB.IsNIL)
			{
				return environmnet.Builder.GetT();
			}

			return (argA as IBAtom).Text == (argB as IBAtom).Text 
				? environmnet.Builder.GetT() : environmnet.Builder.GetNIL();
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IBNode Eval(IBListNode args, IEvaluator environmnet)
		{
			var arg = args.Left;
			return environmnet.EvalNode(arg, null);
		}

		[BNodeFunc(NumberOfArguments = 1, ArgumentQuoting = ArgumentQuotingTypes.All)]
		static IBNode Quote(IBListNode args, IEvaluator environmnet)
		{
			return args.Left;
		}

		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.All)]
		static IBNode Cond(IBListNode args, IEvaluator environmnet)
		{
			if (args == null)
			{
				return environmnet.Builder.GetNIL();
			}

			while (true)
			{
				// each processed (!) list member should be a non-empty list (listA)
				// first element is a condition
				// if it doesn't evaluates to nil, then the reset of listA is 
				//     processed element-by-element and the last result is returned to cond
				// otherwise, 
				// take the next element of args
				var pair = Check4List(args.Left, "cond");
				IBNode condition = environmnet.EvalNode(pair.Left, null);
				IBNode body = pair.Right;

				if (!condition.IsNIL)
				{
					if (body.IsNIL)
					{
						return condition;
					}

					var sequence = body as IBListNode;
					IBNode result;
					while (true)
					{
						result = environmnet.EvalNode(sequence.Left, null);
						if (sequence.Right.IsNIL)
						{
							return result;
						}
						sequence = sequence.Right as IBListNode;
					}
				}

				if (args.Right.IsNIL)
				{
					break;
				}
				args = args.Right as IBListNode;
			}
			return environmnet.Builder.GetNIL();
		}

		[BNodeFunc]
		static IBNode Progn(IBListNode args, IEvaluator environmnet)
		{
			if (args == null)
			{
				return environmnet.Builder.GetNIL();
			}
			var walker = BNodeWalker.Walk(args);
			return walker.Last().Left;
		}

		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.First, NumberOfArguments = 2)]
		static IBNode SetQ(IBListNode args, IEvaluator environmnet)
		{
			var symbol = args.Left;
			if (symbol.NodeType != NodeTypes.Symbol)
			{
				throw new ArgumentException("Not a symbol in SETQ");
			}

			var value = (args.Right as IBListNode).Left;
			SymbolStorage.Symbols.SetValue((symbol as IBAtom).Text, value);
			return value;
		}

		#endregion

		private static IBListNode Check4List(IBNode source, string funcName)
		{
			if (source.IsAtom)
			{
				throw new ArgumentException("Not a list in " + funcName);
			}
			var list = source as IBListNode;
			return list;
		}

	}
}
