using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

namespace ml.code
{
	class Evaluator: IEvaluator
	{
		public const string LAMBDA = "lambda";
		private readonly IMLNode nil;		

		#region Setup

		public Evaluator(ISymbolStorage scope, INodeFactory builder)
		{
			Builder = builder;
			Symbols = scope;
			nil = builder.GetNIL();
		}

		#endregion

		#region IEvaluator

		/// <summary>
		/// Cached NIL
		/// </summary>
		public IMLNode NIL { get { return nil; } }

		/// <summary>
		/// Current node factory
		/// </summary>
		public INodeFactory Builder { get; private set; }

		/// <summary>
		/// Current global scope
		/// </summary>
		public ISymbolStorage Symbols { get; private set; }

		/// <summary>
		/// Eval sungle expression. Either atom or list.
		/// </summary>
		/// <param name="arg">expression to be evaluated</param>
		/// <param name="links">local variable scope</param>
		/// <returns></returns>
		public IMLNode EvalNode(IMLNode arg, Dictionary<string, IMLNode> links)
		{
			return arg.IsAtom ? EvalAtom(arg as IAtom, links) : EvalFunc(arg as IListNode, links);
		}

		/// <summary>
		/// Implicit progn
		/// </summary>
		/// <param name="args"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		public IMLNode EvalSequence(IListNode args, Dictionary<string, IMLNode> links)
		{
			IMLNode result = NIL;
			if (args != null)
			{
				foreach (var item in BNodeWalker.Walk(args))
				{
					result = EvalNode(item.Left, links);
				}
			}
			return result;
		}

		public IMLNode ApplyCall(IMLNode funcNode, IMLNode argList, Dictionary<string, IMLNode> localScope)
		{
			IAtom funcAtom;
			var fcontext = LoadFunctionDefinition(funcNode, localScope, out funcAtom);
			int argumentsCount = argList.IsNIL ? 0 : BNodeWalker.Walk(argList).Count();
			CheckArgumentsCount(fcontext, funcAtom.Text, argumentsCount);
			return CallFunction(fcontext, argList);
		}

		public INumberConverter GetNumberConverter()
		{
			return new NumberConverter();
		}

		#endregion

		#region Calcualtions

		private IMLNode EvalAtom(IAtom atom, Dictionary<string, IMLNode> localScope)
		{
			switch (atom.NodeType)
			{
				case NodeTypes.NIL: 					
				case NodeTypes.T:
				case NodeTypes.RealNumber:
				case NodeTypes.IntegerNumber:
					return atom;

				case NodeTypes.Symbol:
				case NodeTypes.TextLiteral:
					// 1. check local scope
					IMLNode value;
					if (localScope != null 
						&& localScope.TryGetValue(SymbolStorage.NormalizeName(atom.Text), out value))
					{
						return value;
					}

					var sc = Symbols[atom.Text];
					if (sc == null || sc.Value == null)
					{
						throw new UnknownSymbolException(atom);
					}
					return sc.Value;
					
				default:
					throw new InvalidOperationException("Node type not supported: " + atom.NodeType);

			}
		}

		/// <summary>
		/// (func arg1 arg2 arg3 ....
		/// - find the function definiton
		/// - create a list of evaluated args
		/// - call the func, passing new list
		/// </summary>
		/// <param name="list">list with source args</param>
		/// <returns>result of calucation</returns>
		private IMLNode EvalFunc(IListNode list, Dictionary<string, IMLNode> localScope)
		{
			// checking function name
			IAtom funcAtom;
			int argumentsCount;

			var fcontext = LoadFunctionDefinition(list.Left, localScope, out funcAtom);
			var argList = EvalArgumentList(list, localScope, fcontext.ArgumentQuoting, out argumentsCount);
			CheckArgumentsCount(fcontext, funcAtom.Text, argumentsCount);
			return CallFunction(fcontext, argList);
		}

		private IMLNode CheckAndEvalLambda(IListNode expression, 
			Dictionary<string, IMLNode> links)
		{
			if (expression.Left.NodeType != NodeTypes.Symbol
				|| ((IAtom)expression.Left).Text.ToLowerInvariant() != LAMBDA)
			{
				throw new InvalidOperationException("Not a lambda expression");
			}

			if (expression.Right.IsNIL)
			{
				throw new ArgumentException("LAMBDA can't be called with no args");
			}

			//var args = (IBListNode)expression.Right;
			//return LambdaStuff.Lambda(args, Instance) as LambdaNode;
			return EvalFunc(expression, links);
		}

		#endregion
		
		#region helpers
		private static void TooFewArguments(string funcName)
		{
			throw new ArgumentException("Too few arguments for function " + funcName);
		}

		private static void TooManyArguments(string funcName)
		{
			throw new ArgumentException("Too many arguments for function " + funcName);
		}

		//private IMLNode PrepareFunctionCallArguments(IListNode functionExpression,
		//   Dictionary<string, IMLNode> localScope,
		//   FunctionExecutionContext fcontext,
		//   string funcName)
		//{
		//   int argumentsCount = 0;
		//   var checkMinimalNumberOfArguments =
		//      fcontext.NumberOfArguments >= 0 || fcontext.MinimalNumberOfArguments >= 0;
		//   var checkMaximalNumberOfArguments = fcontext.NumberOfArguments >= 0;

		//   var minimalNumberOfArgs =
		//      fcontext.NumberOfArguments >= 0 ? fcontext.NumberOfArguments : fcontext.MinimalNumberOfArguments;
		//   var maximalNumberOfArgs = fcontext.NumberOfArguments;

		//   IMLNode argList;
		//   var walker = BNodeWalker.Walk(functionExpression);
		//   if (fcontext.ArgumentQuoting == ArgumentQuotingTypes.All)
		//   {
		//      argList = functionExpression.Right;
		//      argumentsCount = walker.Skip(1).Count();
		//      if (checkMaximalNumberOfArguments && (argumentsCount > maximalNumberOfArgs))
		//      {
		//         TooManyArguments(funcName);
		//      }
		//   }
		//   else
		//   {
		//      var listBuilder = Builder.CreateListBuilder();
		//      foreach (var listNode in walker.Skip(1))
		//      {
		//         var arg = listNode.Left;
		//         var evalArg = fcontext.ArgumentQuoting == ArgumentQuotingTypes.None
		//            || ((fcontext.ArgumentQuoting == ArgumentQuotingTypes.First) && (argumentsCount > 0));
		//         if (evalArg)
		//         {
		//            arg = EvalNode(arg, localScope);
		//         }

		//         listBuilder.Append(arg);
		//         argumentsCount++;

		//         if (checkMaximalNumberOfArguments && (argumentsCount > maximalNumberOfArgs))
		//         {
		//            TooManyArguments(funcName);
		//         }
		//      }
		//      argList = listBuilder.GetList();
		//   }

		//   // check and build arglist
		//   if (checkMinimalNumberOfArguments && (argumentsCount < minimalNumberOfArgs))
		//   {
		//      TooFewArguments(funcName);
		//   }
		//   return argList;
		//}

		private void CheckArgumentsCount(FunctionExecutionContext fcontext, string funcName,
			int argumentsCount)
		{
			var checkMinimalNumberOfArguments =
				fcontext.NumberOfArguments >= 0 || fcontext.MinimalNumberOfArguments >= 0;
			var checkMaximalNumberOfArguments = fcontext.NumberOfArguments >= 0;

			var minimalNumberOfArgs =
				fcontext.NumberOfArguments >= 0 ? fcontext.NumberOfArguments : fcontext.MinimalNumberOfArguments;
			var maximalNumberOfArgs = fcontext.NumberOfArguments;

			if (checkMaximalNumberOfArguments && (argumentsCount > maximalNumberOfArgs))
			{
				TooManyArguments(funcName);
			}

			if (checkMinimalNumberOfArguments && (argumentsCount < minimalNumberOfArgs))
			{
				TooFewArguments(funcName);
			}
		}

		private IMLNode EvalArgumentList(IListNode functionExpression,
			Dictionary<string, IMLNode> localScope,
			ArgumentQuotingTypes quotingType,
			out int argumentsCount)
		{
			IMLNode argList;
			var walker = BNodeWalker.Walk(functionExpression);
			if (quotingType == ArgumentQuotingTypes.All)
			{
				argList = functionExpression.Right;
				argumentsCount = walker.Skip(1).Count();
			}
			else
			{
				argumentsCount = 0;
				var listBuilder = Builder.CreateListBuilder();
				foreach (var listNode in walker.Skip(1))
				{
					var arg = listNode.Left;
					var evalArg = quotingType == ArgumentQuotingTypes.None
						|| ((quotingType == ArgumentQuotingTypes.First) && (argumentsCount > 0));
					if (evalArg)
					{
						arg = EvalNode(arg, localScope);
					}

					listBuilder.Append(arg);
					argumentsCount++;
				}
				argList = listBuilder.GetList();
			}
			return argList;
		}

		private FunctionExecutionContext LoadFunctionDefinition(IMLNode funcNode, Dictionary<string, IMLNode> links, out IAtom funcAtom)
		{
			FunctionExecutionContext fcontext;
			LambdaNode lambdaNode;
			switch (funcNode.NodeType)
			{
				case NodeTypes.Symbol:
					funcAtom = (funcNode as IAtom);
					var scontext = Symbols[funcAtom.Text];
					if (scontext == null || scontext.Function == null)
					{
						throw new ArgumentException("Unknown function name: " + funcAtom.Text);
					}
					fcontext = scontext.Function;
					break;

				case NodeTypes.Lambda: /* used in apply and funcall */
					lambdaNode = (funcNode as LambdaNode);
					fcontext = lambdaNode.ExecutionContext;
					funcAtom = lambdaNode;
					break;

				case NodeTypes.List:
					lambdaNode = CheckAndEvalLambda(funcNode as IListNode, links) as LambdaNode;
					fcontext = lambdaNode.ExecutionContext;
					funcAtom = lambdaNode;
					break;

				default:
					throw new InvalidOperationException("Bad function name: "
						+ SequenceFormatter.AsString(funcNode));
			}

			return fcontext;
		}

		private IMLNode CallFunction(FunctionExecutionContext fcontext, IMLNode argList)
		{
			// call func
			if (fcontext.IsCompiled)
			{
				//return fcontext.Method(argList.IsNIL ? null : argList as IListNode, Instance);
				return fcontext.Method(argList.IsNIL ? null : argList as IListNode, this);
			}
			else
			{
				var newLinks = fcontext.CreateLinks(argList);

				// implicit progn:
				return fcontext.Body.IsNIL ?
					fcontext.Body : EvalSequence(fcontext.Body as IListNode, newLinks);
			}
		}
		#endregion
	}

}