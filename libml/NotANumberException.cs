using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	public class NotANumberException: ArgumentException
	{
		public NotANumberException(string message)
			:base(message)
		{
		}
	}
}
