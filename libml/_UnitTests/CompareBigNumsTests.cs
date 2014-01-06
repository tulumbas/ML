using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ml._UnitTests
{
	[TestFixture]
	class CompareBigNumsTests
	{
		string[,] group1 = new string[,]
		{
			{"(= 1 0)", "NIL"},
			{"(= 1 1)", "T"},
			{"(= 123456789012345678901234567890 123456789012345678901234567890)", "T" },
			{"(= 123456789012345678901234567890 223456789012345678901234567890)", "NIL" },
		},
		group2 = new string[,]
		{
			{"(!= 1 0)", "T"},
			{"(!= 1 1)", "NIL"},
			{"(!= 123456789012345678901234567890 123456789012345678901234567890)", "NIL" },
			{"(!= 123456789012345678901234567890 223456789012345678901234567890)", "T" },
		},
		group3 = new string[,]
		{
			{ "(< 999999999999 1000000000000)", "T"},
			{ "(< 1000000000000 999999999999)", "NIL"},
			{ "(<= 999999999999 1000000000000)", "T"},
			{ "(<= 1000000000000 999999999999)", "NIL"},
			{ "(> 999999999999 1000000000000)", "NIL"},
			{ "(> 1000000000000 999999999999)", "T"},
			{ "(>= 999999999999 1000000000000)", "NIL"},
			{ "(>= 1000000000000 999999999999)", "T"},
		},
		group4 = new string[,]
		{
			{ "(< 1 2 3 4 5 6)", "T" },
			{ "(< 1 2 3 4 6 5)", "NIL" },
			{ "(> 6 5 4 3 2 1)", "T" },
			{ "(> 6 5 4 3 1 2)", "NIL" },
		},
		group5 = new string[,]
		{
			{ "(<= 1 2 3 4 5 6)", "T" },
			{ "(<= 1 2 3 4 6 5)", "NIL" },
			{ "(>= 6 5 4 3 2 1)", "T" },
			{ "(>= 6 5 4 3 1 2)", "NIL" },
		};

		[Test]
		public void CompareBignumsTest1()
		{
			var eval = new ML();
			Common.RunTestCases(eval, group1);
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(< 1)"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(< 1)"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(< 1)"));
			Assert.Catch<ArgumentException>(() => eval.EvalAndPrint("(< 1)"));
		}

		[Test]
		public void CompareBignumsTest2()
		{
			var eval = new ML();
			Common.RunTestCases(eval, group2);
		}

		[Test]
		public void CompareBignumsTest3()
		{
			var eval = new ML();
			Common.RunTestCases(eval, group3);
		}

		[Test]
		public void CompareBignumsTest4()
		{
			var eval = new ML();
			Common.RunTestCases(eval, group4);
		}

		[Test]
		public void CompareBignumsTest5()
		{
			var eval = new ML();
			Common.RunTestCases(eval, group5);
		}
	}
}
