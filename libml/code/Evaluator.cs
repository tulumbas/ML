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
		static readonly IMLNode NILSTATIC;

		#region Setup
		public static Evaluator Instance { get; private set; }

		static Evaluator()
		{
			NILSTATIC = DependencyResolver.Builder.GetNIL();		
			Instance = new Evaluator();
		}

		#endregion

		#region IEvaluator

		public IMLNode NIL { get { return NILSTATIC; } }

		public INodeFactory Builder { get { return DependencyResolver.Builder; } }

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
			IMLNode result = NILSTATIC;
			if (args != null)
			{
				foreach (var item in BNodeWalker.Walk(args))
				{
					result = EvalNode(item.Left, links);
				}
			}
			return result;
		}

		#endregion

		#region Calcualtions

		private IMLNode EvalAtom(IAtom atom, Dictionary<string, IMLNode> localScope)
		{
			switch (atom.NodeType)
			{
				case NodeTypes.NIL: 					
				case NodeTypes.T:
				case NodeTypes.DecimalNumber:
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

					var sc = SymbolStorage.Symbols[atom.Text];
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
		private IMLNode EvalFunc(IListNode list, Dictionary<string, IMLNode> links)
		{
			// checking function name
			FunctionExecutionContext fcontext;
			string funcName;

			switch (list.Left.NodeType)
			{
				case NodeTypes.Symbol:
					var funcAtom = (list.Left as IAtom);
					funcName = funcAtom.Text;
					var scontext = SymbolStorage.Symbols[funcName];
					if (scontext == null || scontext.Function == null)
					{
						throw new ArgumentException("Unknown function name: " + funcName);
					}
					fcontext = scontext.Function;
					break;

/* does not correspond to CommonLisp
				case NodeTypes.Lambda:
					lambdaNode = (funcNode as LambdaNode);
					fcontext = lambdaNode.ExecutionContext;
					break;
*/
				case NodeTypes.List:
					var lambdaNode = CheckAndEvalLambda(list.Left as IListNode, links) as LambdaNode;
					fcontext = lambdaNode.ExecutionContext;
					funcName = lambdaNode.Text;
					break;

				default:
					throw new InvalidOperationException("Bad function name: " 
						+ SequenceFormatter.AsString(list.Left));
			}


			// eval all args
			int argumentsCount = 0;
			var checkNumberOfArguments = fcontext.NumberOfArguments >= 0;
			IMLNode argList;
			
			var walker = BNodeWalker.Walk(list);
			if (fcontext.ArgumentQuoting == ArgumentQuotingTypes.All)
			{
				argList = list.Right;
				argumentsCount = walker.Skip(1).Count();
			}
			else
			{
				var listBuilder = DependencyResolver.Builder.CreateListBuilder();
				foreach (var listNode in walker.Skip(1))
				{
					var arg = listNode.Left;
					var evalArg = fcontext.ArgumentQuoting == ArgumentQuotingTypes.None
						|| ((fcontext.ArgumentQuoting == ArgumentQuotingTypes.First) && (argumentsCount > 0));
					if (evalArg)
					{
						arg = EvalNode(arg, links);
					}

					listBuilder.Append(arg);
					argumentsCount++;

					if (checkNumberOfArguments && (argumentsCount > fcontext.NumberOfArguments))
					{
						TooManyArguments(funcName);
					}
				}
				argList = listBuilder.GetList();
			}

			// check and build arglist
			if (checkNumberOfArguments && (argumentsCount < fcontext.NumberOfArguments))
			{
				TooFewArguments(funcName);
			}

			// call func
			if (fcontext.IsCompiled)
			{
				return fcontext.Method(argList.IsNIL ? null : argList as IListNode, Instance);
			}
			else 
			{
				var newLinks = fcontext.CreateLinks(argList);
				
				// implicit progn:
				return fcontext.Body.IsNIL ? 
					fcontext.Body : EvalSequence(fcontext.Body as IListNode, newLinks);
			}
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

		#endregion
	}

}