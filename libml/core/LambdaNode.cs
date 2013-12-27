using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class LambdaNode : IAtom
	{
		public LambdaNode(IMLNode lambdaListNode, IMLNode lambdaBody)
		{			
			ExecutionContext = CreateLambdaExecutionContext(lambdaListNode, lambdaBody);
			Text = "#FUNCTION(" + string.Join(",", ExecutionContext.ParameterNames) + "):"
				+ SequenceFormatter.AsString(lambdaBody);
		}

		public static FunctionExecutionContext CreateLambdaExecutionContext(IMLNode lambdaListNode, IMLNode lambdaBody)
		{
			if (lambdaListNode.IsAtom && !lambdaListNode.IsNIL)
			{
				var errorText = ((IAtom)lambdaListNode).Text;
				throw new ArgumentException("lambda list should be a list, not: " + errorText);
			}

			var lambdaList = BNodeWalker.Walk(lambdaListNode);
			var paramNames = new List<string>();
			foreach (var item in lambdaList)
			{
				if (item.Left.NodeType == NodeTypes.Symbol)
				{
					paramNames.Add(SymbolStorage.NormalizeName(((IAtom)item.Left).Text));
				}
				else
				{
					throw new ArgumentException("Argument not a symbol: " 
						+ SequenceFormatter.AsString(item.Left));
				}
			}

			var context = FunctionExecutionContext.CreateMLDefinition(paramNames, lambdaBody);
			return context;
		}

		public NodeTypes NodeType { get { return NodeTypes.Lambda; } }
		public bool IsAtom { get { return true; } }
		public bool IsNIL { get { return false; } }

		public string Text { get; private set; }
		public FunctionExecutionContext ExecutionContext { get; private set; }

#if DEBUG
		public override string ToString()
		{
			return Text;
		}
#endif
	}
}
