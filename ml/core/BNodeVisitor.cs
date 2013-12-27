using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class BNodeVisitor
	{
		bool stopVisiting;

		/// <summary>
		/// callback params - current node and depth. returns false to stop visiting
		/// </summary>
		/// <param name="root"></param>
		/// <param name="callback"></param>
		public void VisitTree(IBNode root, Func<IBNode, int, bool> callback)
		{
			if (callback != null && root != null)
			{
				stopVisiting = false;
				VisitTreeInternal(root, callback, 0);
			}
		}

		private void VisitTreeInternal(IBNode root, Func<IBNode, int, bool> callback, int depth)
		{
			if (stopVisiting)
			{
				return;
			}

			var result = callback(root, depth);
			if (!result)
			{
				stopVisiting = true;
			}

			if (!root.IsAtom)
			{
				var list = root as IBListNode;
				VisitTreeInternal(list.Left, callback, depth + 1);
				if (stopVisiting)
				{
					return;
				}
				if (!list.Right.IsNIL) // don't call if list has ended
				{
					VisitTreeInternal(list.Right, callback, depth + 1);
				}
			}
		}
	}
}
