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
		public bool IsCompiled { get; private set; }

		/// <summary>
		/// If greater then -1, function call 
		/// should be checked against number of arguments used.
		/// </summary>
		public int NumberOfArguments { get; set; }

		/// <summary>
		/// If greater then -1, function call 
		/// should be checked to have at least this number of args
		/// </summary>
		public int MinimalNumberOfArguments { get; set; }

		/// <summary>
		/// If not "None", prohibits arguments evaluation beforehand function call
		/// </summary>
		public ArgumentQuotingTypes ArgumentQuoting { get; set; }

		/// <summary>
		/// Compiled function pointer
		/// </summary>
		public Func<IListNode, IEvaluator, IMLNode> Method { get; private set; }

		/// <summary>
		/// If !Compiled, Body contains ML-executable code
		/// </summary>
		public IMLNode Body { get; private set; }

		/// <summary>
		/// If !Compiled, contains list of formal parameters
		/// </summary>
		public string[] ParameterNames { get; private set; }

		public static FunctionExecutionContext
			CreateCompiled(Func<IListNode, IEvaluator, IMLNode> compiledDelegate)
		{
			var ctx = new FunctionExecutionContext { IsCompiled = true, Method = compiledDelegate };
			return ctx;
		}

		public static FunctionExecutionContext CreateMLDefinition(IEnumerable<string> lambdaList, IMLNode body)
		{
			var ctx = new FunctionExecutionContext
			{
				IsCompiled = false,
				Body = body,
				ParameterNames = lambdaList.ToArray(),
				NumberOfArguments = lambdaList.Count()
			};
			return ctx;
		}

		/// <summary>
		/// Discrepancies bwtween formal and actual parameters counts are controlled by Evaluator and
		/// "NumberOfArguments" parameter
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public Dictionary<string, IMLNode> CreateLinks(IMLNode args)
		{
			var links = new Dictionary<string, IMLNode>();
			if (args != null)
			{
				var actual = BNodeWalker.Walk(args).ToArray();
				for (int i = 0; i < actual.Length; i++)
				{
					links.Add(ParameterNames[i], actual[i].Left);
				}
			}

			return links;
		}
	}
}
