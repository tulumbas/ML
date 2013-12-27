using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	/// <summary>
	/// Meta info about function
	/// </summary>
	class FunctionExecutionContext
	{
		/// <summary>
		/// If true, function is defined in delegate "Method"
		/// </summary>
		public bool IsCompiled { get; set; }

		/// <summary>
		/// If greater then -1, function call should be checked against number of arguments used.
		/// </summary>
		public int NumberOfArguments { get; set; }

		/// <summary>
		/// If not "None", prohibits arguments evaluation beforehand function call
		/// </summary>
		public ArgumentQuotingTypes ArgumentQuoting { get; set; }

		/// <summary>
		/// Compiled function pointer
		/// </summary>
		public Func<IBListNode, IEvaluator, IBNode> Method { get; set; }
	}
}
