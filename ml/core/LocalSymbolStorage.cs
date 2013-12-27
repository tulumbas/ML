using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
{
	class LocalSymbolStorage: ISymbolStorage
	{
		Dictionary<string, SymbolContext> symbols = new Dictionary<string, SymbolContext>();

		private void AddOrUpdateSymbolContext(string symbol, Action<SymbolContext> update)
		{
			var key = SymbolStorage.NormalizeName(symbol);

			SymbolContext sc;
			if (!symbols.TryGetValue(key, out sc))
			{
				sc = new SymbolContext();
				symbols.Add(key, sc);
			}

			if (sc == null)
			{
				sc = new SymbolContext();
				symbols[key] = sc;
			}

			update(sc);
		}

		public void AddFunction(string symbol, FunctionExecutionContext function)
		{
			AddOrUpdateSymbolContext(symbol, (sc) => sc.Function = function);
		}

		public bool AddSymbol(string symbol)
		{
			var key = SymbolStorage.NormalizeName(symbol);
			if (!symbols.ContainsKey(key))
			{
				symbols.Add(key, null);
				return true;
			}
			return false;
		}

		public bool ContainsSymbol(string symbol)
		{
			var key = SymbolStorage.NormalizeName(symbol);
			return symbols.ContainsKey(key);
		}

		public void SetProperty(string symbol, string key, object value)
		{
			throw new NotImplementedException();
		}

		public void SetValue(string symbol, IBNode value)
		{
			AddOrUpdateSymbolContext(symbol, (sc) => sc.Value = value);
		}

		public SymbolContext this[string symbol]
		{
			get 
			{
				SymbolContext sc;
				if (!symbols.TryGetValue(SymbolStorage.NormalizeName(symbol), out sc))
				{
					return null;
				}
				return sc;
			}
		}
	}
}
