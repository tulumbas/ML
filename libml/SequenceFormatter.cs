using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml
{
	public static class SequenceFormatter
	{
		/// <summary>
		/// Builds textual representation of expression
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string AsString(IMLNode expression)
		{
			if (expression.IsAtom)
			{
				return (expression as IAtom).Text;
			}
			else
			{
				var sb = new StringBuilder("(");
				var first = true;
				while (!expression.IsNIL)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						sb.Append(" ");
					}

					var list = (expression as IListNode);
					sb.Append(AsString(list.Left));
					expression = list.Right;
				}
				sb.Append(")");
				return sb.ToString();
			}
		}
	}
}
