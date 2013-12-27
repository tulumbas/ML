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
	public class MLTest
	{
		string[,] testcases = new string[,] {
			{"(eq 'a '(a))", "NIL"},
			{"(eq (eval (car (cdr (cons 'a '((car '(z x c)) c))))) 'z)", "T"},
			{"(car (cdr '(a b c)))", "b"},
			{"(car (cdr '()))", "NIL"},
			{"T", "T"},
			{"Nil", "NIL"},
			{"(progn (car '(a b)) (cdr '(c d)))", "(d)"},
			{"(cond ((car '()) (cdr '(z c v))) (t 'a 'b))", "b"},
			{"(setq a (car '(3 b c)))", "3"},
			{"a","3"},
			{"(eval a)", "3"},
			{"(lambda (a) (car '(z x)))","#FUNCTION(a):((car (quote (z x))))"},
			{"((lambda (x y) (car x) (cadr y)) '(q w e) '(a s d))", "s"},
		};

		[Test]
		public void EvalStringTestEmpty()
		{
			var result = ML.EvalCommand("");
			Assert.IsNull(result);
		}

		[Test]
		public void EvalStringTest()
		{
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = ml.ML.EvalCommand(testcases[i, 0]);
				Assert.IsNotNull(result, "EvalString returned null");
				var text = ml.SequenceFormatter.AsString(result);
				Assert.IsNotEmpty(text, "Formatter could not print result");
				Assert.AreEqual(testcases[i, 1], text);
			}
		}

		[Test]
		public void UnknownSymbolExceptionTest()
		{
			Assert.Catch<UnknownSymbolException>(() => ML.EvalAndPrint("asdasdsdfsdf"));
		}

		[Test]
		public void ProtectedSymbolsTest()
		{
			var ex = Assert.Catch(() => ML.EvalAndPrint("(setq t nil)"));
			Trace.WriteLine(ex.ToString());
			ex = Assert.Catch(() => ML.EvalAndPrint("(setq nil t)"));
			Trace.WriteLine(ex.ToString());
		}

		[Test]
		public void BadFunctionCalls()
		{
			Assert.Catch<InvalidOperationException>(() => ML.EvalAndPrint("((car '(a c)))"));
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(zozozozo '(a c))"));
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(car '(a c) '(z c))"));
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(car)"));
		}

		[Test]
		public void FunctionCalls()
		{
			Assert.AreEqual("zz", ML.EvalAndPrint("(defun zz (a b c) (cons c (cons (car a) (cdr b))))"));
			Assert.AreEqual("((uu ii oo) qq tt yy)", ML.EvalAndPrint("(zz '(qq ww ee) '(rr tt yy) '(uu ii oo))"));
			Assert.Catch<ArgumentException>(() => ML.EvalAndPrint("(zz)"));
		}
	}
}
