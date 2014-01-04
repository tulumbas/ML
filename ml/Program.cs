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

			Console.WriteLine(eval.EvalAndPrint("(* 1000000 1000000)"));			

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
