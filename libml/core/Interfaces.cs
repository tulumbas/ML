using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	/// <summary>
	/// Common properties of atom
	/// </summary>
	public interface IAtom: IMLNode 
	{
		string Text { get; }
	}

	/// <summary>
	/// Nodes creation
	/// </summary>
	interface INodeFactory
	{
		IAtom CreateAtom(string value, NodeTypes atomType);
		IAtom GetT();
		IAtom GetNIL();
		IListBuilder CreateListBuilder();
		IListBuilder CreateListBuilder(IMLNode firstItemValue);
		ISymbolStorage SymbolScope { get;  }
	}

	/// <summary>
	/// Builds lists
	/// </summary>
	interface IListBuilder
	{
		/// <summary>
		/// Add new node and it's Left value (Right is NIL)
		/// </summary>
		/// <param name="nodeValue">value for Left</param>
		void Append(IMLNode nodeValue);

		/// <summary>
		/// Return ready list
		/// </summary>
		/// <returns></returns>
		IMLNode GetList();

		/// <summary>
		/// Add inline list (not a s "left" element)
		/// </summary>
		/// <param name="list">list to be added</param>
		void Merge(IListNode list);
	}

	interface INumberConverter
	{
		//bool NeedsConversion(NodeTypes from, NodeTypes to);
		bool CompareConvertType(ref ANumber n1, ref ANumber n2);
	}

	/// <summary>
	/// Evaluation environment, passed to function 
	/// and giving access to node builder and evaluation mechanism
	/// </summary>
	interface IEvaluator
	{
		IMLNode NIL { get; }

		INodeFactory Builder { get; }

		/// <summary>
		/// Checks whether it is atom or not and calls required code to evaluate node.
		/// </summary>
		/// <param name="arg">Node to be evaluated</param>
		/// <returns>Node with result of evaluation</returns>
		IMLNode EvalNode(IMLNode arg);

		/// <summary>
		/// Implicit PROGN. Evaluates nodes and returns last result.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		IMLNode EvalSequence(IListNode args);

		/// <summary>
		/// Applies function to list of args
		/// </summary>
		/// <param name="funcNode"></param>
		/// <param name="argList"></param>
		/// <param name="localScope"></param>
		/// <returns></returns>
		IMLNode ApplyCall(IMLNode funcNode, IMLNode argList);

		/// <summary>
		/// Symbols cache
		/// </summary>
		ISymbolStorage Symbols { get; }


		INumberConverter GetNumberConverter();
	}


}
