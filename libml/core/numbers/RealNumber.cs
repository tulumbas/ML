using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core.numbers
{
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
