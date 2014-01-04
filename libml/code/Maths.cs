using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core;

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
			var numberConverter = context.GetNumberConverter();
			foreach (var item in BNodeWalker.Walk(args))
			{
				var arg = Check4Number(item.Left);
				numberConverter.CompareConvertType(ref result, ref arg);
				result += arg;
			}
			return result;
		}

		[BNodeFunc(Alias = "-", MinimalNumberOfArguments = 1)]
		static IMLNode Minus(IListNode args, IEvaluator context)
		{
			ANumber result = Check4Number(args.Left);
			if (args.Right.IsNIL)
			{
				return -result;
			}
			else
			{
				var numberConverter = context.GetNumberConverter();
				foreach (var item in BNodeWalker.Walk(args).Skip(1))
				{
					var arg = Check4Number(item.Left);
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
			var numberConverter = context.GetNumberConverter();
			foreach (var item in BNodeWalker.Walk(args))
			{
				var arg = Check4Number(item.Left);
				numberConverter.CompareConvertType(ref result, ref arg);
				result *= arg;
			}
			return result;
		}

		[BNodeFunc(Alias = "/", MinimalNumberOfArguments = 1)]
		static IMLNode Divide(IListNode args, IEvaluator context)
		{
			ANumber arg1 = Check4Number(args.Left);
			var numberConverter = context.GetNumberConverter();
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
					var arg = Check4Number(item.Left);
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

		public static ANumber Check4Number(IMLNode node)
		{
			if ((node.NodeType & NodeTypes.Number) == 0)
			{
				throw new NotANumberException(SequenceFormatter.AsString(node));
			}

			return node as ANumber;
		}
	}
}
