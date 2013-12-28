using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class NodeFactory : INodeFactory
	{
		static IAtom NIL, T;
		private ISymbolStorage scope;

		static NodeFactory()
		{
			NIL = new BasicAtom("NIL", NodeTypes.NIL);
			T = new BasicAtom("T", NodeTypes.T);
		}

		public NodeFactory(ISymbolStorage symbols)
		{
			scope = symbols;
		}

		public IAtom CreateAtom(string value, NodeTypes atomType)
		{
			switch (atomType)
			{
				case NodeTypes.Symbol:
				case NodeTypes.TextLiteral:
					scope.AddSymbol(value);
					return new BasicAtom(value, atomType);

				case NodeTypes.Number:
					return new BasicAtom(value, value.Contains('.') ? 
						NodeTypes.DecimalNumber : NodeTypes.IntegerNumber);

				default:
					throw new ArgumentException("Can't create atom with nodeType: " + atomType.ToString());
			}
		}

		public IAtom GetT()
		{
			return T;
		}

		public IAtom GetNIL()
		{
			return NIL;
		}

		//public IListNode Merge(IListNode listA, IListNode listB)
		//{
		//   (listA as XListNode).Right = listB;
		//   return listA;
		//}

		public IListBuilder CreateListBuilder()
		{
			return new XListBuilder(GetNIL());
		}

		public IListBuilder CreateListBuilder(IMLNode firstItemValue)
		{
			var builder = new XListBuilder(GetNIL());
			builder.Append(firstItemValue);
			return builder;
		}
	}

}
