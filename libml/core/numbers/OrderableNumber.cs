using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core.numbers
{
	abstract class OrderableNumber : ANumber
	{
		public abstract bool IsGreater(ANumber arg);

		//public static bool operator >(OrderableNumber A, OrderableNumber B)
		//{
		//   return A.IsGreater(B);
		//}

		//public static bool operator <(OrderableNumber A, OrderableNumber B)
		//{
		//   return B.IsGreater(A);
		//}

		//public static bool operator >=(OrderableNumber A, OrderableNumber B)
		//{
		//   return A.IsGreater(B) || A.IsEqualTo(B);
		//}

		//public static bool operator <=(OrderableNumber A, OrderableNumber B)
		//{
		//   return B.IsGreater(A) || A.IsEqualTo(B);
		//}

		public static OrderableNumber Check4OrderableNumber(IMLNode node)
		{
			if (node is OrderableNumber)
			{
				return node as OrderableNumber;
			}

			throw new NotANumberException("can't compare: " + SequenceFormatter.AsString(node));
		}
	}

}
