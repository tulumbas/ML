using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	sealed class BNodeFuncAttribute : Attribute
	{
		int _numberOfArgs = -1;
		int _miminalNumberOfArgs = -1;
		public BNodeFuncAttribute()
		{
		}

		/// <summary>
		/// Marks that argumetns should be evaluated. 
		/// </summary>
		public ArgumentQuotingTypes ArgumentQuoting { get; set; }

		/// <summary>
		/// Arguments count should be checked before calling func
		/// </summary>
		public int NumberOfArguments
		{
			get
			{
				return _numberOfArgs;
			}
			set
			{
				_numberOfArgs = value;
			}
		}

		public int MinimalNumberOfArguments
		{
			get
			{
				return _miminalNumberOfArgs;
			}
			set
			{
				_miminalNumberOfArgs = value;
			}
		}

		/// <summary>
		/// Symbol name (in case it can't be reproduced in .Net)
		/// </summary>
		public string Alias { get; set; }
	}


	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	sealed class FunctionSetsAttribute : Attribute
	{
		public string SetupMethod { get; set; }
	}

	enum ArgumentQuotingTypes
	{
		None,
		First,
		All
	}
}
