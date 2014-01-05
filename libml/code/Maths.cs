using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;
using ml.core.numbers;

namespace ml.code
{
	/// <summary>
	/// Upon basic arith operations numbers should be converted to highest 
	/// numerical hierarchy members presenting in expresions
	/// int -> rational -> real -> complex (based on real)
	/// 1234 -> 1234/1 -> 1234.0 -> 1234.0 + 0 i
	/// </summary>
	[FunctionSets]
	class Maths
	{
		[BNodeFunc(Alias = "+")]
		static IMLNode Add(IListNode args, IEvaluator context)
		{
			ANumber result = BigNum.Zero;
			var numberConverter = ANumber.GetNumberConverter();
			foreach (var item in BNodeWalker.Walk(args))
			{
				var arg = ANumber.Check4Number(item.Left);
				numberConverter.CompareConvertType(ref result, ref arg);
				result += arg;
			}
			return result;
		}

		[BNodeFunc(Alias = "-", MinimalNumberOfArguments = 1)]
		static IMLNode Minus(IListNode args, IEvaluator context)
		{
			ANumber result = ANumber.Check4Number(args.Left);
			if (args.Right.IsNIL)
			{
				return -result;
			}
			else
			{
				var numberConverter = ANumber.GetNumberConverter();
				foreach (var item in BNodeWalker.Walk(args).Skip(1))
				{
					var arg = ANumber.Check4Number(item.Left);
					numberConverter.CompareConvertType(ref result, ref arg);
					result -= arg;
				}
			}
			return result;
		}

		[BNodeFunc(Alias = "*")]
		static IMLNode Multiply(IListNode args, IEvaluator context)
		{
			ANumber result = BigNum.One;
			var numberConverter = ANumber.GetNumberConverter();
			foreach (var item in BNodeWalker.Walk(args))
			{
				var arg = ANumber.Check4Number(item.Left);
				numberConverter.CompareConvertType(ref result, ref arg);
				result *= arg;
			}
			return result;
		}

		[BNodeFunc(Alias = "/", MinimalNumberOfArguments = 1)]
		static IMLNode Divide(IListNode args, IEvaluator context)
		{
			ANumber arg1 = ANumber.Check4Number(args.Left);
			var numberConverter = ANumber.GetNumberConverter();
			if (args.Right.IsNIL)
			{
				ANumber d = BigNum.One;
				numberConverter.CompareConvertType(ref d, ref arg1);
				return d.Divide(arg1);
			}
			else
			{
				foreach (var item in BNodeWalker.Walk(args).Skip(1))
				{
					var arg = ANumber.Check4Number(item.Left);
					numberConverter.CompareConvertType(ref arg1, ref arg);
					arg1 /= arg;
				}
			}
			return arg1;
		}

		[BNodeFunc(NumberOfArguments = 2)]
		static IMLNode Mod(IListNode args, IEvaluator context)
		{
			throw new NotImplementedException();
		}

		[BNodeFunc(MinimalNumberOfArguments = 2, Alias = "=")]
		static IMLNode NumberIsEqualTo(IListNode args, IEvaluator code)
		{
			ANumber arg1 = ANumber.Check4Number(args.Left);
			foreach (var node in BNodeWalker.Walk(args).Skip(1))
			{
				var arg = ANumber.Check4Number(node.Left);
				if (arg1 != arg)
				{
					return code.NIL;
				}
			}
			return code.Builder.GetT();
		}


		[BNodeFunc(MinimalNumberOfArguments = 2, Alias = "!=")]
		static IMLNode NumberIsNotEqualTo(IListNode args, IEvaluator code)
		{
			var result = NumberIsEqualTo(args, code);
			return result.IsNIL ? code.Builder.GetT() : code.NIL;
		}

	}
}
