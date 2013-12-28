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
		public static ISymbolStorage Symbols { get; private set; }

		static DependencyResolver()
		{
			Symbols = new SymbolStorage();
			Builder = new NodeFactory(Symbols);
		}
	}
}
