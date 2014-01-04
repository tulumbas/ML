using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.core
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
			return (op1 != null) && op1.IsEqualTo(op2);
		}

		public static bool operator !=(ANumber op1, ANumber op2)
		{
			return (op1 == null) || !op1.IsEqualTo(op2);
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
	}

	abstract class OrderableNumber: ANumber
	{
		public abstract bool IsGreater(ANumber arg);

		public static bool operator >(OrderableNumber A, OrderableNumber B)
		{
			return A.IsGreater(B);
		}

		public static bool operator <(OrderableNumber A, OrderableNumber B)
		{
			return B.IsGreater(A);
		}

		public static bool operator >=(OrderableNumber A, OrderableNumber B)
		{
			return A.IsGreater(B) || A.IsEqualTo(B);
		}

		public static bool operator <=(OrderableNumber A, OrderableNumber B)
		{
			return B.IsGreater(A) || A.IsEqualTo(B);
		}

	}
}
