﻿using System;
namespace ml
{
	[Flags]
	public enum NodeTypes
	{
		List = 1,
		NIL = 2,
		T = 4,
		Symbol = 0x10,
		TextLiteral = 0x100,

		Number = 0x200,
		RealNumber = 0x600,
		IntegerNumber = 0xa00,
		
		Lambda = 0x8000,
	}
}
