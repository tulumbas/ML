using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	public class BadInputException: ApplicationException 
	{
		public BadInputException(string message)
			:base(message)
		{
		}
	}
}
