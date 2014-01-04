using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.parser;

namespace ml.core
{
	/*
	 * Simple (straight) logic:
	 *		sequence ::= item [ws+ item]*
	 *		item ::= q-item | expr
	 *		q-item ::= '\'' [ws]* expr
	 *		expr ::= list | atom 
	 *		list ::= '(' [ws]* sequence [ws]* ')'
	 *		atom ::= number | word | quoted-string
	 */

	class SequenceBuilder
	{
		public const string QUOTE = "quote";
		
		TokenConveyer Q;
		public INodeFactory Builder { get; private set; }

		public SequenceBuilder(INodeFactory builder)
		{
			Builder = builder;
		}
		
		public IMLNode Process(List<Token> tokens)
		{
			Q = new TokenConveyer(tokens);
			Q.SkipWs();
			return ProcessSequenceItem();
		}

		/// <summary>
		/// sequence ::= [ws]* item [ws+ item]*
		///  || empty-sequence
		/// </summary>
		/// <returns>NIL if empty sequence</returns>
		private IMLNode ProcessSequence()
		{			
			//IBListNode firstNode = null;
			IListBuilder list = Builder.CreateListBuilder();
			while (!(Q.CurrentType == TokenType.RPar || Q.IsEOF))
			{
				// next item in sequence					
				var child = ProcessSequenceItem();
				list.Append(child);
				Q.SkipWs();
			}
			return list.GetList();
		}

		/// <summary>
		/// item ::= q-item | expr
		/// q-item ::= '\'' [ws]* expr
		/// </summary>
		private IMLNode ProcessSequenceItem()
		{
			IMLNode node;
			if (Q.CurrentType == TokenType.Quote)
			{
				Q.NextSkipWs();
				var quote = Builder.CreateAtom(QUOTE, NodeTypes.Symbol);
				var list = Builder.CreateListBuilder(quote);
				var quotedChild = ProcessSequenceItem();
				list.Append(quotedChild);

				node = list.GetList();
			}
			else
			{
				node = ProcessExpression();
			}

			return node;
		}

		/// <summary>
		/// expr ::= empty-list | list | atom
		/// </summary>
		private IMLNode ProcessExpression()
		{
			if (Q.CurrentType == TokenType.LPar)
			{							
				return ProcessList();
			}
			else
			{
				return ProcessAtom();
			}
		}

		/// <summary>
		/// atom ::= number | word | quoted-string
		/// </summary>
		private IMLNode ProcessAtom()
		{
			IAtom atomNode = null;

			switch (Q.CurrentType)
			{
				case TokenType.FloatPoint:
					atomNode = Builder.CreateAtom(Q.Current.Value, NodeTypes.RealNumber);
					break;

				case TokenType.Integer:
					atomNode = Builder.CreateAtom(Q.Current.Value, NodeTypes.IntegerNumber);
					break;

				case TokenType.Word:
					var value = Q.Current.Value.ToLower();
					if (value == "t")
					{
						atomNode = Builder.GetT();
					}
					else if (value == "nil")
					{
						atomNode = Builder.GetNIL();
					}
					else
					{
						atomNode = Builder.CreateAtom(Q.Current.Value, NodeTypes.Symbol);
					}
					break;

				case TokenType.TextLiteral:
					atomNode = Builder.CreateAtom(Q.Current.Value, NodeTypes.TextLiteral);
					break;

				default:
					throw new BadInputException("Unexpected type of lexem or unexpected end of expression");
			}

			Q.NextSkipWs();
			return atomNode;
		}

		/// <summary>
		/// list ::= empty-list | '(' [ws]* sequence [ws]* ')'
		/// empty-list ::= '(' [ws]* ')'
		/// </summary>
		/// <returns>
		/// created list
		/// </returns>
		private IMLNode ProcessList()
		{
			Assert(TokenType.LPar);
			IMLNode node = ProcessSequence();
			Assert(TokenType.RPar);
			return node;
		}

		//private void ProcessingError(string message)
		//{
		//   throw new BadInputException(message);
		//}

		private void Assert(TokenType type)
		{
			if (Q.CurrentType != type)
			{
				throw new BadInputException(type.ToString() + " expected");
			}
			Q.NextSkipWs(); 
		}
	}
}
