using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	[FunctionSets(SetupMethod = "SetupCadrs")]
	class BasicFunctions
	{
		#region Main functions
		[BNodeFunc(NumberOfArguments = 1)]
		static IMLNode Atom(IListNode args, IEvaluator code)
		{
			return args.Left.IsAtom ? code.Builder.GetT() : code.NIL;
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IMLNode Car(IListNode args, IEvaluator environmnet)
		{
			if (args.Left.IsNIL)
			{
				return environmnet.NIL;
			}
			var list = Check4List(args.Left, "car");
			return list.Left;
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IMLNode Cdr(IListNode args, IEvaluator environmnet)
		{
			if (args.Left.IsNIL)
			{
				return environmnet.NIL;
			}
			var list = Check4List(args.Left, "car");
			return list.Right.IsNIL ? environmnet.NIL as IMLNode : list.Right;
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IMLNode Cons(IListNode args, IEvaluator environmnet)
		{
			var a = args.Left;
			var b = ((args.Right) as IListNode).Left;
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
		static IMLNode Eq(IListNode args, IEvaluator environmnet)
		{
			var argA = args.Left;
			var argB = (args.Right as IListNode).Left;
			if (!argA.IsAtom || !argB.IsAtom)
			{
				return environmnet.NIL;
			}

			if (argA.IsNIL && argB.IsNIL)
			{
				return environmnet.Builder.GetT();
			}

			return (argA as IAtom).Text == (argB as IAtom).Text 
				? environmnet.Builder.GetT() : environmnet.NIL;
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IMLNode Eval(IListNode args, IEvaluator environmnet)
		{
			var arg = args.Left;
			return environmnet.EvalNode(arg, null);
		}

		[BNodeFunc(NumberOfArguments = 1, ArgumentQuoting = ArgumentQuotingTypes.All)]
		static IMLNode Quote(IListNode args, IEvaluator environmnet)
		{
			return args.Left;
		}

		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.All)]
		static IMLNode Cond(IListNode args, IEvaluator environmnet)
		{
			if (args == null)
			{
				return environmnet.NIL;
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
				IMLNode condition = environmnet.EvalNode(pair.Left, null);
				IMLNode body = pair.Right;

				if (!condition.IsNIL)
				{
					if (body.IsNIL)
					{
						return condition;
					}

					var sequence = body as IListNode;
					IMLNode result;
					while (true)
					{
						result = environmnet.EvalNode(sequence.Left, null);
						if (sequence.Right.IsNIL)
						{
							return result;
						}
						sequence = sequence.Right as IListNode;
					}
				}

				if (args.Right.IsNIL)
				{
					break;
				}
				args = args.Right as IListNode;
			}
			return environmnet.NIL;
		}

		[BNodeFunc]
		static IMLNode Progn(IListNode args, IEvaluator environmnet)
		{
			if (args == null)
			{
				return environmnet.NIL;
			}
			var walker = BNodeWalker.Walk(args);
			return walker.Last().Left;
		}

		[BNodeFunc(ArgumentQuoting = ArgumentQuotingTypes.First, NumberOfArguments = 2)]
		static IMLNode SetQ(IListNode args, IEvaluator environmnet)
		{
			var symbol = args.Left;
			if (symbol.NodeType != NodeTypes.Symbol)
			{
				throw new ArgumentException("Not a symbol in SETQ");
			}

			var value = (args.Right as IListNode).Left;
			environmnet.Symbols.SetValue((symbol as IAtom).Text, value);
			return value;
		}

		[BNodeFunc(NumberOfArguments = 1, Alias = "null")]
		static IMLNode IsNullP(IListNode args, IEvaluator environment)
		{
			return args.Left.IsNIL ? environment.Builder.GetT() : environment.NIL;			
		}

		#endregion

		#region CADR

		public static void SetupCadrs(ISymbolStorage storage, INodeFactory builder)
		{
			var lambdaList = new string[] { "x" };

			for (int i = 2; i < 5; i++)
			{
				var variants = 1 << i;
				var namechars = new char[i + 2];
				namechars[0] = 'c';
				namechars[i + 1] = 'r';

				for (int j = 0; j < variants; j++)
				{
					IMLNode arg = builder.CreateAtom("x", NodeTypes.Symbol);
					IListBuilder list;

					for (int k = i - 1; k >= 0; k--)
					{
						var flag = (j & (1 << k)) == 0;
						namechars[k + 1] = flag ? 'a' : 'd';
						var cr = flag ? "car" : "cdr";
						list = builder.CreateListBuilder(builder.CreateAtom(cr, NodeTypes.Symbol));
						list.Append(arg);
						arg = list.GetList();
					}

					list = builder.CreateListBuilder(arg);
					var context = FunctionExecutionContext.CreateMLDefinition(lambdaList, list.GetList());
					storage.AddFunction(new string(namechars), context);
				}
			}
		}

		#endregion

		#region Others
		//[BNodeFunc(NumberOfArguments = 2)]
		//static IMLNode Nth(IListNode args, IEvaluator code)
		//{
			
		//}
		#endregion

		#region helpers
		public static IListNode Check4List(IMLNode source, string funcName)
		{
			if (source.IsAtom)
			{
				throw new ArgumentException("Not a list in " + funcName);
			}
			var list = source as IListNode;
			return list;
		}
		#endregion
	}
}
