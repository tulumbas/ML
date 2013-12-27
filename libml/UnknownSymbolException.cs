using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	public class UnknownSymbolException: ApplicationException
	{
		public UnknownSymbolException(IMLNode symbolNode)
			:base("Unknown symbol: " + SequenceFormatter.AsString(symbolNode))
		{
		}
	}
}
