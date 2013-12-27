using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml;

namespace mlrun
{
	class TreePrinter
	{
		public static void PrintBTree(IBNode root)
		{
			if (root != null)
			{
				PrintLevel(root);
				Console.WriteLine();
			}
		}

		static void PrintLevel(IBNode node)
		{
			if (node.IsAtom)
			{
				Console.Write((node as IBAtom).Text);
			}
			else
			{
				Console.Write("(");
				var first = true;
				while (!node.IsNIL) 
				{
					if (first)
					{
						first = false;
					}
					else
					{
						Console.Write(" ");
					}
					var list = node as IBListNode;
					PrintLevel(list.Left);

					node = list.Right;
				}				
				Console.Write(")");
			}
		}
	}
}
