using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class XListNode : IBListNode
	{
		public NodeTypes NodeType { get { return NodeTypes.List; } }
		public bool IsAtom { get { return false; } }
		public bool IsNIL { get { return false; } }

		public IBNode Left { get; internal set; }
		public IBNode Right { get; internal set; }

		public override string ToString()
		{
			var sb = new StringBuilder("< ");
			PrintBranch(sb, Left);
			sb.Append(" || ");
			PrintBranch(sb, Right);
			sb.Append(" >");
			return sb.ToString();
		}

		private void PrintBranch(StringBuilder sb, IBNode branch)
		{
			if (branch.IsAtom)
			{
				sb.Append(branch.ToString());
			}
			else
			{
				sb.Append("list");
			}
		}
	}
}
