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
		//public static INodeFactory Builder { get; set; }
		
		public IMLNode Process(List<Token> tokens)
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
		private IMLNode ProcessSequence()
		{			
			//IBListNode firstNode = null;
			IListBuilder list = DependencyResolver.Builder.CreateListBuilder();
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
				Assert(TokenType.Quote, true);

				var quote = DependencyResolver.Builder.CreateAtom(QUOTE, NodeTypes.Symbol);
				var list = DependencyResolver.Builder.CreateListBuilder(quote);
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
				case TokenType.Number:
					atomNode = DependencyResolver.Builder.CreateAtom(Q.Current.Value, NodeTypes.DecimalNumber);
					break;

				case TokenType.Word:
					var value = Q.Current.Value.ToLower();
					if (value == "t")
					{
						atomNode = DependencyResolver.Builder.GetT();
					}
					else if (value == "nil")
					{
						atomNode = DependencyResolver.Builder.GetNIL();
					}
					else
					{
						atomNode = DependencyResolver.Builder.CreateAtom(Q.Current.Value, NodeTypes.Symbol);
					}
					break;

				case TokenType.TextLiteral:
					atomNode = DependencyResolver.Builder.CreateAtom(Q.Current.Value, NodeTypes.TextLiteral);
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
		private IMLNode ProcessList()
		{
			Assert(TokenType.LPar, true);
			IMLNode node = ProcessSequence();
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
