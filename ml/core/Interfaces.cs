using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	interface IBNode
	{
		NodeTypes NodeType { get; }

		/// <summary>
		/// True - this is atom
		/// </summary>
		bool IsAtom { get; }

		/// <summary>
		/// True - either NIL or empty list
		/// </summary>
		bool IsNIL { get; }
	}

	interface IBAtom: IBNode 
	{
		string Text { get; }
	}

	interface IBListNode: IBNode 
	{
		IBNode Left { get; }
		IBNode Right { get; }
	}

	interface IBNodeBuilder
	{
		IBAtom CreateAtom(string value, NodeTypes atomType);
		IBAtom GetT();
		IBAtom GetNIL();
		IBListBuilder CreateListBuilder();
		IBListBuilder CreateListBuilder(IBNode firstItemValue);
	}

	/// <summary>
	/// Builds lists
	/// </summary>
	interface IBListBuilder
	{
		/// <summary>
		/// Add new node and it's Left value (Right is NIL)
		/// </summary>
		/// <param name="nodeValue">value for Left</param>
		void Append(IBNode nodeValue);

		/// <summary>
		/// Return ready list
		/// </summary>
		/// <returns></returns>
		IBNode GetList();

		/// <summary>
		/// Add inline list (not a s "left" element)
		/// </summary>
		/// <param name="list">list to be added</param>
		void Merge(IBListNode list);
	}

	/// <summary>
	/// Evaluation environment, passed to function 
	/// and giving access to node builder and evaluation mechanism
	/// </summary>
	interface IEvaluator
	{
		IBNodeBuilder Builder { get; }

		/// <summary>
		/// Checks whether it is atom or not and calls required code to evaluate node.
		/// </summary>
		/// <param name="arg">Node to be evaluated</param>
		/// <returns>Node with result of evaluation</returns>
		IBNode EvalNode(IBNode arg, Dictionary<string, IBNode> links);

		/// <summary>
		/// Implicit PROGN. Evaluates nodes and returns last result.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		IBNode EvalSequence(IBListNode args, Dictionary<string, IBNode> links);

		//LocalSymbolStorage localScope { get; }
	}
}
