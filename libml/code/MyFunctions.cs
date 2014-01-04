using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	[FunctionSets]
	class MyFunctions
	{
		[BNodeFunc(NumberOfArguments = 1, Alias = "is-defined-value")]
		static IMLNode IsDefinedValue(IListNode args, IEvaluator code)
		{
			var symbol = args.Left;
			switch (symbol.NodeType)
			{
				case NodeTypes.Symbol:
				case NodeTypes.TextLiteral:
					var name = ((IAtom)symbol).Text;
					var sc = code.Symbols[name];
					return sc != null && sc.Value != null ? code.Builder.GetT() : code.NIL;

				case NodeTypes.NIL:
				case NodeTypes.T:
					return code.Builder.GetT();

				default:
					return code.NIL;
			}
		}

		[BNodeFunc(NumberOfArguments = 1)]
		static IMLNode Reverse(IListNode args, IEvaluator code)
		{
			if (args.Left.IsNIL)
			{
				return code.NIL;
			}
			var sourceList = BasicFunctions.Check4List(args.Left, "Reverse");
			var elements = BNodeWalker.Walk(sourceList).ToArray();
			var result = code.Builder.CreateListBuilder();
			for (int i = elements.Length - 1; i >= 0; i--)
			{
				result.Append(elements[i].Left);
			}
			return result.GetList();
		}
	}
}
