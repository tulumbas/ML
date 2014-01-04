using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml.core;

namespace ml._UnitTests
{
	[TestFixture]
	class NodeFactoryTests
	{
		class ScopelessNodeFactoryTests: NodeFactoryTests
		{
			public ScopelessNodeFactoryTests()
			{
				factory = new NodeFactory();
			}
		}		
		
		protected NodeFactory factory;

		public NodeFactoryTests()
		{
			factory = new NodeFactory(new SymbolStorage());
		}

		[Test]
		public void NILTest()
		{
			var nil = factory.GetNIL();
			Assert.AreEqual("NIL", nil.Text);
			Assert.AreEqual(NodeTypes.NIL, nil.NodeType);
			Assert.IsTrue(nil.IsAtom);
			Assert.IsTrue(nil.IsNIL);
			Assert.AreEqual(nil, factory.GetNIL(), "Nil object reference is non-unique");
		}

		[Test]
		public void TTest()
		{
			var t = factory.GetT();
			Assert.AreEqual("T", t.Text);
			Assert.AreEqual(NodeTypes.T, t.NodeType);
			Assert.IsTrue(t.IsAtom);
			Assert.IsFalse(t.IsNIL);
			Assert.AreEqual(t, factory.GetT(), "T object reference is non-unique");
		}

	}
}
