using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class BNodeWalker : IEnumerable<IBListNode>
	{
		class NodeEnumerator : IEnumerator<IBListNode>
		{
			bool isEmpty;
			IBListNode first, _current;

			public NodeEnumerator(IBNode first)
			{
				if (first == null)
				{
					throw new ArgumentNullException("NodeEnumerator can't be initialized with null");
				}
				if (!(first is IBListNode))
				{
					isEmpty = true;
				}
				else
				{
					this.first = first as IBListNode;
				}
			}

			public IBListNode Current
			{
				get
				{
					if (_current == null)
					{
						throw new ArgumentOutOfRangeException("Iterator is beyonf limits");
					}
					return _current;
				}
			}

			public void Dispose() { }

			object System.Collections.IEnumerator.Current { get { return Current; } }

			public bool MoveNext()
			{
				if (isEmpty)
				{
					return false;
				}

				if (_current != null && _current.Right.IsNIL)
				{
					_current = null;
					return false;
				}
				
				if (_current == null)
				{
					_current = first;
					return true;
				}

				_current = _current.Right as IBListNode;
				return true;
			}

			public void Reset()
			{
				_current = first;
			}
		}

		NodeEnumerator enumerator;
		private BNodeWalker(IBNode first)
		{
			enumerator = new NodeEnumerator(first);
		}

		public static BNodeWalker Walk(IBNode first)
		{
			return new BNodeWalker(first);
		}

		public IEnumerator<IBListNode> GetEnumerator()
		{
			return enumerator as IEnumerator<IBListNode>;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return enumerator;
		}
	}

}
