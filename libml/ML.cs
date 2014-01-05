using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ml.parser;
using ml.core;
using ml.code;
using System.Threading;

namespace ml
{
	public class ML
	{		
		static InputParser parser;
		SequenceBuilder sequenceBuilder;		
		IEvaluator evaluator;
		AutoResetEvent evt;
		IMLNode evaluationResult;

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
			var th = new Thread(this.EvalCommandInThread, 10 * 1024 * 1024);
			evt = new AutoResetEvent(false);
			th.Start(expression);
			evt.WaitOne();
			GC.Collect();
			return evaluationResult;
		}

		private void EvalCommandInThread(object command)
		{
			evaluationResult = null;
			var expression = command as string;
			var feeder = new ParserFeeder(expression);
			parser.ParseSource(feeder);
			if (parser.TokenQueue.Count > 1) // more then just EOL
			{
				var sequence = sequenceBuilder.Process(parser.TokenQueue);
				evaluationResult = evaluator.EvalNode(sequence);
			}
			evt.Set();
		}

		public string EvalAndPrint(string expression)
		{
			var result = EvalCommand(expression);
			var output = SequenceFormatter.AsString(result);			
			return output;
		}
	}
}
