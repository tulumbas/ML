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

	class SequenceBuilder<BuilderT>
		where BuilderT : IBNodeBuilder
	{
		public const string QUOTE = "quote";
		
		TokenConveyer Q;
		BuilderT builder;

		public SequenceBuilder(BuilderT builder)
		{
			this.builder = builder;
		}

		
		public IBNode Process(List<Token> tokens)
		{
			if(tokens.Count < 1)
			{
				ProcessingError("Empty token queue");
			}
			Q = new TokenConveyer(tokens);
			return ProcessSequenceItem();
		}

		/// <summary>
		/// sequence ::= [ws]* item [ws+ item]*
		///  || empty-sequence
		/// </summary>
		/// <returns>NIL if empty sequence</returns>
		private IBNode ProcessSequence()
		{
			//IBListNode firstNode = null;
			IBListBuilder list = builder.CreateListBuilder();
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
		private IBNode ProcessSequenceItem()
		{
			IBNode node;
			if (Q.CurrentType == TokenType.Quote)
			{
				Assert(TokenType.Quote, true);

				var quote = builder.CreateAtom(QUOTE, NodeTypes.Symbol);
				var list = builder.CreateListBuilder(quote);
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
		private IBNode ProcessExpression()
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
		private IBNode ProcessAtom()
		{
			IBAtom atomNode = null;

			switch (Q.CurrentType)
			{
				case TokenType.Number:
					atomNode = builder.CreateAtom(Q.Current.Value, NodeTypes.DecimalNumber);
					break;

				case TokenType.Word:
					var value = Q.Current.Value.ToLower();
					if (value == "t")
					{
						atomNode = builder.GetT();
					}
					else if (value == "nil")
					{
						atomNode = builder.GetNIL();
					}
					else
					{
						atomNode = builder.CreateAtom(Q.Current.Value, NodeTypes.Symbol);
					}
					break;

				case TokenType.TextLiteral:
					atomNode = builder.CreateAtom(Q.Current.Value, NodeTypes.TextLiteral);
					break;

				default:
					ProcessingError("Unexpected type of lexem");
					break;
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
		private IBNode ProcessList()
		{
			Assert(TokenType.LPar, true);
			IBNode node = ProcessSequence();
			Assert(TokenType.RPar, true);
			return node;
		}

		private void ProcessingError(string message)
		{
			throw new ApplicationException(message);
		}

		private void Assert(TokenType type, bool skipWs)
		{
			if (Q.CurrentType != type)
			{
				ProcessingError(type.ToString() + " expected");
			}
			if (skipWs) { Q.NextSkipWs(); } else { Q.Next(); }
		}
	}
}
