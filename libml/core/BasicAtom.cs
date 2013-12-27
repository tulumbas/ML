using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class BasicAtom : IAtom
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


}
