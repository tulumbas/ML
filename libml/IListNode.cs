using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml
{
	/// <summary>
	/// Element of sequence, containing generic Left and Right part
	/// </summary>
	public interface IListNode : IMLNode
	{
		IMLNode Left { get; }
		IMLNode Right { get; }
	}
}
