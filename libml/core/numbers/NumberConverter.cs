using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core.numbers
{
	class NumberConverter
	{
		static readonly NodeTypes[] typesHierarchy = new NodeTypes[]
		{
			NodeTypes.IntegerNumber,
			NodeTypes.RealNumber,
		};


		public bool CompareConvertType(ref ANumber n1, ref ANumber n2)
		{
			if (n1.NodeType == n2.NodeType)
			{
				return false;
			}

			int t1 = 0, t2 = 0;
			for (int i = 0; i < typesHierarchy.Length; i++)
			{
				if (n1.NodeType == typesHierarchy[i])
				{
					t1 = i;
				}
				else if (n2.NodeType == typesHierarchy[i])
				{
					t2 = i;
				}
			}

			if (t2 > t1)
			{
				n1 = ConvertTo(n1, n2.NodeType);
			}
			else
			{
				n2 = ConvertTo(n2, n1.NodeType);
			}

			return true;
		}

		private ANumber ConvertTo(ANumber num, NodeTypes toType)
		{
			switch (toType)
			{
				case NodeTypes.RealNumber:
					if (num is BigNum)
					{
						//return new RealNumber(num as BigNum);
					}
					throw new NumberConvertException("Can't convert " +
						num.NodeType.ToString() + " to " + toType.ToString());


				default:
					throw new NumberConvertException("Can't convert " +
						num.NodeType.ToString() + " to " + toType.ToString());
			}
		}
	}
}
