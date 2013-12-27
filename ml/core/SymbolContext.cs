using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class SymbolContext
	{
		Dictionary<string, object> properties;

		public IBNode Value { get; set; }
		public FunctionExecutionContext Function { get; set; }
		public Dictionary<string, object> Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new Dictionary<string, object>();
				}
				return properties;
			}
		}
	}
}
