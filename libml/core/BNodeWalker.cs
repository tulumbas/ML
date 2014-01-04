using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	public class BNodeWalker : IEnumerable<IListNode>
	{
		class NodeEnumerator : IEnumerator<IListNode>
		{
			bool isEmpty, enumeratedAlready;
			IListNode first, _current;

			public NodeEnumerator(IMLNode first)
			{
				if (first == null)
				{
					throw new ArgumentNullException("NodeEnumerator can't be initialized with null");
				}
				if (!(first is IListNode))
				{
					isEmpty = true;
				}
				else
				{
					this.first = first as IListNode;
				}
			}

			public IListNode Current
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

				if (_current == null)
				{
					if (!enumeratedAlready)
					{
						_current = first;
						return true;
					}
					return false;
				}
				else if (_current.Right.IsNIL)
				{
					_current = null;
					enumeratedAlready = true;
					return false;
				}
				else
				{
					_current = _current.Right as IListNode;
					return true;
				}
			}

			public void Reset()
			{
				_current = null;
				enumeratedAlready = false;
			}
		}

		NodeEnumerator enumerator;
		private BNodeWalker(IMLNode first)
		{
			enumerator = new NodeEnumerator(first);
		}

		public static BNodeWalker Walk(IMLNode first)
		{
			return new BNodeWalker(first);
		}

		public IEnumerator<IListNode> GetEnumerator()
		{
			enumerator.Reset();
			return enumerator as IEnumerator<IListNode>;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			enumerator.Reset();
			return enumerator as System.Collections.IEnumerator;
		}
	}

}
