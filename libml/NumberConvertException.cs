using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	public class NumberConvertException : InvalidCastException
	{
		public NumberConvertException(string message)
			:base(message)
		{
		}
	}
}
