using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mlrun
{
	class Program
	{
		static void Main(string[] args)
		{
			var eval = new ml.ML();

			var initialStrings = new string[]
			{
				"(defun ! (n) (cond ((= n 1) 1) (t (* n (! (- n 1))))))"
			};

			foreach (var item in initialStrings)
			{
				Console.WriteLine(item);
				var result = eval.EvalAndPrint(item);
				Console.WriteLine(result);
				Console.WriteLine();
			}

			Console.WriteLine("Please type single line expressions");

			do
			{
				Console.WriteLine();
				Console.Write("#: ");
				var line = Console.ReadLine();

				if (line.Trim().ToLowerInvariant() == "(exit)")
				{
					break;
				}

				try
				{
					var result = eval.EvalAndPrint(line);
					Console.WriteLine("Result: " + result);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
				}
			}
			while (true);

			//Console.WriteLine("Press any key");
			//Console.ReadKey();
		}
	}
}
