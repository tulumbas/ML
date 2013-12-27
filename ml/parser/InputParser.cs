using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.parser
{
	class InputParser
	{
		enum PrimitiveLexemTypes
		{
			// special chars
			LPar,
			RPar,
			Quote,
			Dot,
			Backslash,
			DQuote,

			// general stuff
			Character,
			Digit,

			// breaks
			Whitespace,
			Semicolon,
			EOL
		}

		public List<string> ErrorMessages { get; private set; }
		public List<Token> TokenQueue { get; private set; }

		IEnumerator<char> sourceFeed;
		int parenthesisBalance;

		public void ParseSource(IEnumerable<char> source)
		{
			ResetParser();
			if (source == null)
			{
				return;
			}

			sourceFeed = source.GetEnumerator();
			if (sourceFeed == null)
			{
				return;
			}

			sourceFeed.Reset();
			var flag = sourceFeed.MoveNext();

			while (flag)
			{
				flag = ParseLexem();
			}

			AddToken(TokenType.End);

			if (parenthesisBalance != 0)
			{
				MakeErrorMessage("Unbalanced pars:");
			}
		}

		private bool ParseLexem()
		{
			char c = sourceFeed.Current;
			var t = GetPrimitiveType(c);
			switch (t)
			{
				case PrimitiveLexemTypes.Digit:
					return ReadNumber(c);

				case PrimitiveLexemTypes.Character:
					return ReadWord(c);

				case PrimitiveLexemTypes.DQuote:
					return ReadDQuotedText(c);

				case PrimitiveLexemTypes.LPar:
					AddToken(TokenType.LPar);
					parenthesisBalance++;
					return sourceFeed.MoveNext();

				case PrimitiveLexemTypes.RPar:
					AddToken(TokenType.RPar);
					parenthesisBalance--;
					if (parenthesisBalance < 0)
					{
						MakeErrorMessage("Unbalanced closing pars:");
					}
					return sourceFeed.MoveNext();

				case PrimitiveLexemTypes.Whitespace:
					return ReadWhitespaces();

				case PrimitiveLexemTypes.Quote:
					AddToken(TokenType.Quote);
					return sourceFeed.MoveNext();

				case PrimitiveLexemTypes.Semicolon:
					return sourceFeed.MoveNext();

				default:
					MakeErrorMessage("Can't parse input:");
					return false;
			}
		}

		#region Readers
		private void MakeErrorMessage(string message, string preText = null)
		{
			var sb = new StringBuilder(message);
			sb.Append(" ").Append(preText);
			try
			{
				int len = 10;
				do
				{
					sb.Append(sourceFeed.Current);
				}
				while (--len > 0 && sourceFeed.MoveNext());
			}
			catch { }
			throw new ApplicationException(sb.ToString());
		}

		private bool ReadWhitespaces()
		{
			bool canContinue = sourceFeed.MoveNext();
			while (canContinue)
			{
				var t = GetPrimitiveType(sourceFeed.Current);
				if (t != PrimitiveLexemTypes.Whitespace)
				{
					break;
				}
			}

			AddToken(TokenType.Whitespace);
			return canContinue;
		}

		private bool ReadDQuotedText(char c)
		{
			var sb = new StringBuilder(c.ToString());
			bool canContinue = sourceFeed.MoveNext();
			char prevc;
			while (canContinue)
			{
				prevc = c;
				c = sourceFeed.Current;
				canContinue = sourceFeed.MoveNext();
				sb.Append(c);

				if (c == '"' && prevc != '\\') // closing quote
				{
					break;
				}
			}

			if (sb.Length > 2)
			{
				AddToken(TokenType.TextLiteral, sb.ToString());
			}
			return canContinue;
		}

		private bool ReadWord(char c)
		{
			// if first is minus or plus it can be a number as well
			if (c == '-' || c == '+')
			{
				return ReadPlusMinus(c);
			}

			// in other case - first char is word always
			var sb = new StringBuilder(c.ToString());
			return ContinueReadingWord(sb);
		}

		private bool ReadPlusMinus(char c)
		{
			var canContinue = sourceFeed.MoveNext();
			if (canContinue)
			{
				// store first char and check the following
				var sb = new StringBuilder(c.ToString());

				var t = GetPrimitiveType(sourceFeed.Current);

				// if it's a digit or a char - read number
				if (t == PrimitiveLexemTypes.Digit)
				{
					// read number
					return ContinueReadingNumber(sb);
				}
				else if (t == PrimitiveLexemTypes.Character)
				{
					// read word
					return ContinueReadingWord(sb);
				}
			}

			// any other case - it's a standalone char
			AddToken(TokenType.Word, c.ToString());
			return canContinue;
		}

		private bool ReadNumber(char c)
		{
			var sb = new StringBuilder(c.ToString());
			return ContinueReadingNumber(sb);
		}

		/// <summary>
		/// Char pointed by sourceFeed.Current is already in the builder
		/// Number can't start with dot
		/// </summary>
		/// <param name="sb"></param>
		/// <returns></returns>
		private bool ContinueReadingNumber(StringBuilder sb)
		{
			bool canContinue = sourceFeed.MoveNext();
			bool hasDot = false; // do we have a floating?			
			while (canContinue)
			{
				var c = sourceFeed.Current;
				var t = GetPrimitiveType(c);
				if (t == PrimitiveLexemTypes.Digit) // if still digit, add to number
				{
					sb.Append(c);
				}
				else if (c == '.')
				{
					if (!hasDot)
					{
						hasDot = true;
						sb.Append(c);
					}
					else
					{
						MakeErrorMessage("Can't read number", sb.ToString());
						return false;
					}
				}
				else if (isBreak(t))
				{
					break;
				}
				else
				{
					MakeErrorMessage("Error in number", sb.ToString());
					return false;
				}
				canContinue = sourceFeed.MoveNext();
			}

			if (sb.Length > 0)
			{
				var representation = sb.ToString();
				decimal d;
				if (decimal.TryParse(representation, out d))
				{
					AddToken(TokenType.Number, d, representation);
				}
				else
				{
					MakeErrorMessage("Can't parse number", representation);
				}
			}
			return canContinue;
		}

		private bool isBreak(PrimitiveLexemTypes t)
		{
			switch (t)
			{
				case PrimitiveLexemTypes.LPar:
				case PrimitiveLexemTypes.RPar:
				case PrimitiveLexemTypes.Quote:
				case PrimitiveLexemTypes.DQuote:
				case PrimitiveLexemTypes.Whitespace:
				case PrimitiveLexemTypes.Semicolon:
				case PrimitiveLexemTypes.EOL:
					return true;
				
				default:
					return false;
			}
		}

		private bool ContinueReadingWord(StringBuilder sb)
		{
			bool canContinue = sourceFeed.MoveNext();
			while (canContinue)
			{
				var c = sourceFeed.Current;
				var t = GetPrimitiveType(c);
				if (t == PrimitiveLexemTypes.Character || t == PrimitiveLexemTypes.Digit) // if still char or digit, go further
				{
					sb.Append(c);
				}
				else
				{
					break;
				}
				canContinue = sourceFeed.MoveNext();
			}

			if (sb.Length > 0)
			{
				AddToken(TokenType.Word, sb.ToString());
			}

			return canContinue;
		}
		#endregion

		#region QueueManagement

		private void AddToken(TokenType tokenType, string value = null)
		{
			TokenQueue.Add(new Token { TokenType = tokenType, Value = value });
		}

		private void AddToken(TokenType tokenType, decimal val, string representation)
		{
			TokenQueue.Add(new Token
			{
				TokenType = tokenType,
				NumericValue = val,
				Value = representation
			});
		}

		private void ResetParser()
		{
			TokenQueue = new List<Token>();
			ErrorMessages = new List<string>();
			parenthesisBalance = 0;
		}
		#endregion

		private PrimitiveLexemTypes GetPrimitiveType(char c)
		{
			switch (c)
			{
				case ' ':
				case '\t':
				case '\n':
				case '\r': return PrimitiveLexemTypes.Whitespace;

				case '(': return PrimitiveLexemTypes.LPar;
				case ')': return PrimitiveLexemTypes.RPar;
				case '\'': return PrimitiveLexemTypes.Quote;

				case '"': return PrimitiveLexemTypes.DQuote;
				case '\\': return PrimitiveLexemTypes.Backslash;

				case ';': return PrimitiveLexemTypes.Semicolon;
			}

			if (char.IsWhiteSpace(c)) // Unicode extended
			{
				return PrimitiveLexemTypes.Whitespace;
			}

			if (char.IsDigit(c))
			{
				return PrimitiveLexemTypes.Digit;
			}

			return PrimitiveLexemTypes.Character;
		}
	}
}
