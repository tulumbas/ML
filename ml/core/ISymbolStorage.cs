using System;
namespace ml.core
{
	interface ISymbolStorage
	{
		void AddFunction(string symbol, FunctionExecutionContext function);
		bool AddSymbol(string symbol);
		bool ContainsSymbol(string symbol);
		void SetProperty(string symbol, string key, object value);
		void SetValue(string symbol, IBNode value);
		SymbolContext this[string symbol] { get; }
	}
}
