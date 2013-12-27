using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.parser;
using ml.core;
using ml.code;

namespace ml
{
	public static class ML
	{
		static InputParser parser;
		static SequenceBuilder sequenceBuilder;

		static ML()
		{
			// initialization
			parser = new InputParser();
			sequenceBuilder = new SequenceBuilder();
			FunctionLoader.LoadFunctions();
		}

		public static IMLNode EvalCommand(string expression)
		{
			var feeder = new ParserFeeder(expression);
			parser.ParseSource(feeder);
			if (parser.TokenQueue.Count > 1) // more then just EOL
			{
				var sequence = sequenceBuilder.Process(parser.TokenQueue);
				return Evaluator.Instance.EvalNode(sequence, null);
			}
			return null;
		}

		public static string EvalAndPrint(string expression)
		{
			var result = EvalCommand(expression);
			return SequenceFormatter.AsString(result);
		}
	}
}
