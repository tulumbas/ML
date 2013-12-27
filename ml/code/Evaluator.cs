using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ml.core;

namespace ml.code
{
	class Evaluator<BuilderT> : IEvaluator
		where BuilderT : class, IBNodeBuilder, new()
	{
		public const string LAMBDA = "lambda";

		#region Setup
		public static Evaluator<BuilderT> Instance { get; private set; }

		static Evaluator()
		{
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var type in types)
			{
				if (type.GetCustomAttributes(typeof(FunctionSetsAttribute), true).Any())
				{
					LoadPredefinedFuncsFromType(type);
				}
			}
			Instance = new Evaluator<BuilderT>();
		}

		static void LoadPredefinedFuncsFromType(Type t)
		{
			//functionContext = new Dictionary<string, FunctionExecutionContext>();
			var funcs = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			foreach (var func in funcs)
			{
				var attr = func.GetCustomAttributes(typeof(BNodeFuncAttribute), true);
				if (attr.Length > 0)
				{
					var info = attr[0] as BNodeFuncAttribute;
					var funcContext = new FunctionExecutionContext
					{
						Method =
							Delegate.CreateDelegate(typeof(Func<IBListNode, IEvaluator, IBNode>), func, true)
								as Func<IBListNode, IEvaluator, IBNode>,
						ArgumentQuoting = info.ArgumentQuoting,
						IsCompiled = true,
						NumberOfArguments = info.NumberOfArguments
					};
					//functionContext.Add(context.NormalizedName, context);
					SymbolStorage.Symbols.AddFunction(
						string.IsNullOrEmpty(info.Alias) ? func.Name : info.Alias, 
						funcContext);
				}
			}
		}
		#endregion

		private Evaluator()
		{
			Builder = new BuilderT();
		}

		public IBNodeBuilder Builder { get; private set; }

		public IBNode EvalNode(IBNode arg, Dictionary<string, IBNode> links)
		{
			return arg.IsAtom ? EvalAtom(arg as IBAtom, links) : EvalFunc(arg as IBListNode, links);
		}

		/// <summary>
		/// Implicit progn
		/// </summary>
		/// <param name="args"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		public IBNode EvalSequence(IBListNode args, Dictionary<string, IBNode> links)
		{
			IBNode result = Builder.GetNIL();
			if (args != null)
			{
				foreach (var item in BNodeWalker.Walk(args))
				{
					result = EvalNode(item.Left, links);
				}
			}
			return result;
		}

		private IBNode EvalAtom(IBAtom atom, Dictionary<string, IBNode> localScope)
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
					IBNode value;
					if (localScope != null 
						&& localScope.TryGetValue(SymbolStorage.NormalizeName(atom.Text), out value))
					{
						return value;
					}

					var sc = SymbolStorage.Symbols[atom.Text];
					if (sc == null || sc.Value == null)
					{
						throw new ApplicationException("Variable not defined: " + atom.Text);
					}
					return sc.Value;
					
				default:
					throw new ApplicationException("Node type not supported: " + atom.NodeType);
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
		private IBNode EvalFunc(IBListNode list, Dictionary<string, IBNode> links)
		{
			// checking function name
			var funcNode = list.Left;
			LambdaNode lambdaNode = null;
			IBAtom funcAtom = null;
			FunctionExecutionContext fcontext;

			switch (funcNode.NodeType)
			{
				case NodeTypes.Symbol:
					if (!funcNode.IsAtom)
					{
						throw new ArgumentException("Function name should be atom");
					}
					funcAtom = (funcNode as IBAtom);
					var scontext = SymbolStorage.Symbols[funcAtom.Text];
					if (scontext == null || scontext.Function == null)
					{
						throw new ArgumentException("Unknown function name: " + funcAtom.Text);
					}
					fcontext = scontext.Function;
					break;

				case NodeTypes.Lambda:
					lambdaNode = (funcNode as LambdaNode);
					fcontext = lambdaNode.ExecutionContext;
					break;

				case NodeTypes.List:
					lambdaNode = CheckAndEvalLambda(funcNode as IBListNode, links) as LambdaNode;
					fcontext = lambdaNode.ExecutionContext;
					break;

				default:
					throw new ArgumentException("Bad function name");
			}


			var walker = BNodeWalker.Walk(list);
			var listBuilder = Builder.CreateListBuilder();

			// eval all args
			int argumentsCount = 0;
			var checkNumberOfArguments = fcontext.NumberOfArguments >= 0;
			foreach (var listNode in walker.Skip(1))
			{
				var arg = listNode.Left;
				var evalArg = fcontext.ArgumentQuoting == ArgumentQuotingTypes.None
					|| ((fcontext.ArgumentQuoting == ArgumentQuotingTypes.First) && (argumentsCount > 0));
				if(evalArg)
				{
					arg = EvalNode(arg, links);
				}
				
				listBuilder.Append(arg);
				argumentsCount++;

				if (checkNumberOfArguments && (argumentsCount > fcontext.NumberOfArguments))
				{
					TooManyArguments(funcAtom.Text);
				}
			}

			// check and build arglist
			if (checkNumberOfArguments && (argumentsCount < fcontext.NumberOfArguments))
			{
				TooFewArguments(funcAtom.Text);
			}
			var argList = listBuilder.GetList();

			// call func
			if (fcontext.IsCompiled)
			{
				return fcontext.Method(argList.IsNIL ? null : argList as IBListNode, Instance);
			}
			else if (lambdaNode != null)
			{
				var newLinks = lambdaNode.CreateLinks(argList);
				
				// implicit progn:
				return lambdaNode.Body.IsNIL ? 
					lambdaNode.Body 
					: EvalSequence(lambdaNode.Body as IBListNode, newLinks);
			}
			else
			{
				throw new ApplicationException("Some shit happened");
			}
		}

		private IBNode CheckAndEvalLambda(IBListNode expression, 
			Dictionary<string, IBNode> links)
		{
			if (expression.Left.NodeType != NodeTypes.Symbol
				|| ((IBAtom)expression.Left).Text.ToLowerInvariant() != LAMBDA)
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