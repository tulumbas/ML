using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml;

namespace mlnutest
{
	[TestFixture]
	public class NumbersTest
	{

		[Test]
		public void MinusPlusTest()
		{
			var eval = new ML();
			Assert.AreEqual("-abc", eval.EvalAndPrint("'-abc"));
			Assert.AreEqual("-3.0", eval.EvalAndPrint("-3.0"));
			Assert.AreEqual("-13.05", eval.EvalAndPrint("-13.05"));
			Assert.AreEqual("+3.0", eval.EvalAndPrint("+3.0"));
			Assert.AreEqual("+13.05", eval.EvalAndPrint("+13.05"));
			Assert.AreEqual("-", eval.EvalAndPrint("'-"));
			Assert.AreEqual("+", eval.EvalAndPrint("'+"));
		}

		[Test]
		public void BadNumbersTest()
		{
			var eval = new ML();

			Assert.Catch<BadInputException>(() => eval.EvalAndPrint("0.0.0"));
			Assert.Catch<BadInputException>(() => eval.EvalAndPrint("0.a"));
		}
	}
}
