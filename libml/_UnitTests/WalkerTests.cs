using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ml;
using ml.core;

namespace ml._UnitTests
{
	[TestFixture]
	public class WalkerTests
	{
		[Test]
		public void EmptyWalkerTest()
		{
			Assert.Catch<ArgumentNullException>(() => BNodeWalker.Walk(null));
			
			var eval = new ML();
			var nil = eval.EvalCommand("()");
			var list = eval.EvalCommand("'(a b c)");

			var walkerNil = BNodeWalker.Walk(nil);
			Assert.AreEqual(0, walkerNil.Count());
			Assert.IsNull(walkerNil.FirstOrDefault());

			var walker3 = BNodeWalker.Walk(list);
			var iterator3 = walker3.GetEnumerator();
			Assert.Catch<ArgumentOutOfRangeException>(() => { var x = iterator3.Current; });
			Assert.AreEqual(3, walker3.Count());
			Assert.IsInstanceOf<IListNode>(walker3.First());
			Assert.AreEqual("a", SequenceFormatter.AsString(walker3.First().Left));
			Assert.AreEqual("c", SequenceFormatter.AsString(walker3.Last().Left));

			foreach (var item in (walker3 as System.Collections.IEnumerable))
			{
				Assert.IsInstanceOf<IListNode>(item);
			}			
		}
	}
}
