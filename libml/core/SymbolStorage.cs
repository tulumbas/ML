using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace ml.core
{
	class SymbolStorage //: ISymbolStorage
	{
		public static SymbolStorage Symbols { get; private set; }

		static ConcurrentDictionary<string, SymbolContext> symbolDictionary;
		static readonly HashSet<string> protectedNames;

		static SymbolStorage()
		{
			protectedNames = new HashSet<string>(new string[]
			{
				NormalizeName("T"),
				NormalizeName("NIL")
			});

			symbolDictionary = new ConcurrentDictionary<string, SymbolContext>();
			Symbols = new SymbolStorage();
		}

		public static string NormalizeName(string name)
		{
			return name.ToLowerInvariant();
		}

		public bool ContainsSymbol(string symbol)
		{
			return symbolDictionary.ContainsKey(symbol);
		}

		public SymbolContext this[string symbol]
		{
			get
			{
				SymbolContext sc;
				var name = NormalizeName(symbol);
				if (symbolDictionary.TryGetValue(name, out sc))
				{
					return sc;
				}
				return null;
			}
		}

		public bool AddSymbol(string symbol)
		{
			return symbolDictionary.TryAdd(NormalizeName(symbol), null);
		}

		public void AddFunction(string symbol, FunctionExecutionContext function)
		{
			symbolDictionary.AddOrUpdate(NormalizeName(symbol), 
				new SymbolContext{ Function = function },
				(k, v) => { if (v == null) { v = new SymbolContext(); } v.Function = function; return v; });
		}

		public void SetValue(string symbol, IMLNode value)
		{
			// check non-redefinable symbols!!! (T,NIL)
			var name = NormalizeName(symbol);
			if (protectedNames.Contains(name))
			{
				throw new InvalidOperationException("Can't redefin protected symbol: " + symbol);
			}

			symbolDictionary.AddOrUpdate(name,
				new SymbolContext { Value = value },
				(k, v) => { if (v == null) { v = new SymbolContext(); } v.Value = value; return v; });
		}

		public void SetProperty(string symbol, string key, object value)
		{
			var name = NormalizeName(symbol);
			SymbolContext sc;
			if (!symbolDictionary.TryGetValue(name, out sc) || sc == null)
			{
				sc = new SymbolContext();
				symbolDictionary.TryAdd(name, sc);
			}

			if(sc.Properties.ContainsKey(key))
			{
				sc.Properties[key] = value;
			}
			else
			{
				sc.Properties.Add(key, value);
			}
		}
	}
}
