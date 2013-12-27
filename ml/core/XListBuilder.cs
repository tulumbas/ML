using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class XListBuilder : IBListBuilder
	{
		IBNode NIL;
		IBListNode first;
		XListNode current;

		internal XListBuilder(IBNode nilAtom)
		{
			NIL = nilAtom;
		}

		public void Append(IBNode nodeValue)
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

		public IBNode GetList()
		{
			return (first == null) ? NIL : first;
		}

		public void Merge(IBListNode list)
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
					list = list.Right as IBListNode;
				}
				current = list as XListNode;
			}
		}
	}

}
