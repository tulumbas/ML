using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core.numbers
{
	class BigNum: OrderableNumber
	{
		const int GIG = 1000000000;
		const int M100 = 100000000;

		#region Properties
		readonly int sign;
		readonly int[] nonets;
		readonly int length;

		public int Sign { get { return sign; } }
		public int[] Nonets { get { return nonets; } }
		public int Length { get { return length; } }
		#endregion

		#region Setup
		public static readonly BigNum Zero = new BigNum();
		public static readonly BigNum One = new BigNum(new int[] { 1 }, 1, 1);

		private BigNum(int[] newNonets, int newLength, int newSign)
		{
			base.NodeType = NodeTypes.IntegerNumber;
			nonets = newNonets;
			length = newLength;
			sign = newSign;
		}

		public BigNum()
		{
			base.NodeType = NodeTypes.IntegerNumber;
			sign = 0;
			nonets = new int[] { 0 };
			length = 1;
		}

		public static BigNum CreateBigNum(string digitString)
		{
			bool isNeg;
			int lastIndex;
			var list = SplitDigitString(digitString, out isNeg, out lastIndex);
			int sign = 1;
			// if only 1 nonnet check number is zero
			if (lastIndex == 0 && list[0] == 0)
			{
				return Zero;
			}
			else if(isNeg)
			{
				sign = -1;
			}

			var length = lastIndex + 1;
			var nonets = list.Take(length).ToArray();
			return new BigNum(nonets, length, sign);
		}
		#endregion

		#region ANumber implmentation
		public override ANumber Add(ANumber arg1)
		{
			//var arg = GetBigNum(arg1);
			var arg = arg1 as BigNum;
			var s = arg.sign;

			if (s == 0)
			{
				return this;
			}

			if (sign == 0)
			{
				return arg;
			}

			var r = sign * s;
			if (r > 0)
			{
				return AddInternalIgnoreSign(this, arg);
			}
			else
			{
				return SubtractInternalIgnoreSign(this, arg);
			}
		}

		public override ANumber Subtract(ANumber arg1)
		{
			//var arg = GetBigNum(arg1);
			var arg = arg1 as BigNum;
			var s = arg.sign;

			if (s == 0)
			{
				return this;
			}

			if (sign == 0)
			{
				return arg.Negate();
			}

			var r = sign * s;
			if (r < 0)
			{
				return AddInternalIgnoreSign(this, arg);
			}
			else
			{
				return SubtractInternalIgnoreSign(this, arg);
			}
		}

		public override ANumber Multiply(ANumber arg1)
		{
			var arg = GetBigNum(arg1);
			if (sign == 0 || arg.sign == 0)
			{
				return Zero;
			}
			return Multiply2(this, arg);
		}

		public override ANumber Divide(ANumber arg1)
		{
			throw new NotImplementedException();
		}

		public override ANumber Negate()
		{
			return sign == 0 ? Zero : new BigNum(nonets, length, -sign);
		}

		public override string PrintNumber()
		{
			if (sign == 0)
			{
				return "0";
			}

			var sb = new StringBuilder(sign < 0 ? "-" : "");
			sb.Append(nonets[length - 1]);
			for (int i = length - 2; i >= 0; i--)
			{
				sb.AppendFormat("{0:d9}", nonets[i]);
			}
			return sb.ToString();
		}

		protected override bool IsEqualTo(ANumber arg)
		{
			if (arg.NodeType == NodeTypes.IntegerNumber)
			{
				var bn = arg as BigNum;
				if (bn.length == length)
				{
					for (int i = 0; i < length; i++)
					{
						if (nonets[i] != bn.nonets[i])
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public override bool IsGreater(ANumber arg)
		{
			return IsGreater(this, arg as BigNum);
		}

		#endregion

		#region Arithmetics

		private static int Add2Nonets(int sum, ref int shift)
		{
			if (sum >= GIG)
			{
				shift = 1;
				sum -= GIG;
			}
			else
			{
				shift = 0;
			}
			return sum;
		}

		private static BigNum Multiply2(BigNum b1, BigNum b2)
		{
			var result = new int[b1.length + b2.length];
			for (int i = 0; i < b1.length; i++)
			{
				for (int j = 0; j < b2.length; j++)
				{
					long l = (long)b1.nonets[i] * (long)b2.nonets[j];
					long shift = (int)(l / GIG);
					result[i + j] += (int)(l - shift * GIG);
					result[i + j + 1] += (int)shift;
				}
			}

			var lastIndex = GetBiggestNonzeroNonnet(result);
			return new BigNum(result.Take(lastIndex+1).ToArray(), lastIndex + 1, b1.sign * b2.sign);
		}

		private static BigNum AddInternalIgnoreSign(BigNum b1, BigNum b2)
		{
			if (b1.length < b2.length)
			{
				return AddInternalIgnoreSign(b2, b1);
			}

			int i;
			int shift = 0;
			var result = new int[b1.length + 1];
			var resultLength = b1.length;

			for (i = 0; i < b2.length; i++)
			{
				result[i] = Add2Nonets(b1.nonets[i] + b2.nonets[i] + shift, ref shift);
			}
			for (; i < b1.length; i++)
			{
				result[i] = Add2Nonets(b1.nonets[i] + shift, ref shift);				
			}
			if (shift > 0)
			{
				resultLength++;
				result[b1.length] = 1;
			}

			return new BigNum(result, resultLength, b1.sign);
		}

		private static BigNum SubtractInternalIgnoreSign(BigNum b1, BigNum b2)
		{
			if (b1.length == b2.length)
			{
				return SubtractInternalSameLength(b1, b2);
			}
			else if (b1.length < b2.length)
			{				
				return SubtractInternalDifferentLength(b2, b1, true);
			}
			else
			{
				return SubtractInternalDifferentLength(b1, b2, false);
			}
		}

		// same length
		private static BigNum SubtractInternalSameLength(BigNum b1, BigNum b2)
		{
			int i, shift = 0, shift2 = 0;
			int[] result1 = new int[b1.length], result2 = new int[b1.length];
			var sign = b1.sign;

			for (i = 0; i < b1.length; i++)
			{
				result1[i] = Subtract2Nonets(b1.nonets[i] - b2.nonets[i] - shift, ref shift);
				result2[i] = Subtract2Nonets(b2.nonets[i] - b1.nonets[i] - shift2, ref shift2);
			}

			if (shift > 0)
			{
				result1 = result2; sign = (sbyte)-sign;
			}

			var lastIndex = GetBiggestNonzeroNonnet(result1);
			return lastIndex == 0 && result1[0] == 0 ? Zero : new BigNum(result1, lastIndex + 1, sign);
		}

		private static BigNum SubtractInternalDifferentLength(BigNum b1, BigNum b2, bool changeSign)
		{
			// I'm sure b1 is longer than b1 so no augment is possible
			int i, shift = 0;
			var result = new int[b1.length];
			var resultLength = b1.length;
			int lastIndex = 0;

			for (i = 0; i < b2.length; i++)
			{
				result[i] = Subtract2Nonets(b1.nonets[i] - b2.nonets[i] - shift, ref shift);
				if (result[i] > 0)
				{
					lastIndex = i;
				}
			}
			for (; i < b1.length; i++)
			{
				result[i] = Subtract2Nonets(b1.nonets[i] - shift, ref shift);
				if (result[i] > 0)
				{
					lastIndex = i;
				}
			}

			return lastIndex == 0 && result[0] == 0 ? Zero 
				: new BigNum(result, lastIndex + 1, changeSign ? (sbyte)-b1.sign : b1.sign);
		}

		private static int Subtract2Nonets(int diff, ref int shift)
		{
			if (diff < 0)
			{
				shift = +1;
				diff += GIG;
			}
			else
			{
				shift = 0;
			}
			return diff;
		}
		#endregion

		private static bool IsGreater(BigNum b1, BigNum b2)
		{
			if (b1.sign != b2.sign) return b1.sign > b2.sign;

			var isPositive = b1.sign == 1;
			if (b1.length > b2.length) return isPositive;
			if (b1.length < b2.length) return !isPositive;
			for (int i = b1.length - 1; i >= 0; i--)
			{				
				if (b1.nonets[i] > b2.nonets[i]) return isPositive;
				if (b1.nonets[i] < b2.nonets[i]) return !isPositive;
			}
			return false;
		}

		private static BigNum GetBigNum(ANumber arg)
		{
			if (arg.NodeType != NodeTypes.IntegerNumber)
			{
				throw new NumberConvertException("Number should be converted to BigNum: " + arg.Text);
			}
			return arg as BigNum;
		}

		private static int GetBiggestNonzeroNonnet(int[] nonets)
		{
			int lastIndex = 0;
			for (int i = nonets.Length - 1; i > 0; i--)
			{
				if (nonets[i] != 0)
				{
					lastIndex = i;
					break;
				}
			}
			return lastIndex;
		}

		private static List<int> SplitDigitString(string digitString, 
			out bool isNeg, out int lastNonZeroIndex)
		{
			var digLength = digitString.Length;
			var beginning = 0;
			isNeg = false;
			if (!Char.IsDigit(digitString[0]))
			{
				beginning = 1;
				if (digitString[0] == '-')
				{
					isNeg = true;
				}
			}

			var list = new List<int>();
			int idx = digLength - 9, k = 0;
			lastNonZeroIndex = 0;
			while (idx > beginning)
			{
				var nonetString = digitString.Substring(idx, 9);
				var nonet = int.Parse(nonetString);
				list.Add(nonet);
				if (nonet != 0)
				{
					lastNonZeroIndex = k;
				}
				k++;
				idx -= 9;
			}
			{
				var nonetString = digitString.Substring(beginning, 9 + idx - beginning);
				var nonet = int.Parse(nonetString);
				if (nonet != 0)
				{
					lastNonZeroIndex = k;
				}
				list.Add(nonet);
			}
			return list;
		}
	}

}
