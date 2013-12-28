using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml;
using System.Diagnostics;

namespace mlnutest
{
	[TestFixture]
	public class MyFunctionsTest
	{
		string[,] isdefined = new string[,]
			{
				{"(is-defined-value 'a)", "NIL"},
				{"(is-defined-value 3)", "NIL" },
				{"(is-defined-value (lambda ()))", "NIL" },
				{"(is-defined-value t)", "T" },
				{"(is-defined-value ())", "T" },
				{"(is-defined-value '(car '(a v)))", "NIL" },
			};

		[Test]
		public void IsDefinedTest()
		{
			var eval = new ML();
			var testcases = isdefined;
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
			eval.EvalAndPrint("(setq a 4)");
			Assert.AreEqual("T", eval.EvalAndPrint("(is-defined-value 'a)"));
		}
	}
}
