using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using ml;

namespace ml._UnitTests
{
	[TestFixture]
	public class LambdaStuffTests
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
				"(defun t (a))",
				"(defun t ())",
				"((lambda (z) nil))", // not enough args
				"((lambda (z) nil) 1 2)" // too much args
			};

		string[,] applies = new string[,]
			{
				{"(apply 'car '((a b)) )", "a"},
				{"(apply 'null '(()))", "T"},
				{"(apply 'atom '(a))", "T"},
				{"(apply (lambda (x) (car x)) '((a c)))", "a"}
			};

		string[,] funcalls = new string[,]
			{
				{"(funcall 'car '((a b)) )", "(a b)"},
				{"(funcall 'null '(()))", "NIL"},
				{"(funcall 'atom 'a)", "T"},
				{"(funcall 'cons 'a '(b))", "(a b)"},
				{"(funcall (lambda (x) (car x)) '(a c))", "a"}
			};

		[Test]
		public void LambdaArgErrors()
		{
			var eval = new ML();
			foreach (var item in lambdaArgErrCases)
			{
				var testcase = item;
				var ex = Assert.Catch<ArgumentException>(() => eval.EvalCommand(testcase));
				Trace.WriteLine("Arg error: " + ex.Message);
			}
			Assert.AreEqual("NIL", eval.EvalAndPrint("((lambda () nil))"));
			Assert.AreEqual("NIL", eval.EvalAndPrint("(null (lambda ()))"));
			Assert.AreEqual("T", eval.EvalAndPrint("(atom (lambda ()))"));
			Assert.AreEqual("z", eval.EvalAndPrint("(defun z ())"));
		}

		[Test]
		public void ApplyTest()
		{
			var eval = new ML();
			var testcases = applies;
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
		}

		[Test]
		public void FuncallTest()
		{
			var eval = new ML();
			var testcases = funcalls;
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
		}

	}
}
