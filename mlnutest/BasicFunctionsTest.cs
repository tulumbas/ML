using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using ml;


namespace mlnutest
{
	[TestFixture]
	class BasicFunctionsTest
	{
		[Test]
		public void CarTest()
		{
			Assert.AreEqual("a", ML.EvalAndPrint("(car '(a))"));
			Assert.AreEqual("a", ML.EvalAndPrint("(car '(a b))"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(car '())"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(car ())"));
			Assert.Catch<ArgumentException>(() => ML.EvalCommand("(car)"));
			Assert.Catch<ArgumentException>(() => ML.EvalCommand("(car 3)"));
		}

		[Test]
		public void CondTest()
		{
			Assert.AreEqual("NIL", ML.EvalAndPrint("(cond)"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(cond (()))"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(cond (() ()))"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(cond (() ()) ( t ()))"));
			Assert.AreEqual("T", ML.EvalAndPrint("(cond (t))"));
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(cond ())"));
		}

		[Test]
		public void EqTest()
		{
			Assert.AreEqual("NIL", ML.EvalAndPrint("(eq 'a '(a))"));
			Assert.AreEqual("T", ML.EvalAndPrint("(eq 'NIL '())"));
		}

		[Test]
		public void PrognTest()
		{
			Assert.AreEqual("(d)", ML.EvalAndPrint("(progn (car '(a b)) (cdr '(c d)))"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(progn)"));
			Assert.AreEqual("T", ML.EvalAndPrint("(progn t)"));
			Assert.AreEqual("100", ML.EvalAndPrint("(progn  t nil 100)"));
		}

		[Test]
		public void SetQTest()
		{
			Assert.AreEqual("d", ML.EvalAndPrint("(setq x 'd)"));
			Assert.AreEqual("d", ML.EvalAndPrint("x"), "Lost SETQ assignment");
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(setq 'a 's)"));
		}

		[Test]
		public void IsNullPTest()
		{
			Assert.AreEqual("T", ML.EvalAndPrint("(null ())"), "()");
			Assert.AreEqual("T", ML.EvalAndPrint("(null nil)"), "nil");
			Assert.AreEqual("T", ML.EvalAndPrint("(null '())"), "'()");
			Assert.AreEqual("T", ML.EvalAndPrint("(null 'nil)"), "'nil");
			Assert.AreEqual("NIL", ML.EvalAndPrint("(null 'a)"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(null 3)"));
			Assert.AreEqual("NIL", ML.EvalAndPrint("(null '(a b))"));
		}
	}
}
