using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.core.numbers;

namespace ml.core.numbers
{
	abstract class ANumber : IAtom
	{
		#region IAtom
		public NodeTypes NodeType { get; protected set; }
		public bool IsAtom { get { return true; } }
		public bool IsNIL { get { return false; } }
		public virtual string Text { get { return PrintNumber(); } }
		#endregion

		#region Operations
		public abstract ANumber Add(ANumber arg1);
		public abstract ANumber Subtract(ANumber arg1);
		public abstract ANumber Multiply(ANumber arg1);
		public abstract ANumber Divide(ANumber arg1);
		public abstract ANumber Negate();
		public abstract string PrintNumber();

		protected virtual bool IsEqualTo(ANumber arg)
		{
			if (arg.NodeType != NodeType) return false; 
			return Text == arg.Text;
		}

		public override bool Equals(object obj)
		{
			var arg = obj as ANumber;
			if (arg == null) return false;			
			return IsEqualTo(arg);
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode();
		}

		public static bool operator ==(ANumber op1, ANumber op2)
		{
			return !object.ReferenceEquals(op1, null) && op1.IsEqualTo(op2);
		}

		public static bool operator !=(ANumber op1, ANumber op2)
		{
			return object.ReferenceEquals(op1, null) || !op1.IsEqualTo(op2);
		}

		public static ANumber operator +(ANumber A, ANumber B)
		{
			return A.Add(B);
		}

		public static ANumber operator -(ANumber A)
		{
			return A.Negate();
		}

		public static ANumber operator -(ANumber A, ANumber B)
		{
			return A.Subtract(B);
		}

		public static ANumber operator *(ANumber A, ANumber B)
		{
			return A.Multiply(B);
		}

		public static ANumber operator /(ANumber A, ANumber B)
		{
			return A.Divide(B);
		}
		#endregion

		public static ANumber Check4Number(IMLNode node)
		{
			if ((node.NodeType & NodeTypes.Number) == 0)
			{
				throw new NotANumberException(SequenceFormatter.AsString(node));
			}

			return node as ANumber;
		}

		public static NumberConverter GetNumberConverter()
		{
			return new NumberConverter();
		}
	}

}
