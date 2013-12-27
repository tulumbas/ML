using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class NodeFactory : INodeFactory
	{
		static IAtom NIL, T;

		static NodeFactory()
		{
			NIL = new BasicAtom("NIL", NodeTypes.NIL);
			T = new BasicAtom("T", NodeTypes.T);
		}

		public IAtom CreateAtom(string value, NodeTypes atomType)
		{
			switch (atomType)
			{
				case NodeTypes.List:
					throw new ArgumentException("Can't create atom with nodeType LIST");

				case NodeTypes.NIL: return NIL;
				case NodeTypes.T: return T;

				case NodeTypes.Symbol:
				case NodeTypes.TextLiteral:
					SymbolStorage.Symbols.AddSymbol(value);
					return new BasicAtom(value, atomType);

				case NodeTypes.DecimalNumber:
				case NodeTypes.IntegerNumber:
					return new BasicAtom(value, atomType);

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

		public IListNode Merge(IListNode listA, IListNode listB)
		{
			(listA as XListNode).Right = listB;
			return listA;
		}

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
