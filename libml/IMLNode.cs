using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	public interface IMLNode
	{
		NodeTypes NodeType { get; }

		/// <summary>
		/// True - this is atom
		/// </summary>
		bool IsAtom { get; }

		/// <summary>
		/// True - either NIL or empty list
		/// </summary>
		bool IsNIL { get; }
	}


}
