using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml;
using ml.core;
using ml.core.numbers;

namespace ml._UnitTests
{
	[TestFixture]
	public class NumbersTests
	{		
		[Test]
		public void IntegerNumbersInputTest()
		{
			var eval = new ML();
			var node = eval.EvalCommand("1000000000");
			Assert.AreEqual(NodeTypes.IntegerNumber, node.NodeType);
			node = eval.EvalCommand("-1000000000");
			Assert.AreEqual(NodeTypes.IntegerNumber, node.NodeType);
			Assert.AreEqual("0", eval.EvalAndPrint("-00000000000000000000000000"));
			node = eval.EvalCommand("-0000000000000000000000000000");
			//Assert.AreEqual(0, ((ISingleNumber)node).Sign);
			//Assert.AreEqual(1, ((IIntegerNumber)node).Nonets.Length);
			Assert.AreEqual("123456789", eval.EvalAndPrint("123456789"));
			Assert.AreEqual("1234567890", eval.EvalAndPrint("1234567890"));
			Assert.AreEqual("123456789", eval.EvalAndPrint("0123456789"));
			Assert.AreEqual("-123456789", eval.EvalAndPrint("-0123456789"));
			Assert.AreEqual("123456789123456789", eval.EvalAndPrint("123456789123456789"));
		}

		[Test]
		public void FloatingPointNumbersInputTest()
		{
			var eval = new ML();
			var node = eval.EvalCommand("1000000000.0");
			Assert.AreEqual(NodeTypes.RealNumber, node.NodeType);
			node = eval.EvalCommand("-1000000000.0");
			Assert.AreEqual(NodeTypes.RealNumber, node.NodeType);
		}

		[Test]
		public void MinusPlusTest()
		{
			var eval = new ML();

			Common.RunTestCases(eval,
				new string[,]
				{
					{"'-abc", "-abc"},
					{"-3.0", "-3.0" },
					{"-13.05", "-13.05"},
					{"'+3.0", "+3.0"},
					{"'+13.05","+13.05"},
					{"'-", "-"},
					{"'+", "+"},
				});
			
			var node = eval.EvalCommand("'--123");
			Assert.AreEqual(NodeTypes.Symbol, node.NodeType);
		}

		[Test]
		public void BadNumbersTest()
		{
			var eval = new ML();

			Assert.Catch<BadInputException>(() => eval.EvalAndPrint("0.0.0"));
			Assert.Catch<BadInputException>(() => eval.EvalAndPrint("0.a"));
		}

		[Test]
		public void BigNumCreationTest()
		{
			var bn = BigNum.CreateBigNum("-123456789012345678");
			Assert.AreEqual(2, bn.Nonets.Length);
			Assert.AreEqual(-1, bn.Sign);
			Assert.AreEqual("-123456789012345678", bn.PrintNumber());
			Assert.AreEqual(12345678, bn.Nonets[0]);
			Assert.AreEqual(123456789, bn.Nonets[1]);
			bn = BigNum.CreateBigNum("123");
			Assert.AreEqual(1, bn.Nonets.Length);
		}		
	}
}
