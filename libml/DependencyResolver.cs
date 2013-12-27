using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml
{
	static class DependencyResolver
	{
		public static INodeFactory Builder { get; private set; }

		static DependencyResolver()
		{
			Builder = new NodeFactory();
		}
	}
}
