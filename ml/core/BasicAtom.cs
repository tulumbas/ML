using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class BasicAtom : IBAtom
	{
		internal BasicAtom(string representation, NodeTypes atomType)
		{
			Text = representation;
			NodeType = atomType;
		}

		public string Text { get; private set; }
		public NodeTypes NodeType { get; private set; }
		public bool IsAtom { get { return true; } }
		public bool IsNIL { get { return NodeType == NodeTypes.NIL; } }

		public override string ToString()
		{
			return Text;
		}
	}

	class LambdaNode : IBAtom
	{
		public LambdaNode(IEnumerable<string> parameterNames, IBNode body, FunctionExecutionContext context)
		{
			Body = body;
			ParameterNames = parameterNames.ToArray();
			ExecutionContext = context;
			Text = "#FUNCTION(" + string.Join(",", parameterNames) + "):" + BuildName(body);
		}

		public NodeTypes NodeType { get { return NodeTypes.Lambda; } }
		public bool IsAtom { get { return true; } }
		public bool IsNIL { get { return false; } }

		public string Text { get; private set; }
		public IBNode Body { get; private set; }
		public string[] ParameterNames { get; private set; }
		public FunctionExecutionContext ExecutionContext { get; private set; }
		
		public override string ToString()
		{
			return Text;
		}

		public Dictionary<string, IBNode> CreateLinks(IBNode args)
		{
			var links = new Dictionary<string, IBNode>();
			if (ParameterNames.Length > 0)
			{
				if (args == null)
				{
					throw new InvalidOperationException("Formal parameters do not correspond to actual");
				}
				var actual = BNodeWalker.Walk(args).ToArray();
				if (actual.Length != ParameterNames.Length)
				{
					throw new InvalidOperationException("Formal parameters do not correspond to actual");
				}

				for (int i = 0; i < actual.Length; i++)
				{
					links.Add(ParameterNames[i], actual[i].Left);
				}
			}
			else if(args != null)
			{
				throw new InvalidOperationException("Formal parameters do not correspond to actual");
			}
			return links;
		}

		private string BuildName(IBNode node)
		{
			if (node.IsAtom)
			{
				return (node as IBAtom).Text;
			}
			else
			{
				var sb = new StringBuilder("(");
				var first = true;
				while (!node.IsNIL)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						sb.Append(" ");
					}

					var list = (node as IBListNode);
					sb.Append(BuildName(list.Left));
					node = list.Right;
				}
				sb.Append(")");
				return sb.ToString();
			}
		}

	}

}
