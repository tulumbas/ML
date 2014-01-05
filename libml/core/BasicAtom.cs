using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core.numbers;

namespace ml.core
{
	class BasicAtom : IAtom
	{
		readonly NodeTypes nodeType;
		readonly bool isNil;
		
		protected BasicAtom(NodeTypes atomType)
		{
			nodeType = atomType;
			isNil = NodeType == NodeTypes.NIL;
		}

		internal BasicAtom(string representation, NodeTypes atomType)
		{
			Text = representation;
			nodeType = atomType;
			isNil = NodeType == NodeTypes.NIL;
		}

		public string Text { get; protected set; }
		public NodeTypes NodeType { get { return nodeType; } }
		public bool IsNIL { get { return isNil; } }
		public bool IsAtom { get { return true; } }

		//public override string ToString()
		//{
		//   return Text;
		//}
	}

	class RealNumber : BasicAtom
	{
		internal RealNumber(string digitString)
			: base(digitString, NodeTypes.RealNumber)
		{

		}

		internal RealNumber(BigNum bn)
			: base(NodeTypes.RealNumber)
		{
		}
	}

}
