using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.parser;
using ml.core;
using ml.code;

namespace ml
{
	public class ML
	{
		static InputParser parser;
		SequenceBuilder sequenceBuilder;		
		IEvaluator evaluator;

		static ML()
		{
			// initialization
			parser = new InputParser();
		}

		public ML()
		{
			var upperScope = new SymbolStorage();
			var factory = new NodeFactory(upperScope);
			sequenceBuilder = new SequenceBuilder(factory);
			FunctionLoader.LoadFunctions(upperScope, factory);
			evaluator = new Evaluator(upperScope, factory);
		}

		public IMLNode EvalCommand(string expression)
		{
			var feeder = new ParserFeeder(expression);
			parser.ParseSource(feeder);
			if (parser.TokenQueue.Count > 1) // more then just EOL
			{
				var sequence = sequenceBuilder.Process(parser.TokenQueue);
				return evaluator.EvalNode(sequence, null);
			}
			return null;
		}

		public string EvalAndPrint(string expression)
		{
			var result = EvalCommand(expression);
			return SequenceFormatter.AsString(result);
		}
	}
}
