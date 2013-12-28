using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ml.core;

namespace ml.code
{
	class FunctionLoader
	{
		public static void LoadFunctions(ISymbolStorage scope, INodeFactory builder)
		{
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var type in types)
			{
				var attrs = type.GetCustomAttributes(typeof(FunctionSetsAttribute), true);
				if (attrs.Any())
				{
					LoadPredefinedFuncsFromType(type, scope);
					var setupMethod = (attrs[0] as FunctionSetsAttribute).SetupMethod;
					if (!string.IsNullOrEmpty(setupMethod))
					{
						var mi = type.GetMethod(setupMethod);
						if (mi != null)
						{
							var action = Delegate.CreateDelegate(typeof(Action<ISymbolStorage, INodeFactory>), mi, true)
								as Action<ISymbolStorage, INodeFactory>;
							action(scope, builder);
						}
					}
				}
			}
		}

		static void LoadPredefinedFuncsFromType(Type t, ISymbolStorage scope)
		{
			//functionContext = new Dictionary<string, FunctionExecutionContext>();
			var funcs = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			foreach (var func in funcs)
			{
				var attr = func.GetCustomAttributes(typeof(BNodeFuncAttribute), true);
				if (attr.Length > 0)
				{
					var info = attr[0] as BNodeFuncAttribute;
					var funcContext = FunctionExecutionContext.CreateCompiled(
							Delegate.CreateDelegate(typeof(Func<IListNode, IEvaluator, IMLNode>), func, true)
								as Func<IListNode, IEvaluator, IMLNode>);
					funcContext.ArgumentQuoting = info.ArgumentQuoting;
					funcContext.NumberOfArguments = info.NumberOfArguments;
					funcContext.MinimalNumberOfArguments = info.MinimalNumberOfArguments;
					//functionContext.Add(context.NormalizedName, context);
					scope.AddFunction(
						string.IsNullOrEmpty(info.Alias) ? func.Name : info.Alias,
						funcContext);
				}
			}
		}
	}
}
