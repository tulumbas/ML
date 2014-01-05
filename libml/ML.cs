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
		Exception error;
		int stackSize;

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

		public ML(int stackSizeInMegs)
			:this()
		{
			this.stackSize = stackSizeInMegs * 1024 * 1024;
		}

		public IMLNode EvalCommand(string expression)
		{
			var thread = stackSize > 0
				? new Thread(this.EvalCommandInThread, stackSize)
				: new Thread(this.EvalCommandInThread);

			evt = new AutoResetEvent(false);
			thread.Start(expression);
			evt.WaitOne();

			if (evaluationResult == null)
			{
				if (error != null)
				{
					throw error;
				}

				throw new InvalidOperationException("Calculation outcome is null");
			}

			GC.Collect();
			return evaluationResult;
		}

		private void EvalCommandInThread(object command)
		{
			evaluationResult = null;
			try
			{
				var expression = command as string;
				var feeder = new ParserFeeder(expression);
				parser.ParseSource(feeder);
				if (parser.TokenQueue.Count > 1) // more then just EOL
				{
					var sequence = sequenceBuilder.Process(parser.TokenQueue);
					evaluationResult = evaluator.EvalNode(sequence);
				}
			}
			catch (Exception ex)
			{
				error = ex;
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
