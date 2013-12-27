using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class XListBuilder : IListBuilder
	{
		IMLNode NIL;
		IListNode first;
		XListNode current;

		internal XListBuilder(IMLNode nilAtom)
		{
			NIL = nilAtom;
		}

		public void Append(IMLNode nodeValue)
		{
			if (first == null)
			{
				current = new XListNode
				{
					Left = nodeValue,
					Right = NIL
				};
				first = current;
			}
			else
			{
				var node = new XListNode
				{
					Left = nodeValue,
					Right = NIL
				};
				current.Right = node;
				current = node;
			}
		}

		public IMLNode GetList()
		{
			return (first == null) ? NIL : first;
		}

		public void Merge(IListNode list)
		{
			if (first == null)
			{
				first = current = list as XListNode;
			}
			else
			{
				current.Right = list;
				while (!list.Right.IsNIL)
				{
					list = list.Right as IListNode;
				}
				current = list as XListNode;
			}
		}
	}

}
