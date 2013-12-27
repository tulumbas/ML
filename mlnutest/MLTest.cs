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
			var eval = new ML();
			var result = eval.EvalCommand("");
			Assert.IsNull(result);
		}

		[Test]
		public void EvalStringTest()
		{
			var eval = new ML();
			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
		}

		[Test]
		public void UnknownSymbolExceptionTest()
		{
			var eval = new ML();
			Assert.Catch<UnknownSymbolException>(() => eval.EvalAndPrint("asdasdsdfsdf"));
		}

		[Test]
		public void ProtectedSymbolsTest()
		{
			var eval = new ML();
			var ex = Assert.Catch(() => eval.EvalAndPrint("(setq t nil)"));
			Trace.WriteLine(ex.ToString());
			ex = Assert.Catch(() => eval.EvalAndPrint("(setq nil t)"));
			Trace.WriteLine(ex.ToString());
		}

		[Test]
		public void BadFunctionCalls()
		{
			var eval = new ML();
			Assert.Catch<InvalidOperationException>(() => eval.EvalAndPrint("((car '(a c)))"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(zozozozo '(a c))"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(car '(a c) '(z c))"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(car)"));
		}

		[Test]
		public void FunctionCalls()
		{
			var eval = new ML();
			Assert.AreEqual("zz", eval.EvalAndPrint("(defun zz (a b c) (cons c (cons (car a) (cdr b))))"));
			Assert.AreEqual("((uu ii oo) qq tt yy)", eval.EvalAndPrint("(zz '(qq ww ee) '(rr tt yy) '(uu ii oo))"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(zz)"));
		}
	}
}
