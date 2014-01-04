using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using ml;


namespace ml._UnitTests
{
	[TestFixture]
	class BasicFunctionsTests
	{
		string[,] quotes = new string[,]
		{
			{"' a", "a"},
			{"(quote (a b))", "(a b)"},
		};

		string[,] caddrs = new string[,]
		{
			{"(cadr '(a b c))", "b" },
			{"(cdar '((a b) c))", "(b)" },
			{"(cdadr '(a (b c) d))", "(c)" },
			{"(cddr '(a b c))", "(c)" },
		};

		[Test]
		public void QuoteTest()
		{
			var eval = new ML();
			var testcases = quotes;
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(quote)"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(quote a b)"));
		}

		[Test]
		public void CarTest()
		{
			var eval = new ML();
			Assert.AreEqual("a", eval.EvalAndPrint("(car '(a))"));
			Assert.AreEqual("a", eval.EvalAndPrint("(car '(a b))"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(car '())"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(car ())"));
			Assert.Catch<ArgumentException>(() => eval.EvalCommand("(car)"), "(car)");
			Assert.Catch<ArgumentException>(() => eval.EvalCommand("(car 3)"), "(car 3)");
		}

		[Test]
		public void CondTest()
		{
			var eval = new ML();
			Assert.AreEqual("NIL", eval.EvalAndPrint("(cond)"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(cond (()))"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(cond (() ()))"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(cond (() ()) ( t ()))"));
			Assert.AreEqual("T", eval.EvalAndPrint("(cond (t))"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(cond ())"));
		}

		[Test]
		public void EqTest()
		{
			var eval = new ML();
			Assert.AreEqual("NIL", eval.EvalAndPrint("(eq 'a '(a))"));
			Assert.AreEqual("T", eval.EvalAndPrint("(eq 'NIL '())"));
		}

		[Test]
		public void PrognTest()
		{
			var eval = new ML();
			Assert.AreEqual("(d)", eval.EvalAndPrint("(progn (car '(a b)) (cdr '(c d)))"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(progn)"));
			Assert.AreEqual("T", eval.EvalAndPrint("(progn t)"));
			Assert.AreEqual("100", eval.EvalAndPrint("(progn  t nil 100)"));
		}

		[Test]
		public void SetQTest()
		{
			var eval = new ML();
			Assert.AreEqual("d", eval.EvalAndPrint("(setq x 'd)"));
			Assert.AreEqual("d", eval.EvalAndPrint("x"), "Lost SETQ assignment");
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(setq 'a 's)"));
		}

		[Test]
		public void IsNullPTest()
		{
			var eval = new ML();
			Assert.AreEqual("T", eval.EvalAndPrint("(null ())"), "()");
			Assert.AreEqual("T", eval.EvalAndPrint("(null nil)"), "nil");
			Assert.AreEqual("T", eval.EvalAndPrint("(null '())"), "'()");
			Assert.AreEqual("T", eval.EvalAndPrint("(null 'nil)"), "'nil");
			Assert.AreEqual("NIL", eval.EvalAndPrint("(null 'a)"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(null 3)"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(null '(a b))"));
		}

		[Test]
		public void CaddrsTest()
		{
			var eval = new ML();
			var testcases = caddrs;
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
		}
	}
}
