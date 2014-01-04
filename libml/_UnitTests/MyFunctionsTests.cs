using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml;
using System.Diagnostics;

namespace ml._UnitTests
{
	[TestFixture]
	public class MyFunctionsTests
	{
		string[,] isdefined = new string[,]
			{
				{"(is-defined-value 'a)", "NIL"},
				{"(is-defined-value 3)", "NIL" },
				{"(is-defined-value (lambda ()))", "NIL"},
				{"(is-defined-value t)", "T"},
				{"(is-defined-value ())", "T"},
				{"(is-defined-value '(car '(a v)))", "NIL"},
			};

		string[,] reversed = new string[,] 
			{
				{"(reverse '(a b c))", "(c b a)"},
				{"(reverse '())", "NIL"}
			};

		[Test]
		public void IsDefinedTest()
		{
			var eval = new ML();
			Common.RunTestCases(eval, isdefined);
			eval.EvalAndPrint("(setq a 4)");
			Assert.AreEqual("T", eval.EvalAndPrint("(is-defined-value 'a)"));
		}

		[Test]
		public void ReverseTest()
		{
			var eval = new ML();
			Common.RunTestCases(eval, reversed);
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(reverse '() '())"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(reverse 'a)"));
		}


	}
}
