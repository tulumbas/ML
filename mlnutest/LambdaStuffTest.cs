using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using ml;

namespace mlnutest
{
	[TestFixture]
	public class LambdaStuffTest
	{
		string[] lambdaArgErrCases = new string[]
			{
				"((lambda))",
				"(lambda)",
				"(lambda a)",
				"(lambda (a (c)))",
				"(defun)",
				"(defun ())",
				"(defun (a))",
				"(defun a)",
				"((lambda (z) nil))" // not enough args
			};

		[Test]
		public void LambdaArgErrors()
		{
			foreach (var item in lambdaArgErrCases)
			{
				var testcase = item;
				var ex = Assert.Catch<ArgumentException>(() => ML.EvalCommand(testcase));
				Trace.WriteLine("Arg error: " + ex.Message);
			}
			Assert.AreEqual("NIL", ML.EvalAndPrint("((lambda () nil))")); 
		}
	}
}
