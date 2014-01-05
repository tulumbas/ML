using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml.core;
using System.Diagnostics;
using ml.core.numbers;

namespace ml._UnitTests
{
	[TestFixture]
	class BigNumProperTests
	{
		string[,] sums = new string[,]
			{
				{"999 999 999", "1", "1 000 000 000"},
				{"-999 999 999", "-1", "-1 000 000 000"},
				{"-999 999 999", "1", "-999 999 998"},
				{"999 999 999", "-1", "999 999 998"},
				{"999 999 999 999 999 999", "0", "999 999 999 999 999 999"},
			};

		string[,] diffs = new string[,]
			{
				{"1 000 000 000", "1", "999 999 999"},
				{"1 000 000 000 000 000 000", "1", "999 999 999 999 999 999"},
				{"0","999 999 999 999 999 999", "-999 999 999 999 999 999"},
				{"123 456 789", "-987 654 321", "1111 111 110"}
			};


		[Test]
		public void SumTest()
		{
			RunArithTests(sums, (b1, b2) => b1.Add(b2) as BigNum, "{0} + {1}");
			var eval = new ML();
			RunArithTests2(eval, sums, "(+ {0} {1})");
		}

		[Test]
		public void DiffTest1()
		{
			RunArithTests(diffs, (b1, b2) => b1.Subtract(b2) as BigNum, "{0} - {1}");
			var eval = new ML();
			RunArithTests2(eval, diffs, "(- {0} {1})");
		}


		private static void RunArithTests(string[,] tests,
			Func<BigNum, BigNum, BigNum> fn, string format)
		{
			BigNum b1, b2, b3;
			for (int i = 0; i < tests.GetLength(0); i++)
			{
				var test = string.Format(format, tests[i, 0], tests[i, 1]);

				Trace.WriteLine(test);
				b1 = BigNum.CreateBigNum(tests[i, 0].Replace(" ", ""));
				b2 = BigNum.CreateBigNum(tests[i, 1].Replace(" ", ""));
				b3 = fn(b1, b2);
				var result = tests[i, 2].Replace(" ", "");
				Assert.AreEqual(Math.Ceiling(result.Replace("-","").Length / 9.0), b3.Length, "number is of wrong len");
				Assert.AreEqual(result, b3.Text, test);
			}
		}

		private static void RunArithTests2(ML eval, string[,] tests, string fnFormat)
		{
			BigNum b1, b2;
			for (int i = 0; i < tests.GetLength(0); i++)
			{
				var t1 = tests[i, 0].Replace(" ", "");
				var t2 = tests[i, 1].Replace(" ", "");
				var test = string.Format(fnFormat, t1, t2);

				Trace.WriteLine(test);
				b1 = BigNum.CreateBigNum(t1);
				b2 = BigNum.CreateBigNum(t2);
				var s3 = eval.EvalAndPrint(test);
				var result = tests[i, 2].Replace(" ", "");
				Assert.AreEqual(result, s3, test);
			}

		}

	}
}
