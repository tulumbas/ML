using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using ml;

namespace mlnutest
{
	class Common
	{
		public static void RunTestCases(ML eval, string[,] testcases)
		{
			if (eval == null)
			{
				eval = new ML();
			}

			for (int i = 0; i < testcases.GetLength(0); i++)
			{
				Trace.WriteLine(string.Format("Test {0}: {1}", i + 1, testcases[i, 0]));
				var result = eval.EvalAndPrint(testcases[i, 0]);
				Assert.AreEqual(testcases[i, 1], result);
			}
		}
	}
}
