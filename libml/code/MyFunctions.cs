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
	}
}
