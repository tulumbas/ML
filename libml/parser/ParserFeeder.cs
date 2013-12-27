using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ml.parser
{
	class ParserFeeder: IEnumerable<char>
	{
		string text;

		public ParserFeeder(string text)
		{
			this.text = text;
		}

		public IEnumerator<char> GetEnumerator()
		{
			return text.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return text.GetEnumerator();
		}
	}
}
