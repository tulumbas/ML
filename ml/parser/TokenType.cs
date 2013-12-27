using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.parser
{
	enum TokenType
	{
		End,
		LPar,
		RPar,
		Whitespace,
		Word, // atom made of chars
		Quote,
		TextLiteral, // d-quoted text
		Number,
	}
}
