using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class NodeFactory : IBNodeBuilder
	{
		static IBAtom NIL, T;

		static NodeFactory()
		{
			NIL = new BasicAtom("NIL", NodeTypes.NIL);
			T = new BasicAtom("T", NodeTypes.T);
		}

		public IBAtom CreateAtom(string value, NodeTypes atomType)
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

		public IBAtom GetT()
		{
			return T;
		}

		public IBAtom GetNIL()
		{
			return NIL;
		}

		public IBListNode Merge(IBListNode listA, IBListNode listB)
		{
			(listA as XListNode).Right = listB;
			return listA;
		}

		public IBListBuilder CreateListBuilder()
		{
			return new XListBuilder(GetNIL());
		}

		public IBListBuilder CreateListBuilder(IBNode firstItemValue)
		{
			var builder = new XListBuilder(GetNIL());
			builder.Append(firstItemValue);
			return builder;
		}
	}

}
