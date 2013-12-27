using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.parser;

namespace ml.core
{
	class TokenConveyer
	{
		int currentIndex;
		private List<Token> tokens;

		public bool IsEOF
		{
			get
			{
				return tokens[currentIndex].TokenType == TokenType.End;
			}
		}
	
		public Token Current
		{
			get
			{
				return tokens[currentIndex];
			}
		}

		public TokenType CurrentType
		{
			get
			{
				return tokens[currentIndex].TokenType;
			}
		}

		public TokenConveyer(List<Token> tokens)
		{
			this.tokens = tokens;
		}

		public void Reset()
		{
			currentIndex = 0;		
		}

		public void SkipWs()
		{
			while (CurrentType == TokenType.Whitespace)
			{
				currentIndex++;
			}
		}

		/// <summary>
		/// shift to next token
		/// </summary>
		/// <returns></returns>
		public void Next()
		{
			if (!IsEOF)
			{
				currentIndex++;
			}
		}

		/// <summary>
		/// shift to next token.
		/// if next token is WS, skip them
		/// </summary>
		/// <returns></returns>
		public void NextSkipWs()
		{
			Next();
			SkipWs();
		}
	}
}
