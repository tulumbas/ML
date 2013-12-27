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
			var strings = new string[] 			
			{
				"(cond (() ()) ( t ()))",
				"(defun zz (a b c) (cons c (cons (car a) (cdr b))))",
				"(zz '(qq ww ee) '(rr tt yy) '(uu ii oo))"
			};

			var eval = new ml.ML();
			
			foreach (var line in strings)
			{
				Console.WriteLine("Source: "+ line);
				try
				{
					var result = eval.EvalCommand(line);
					Console.Write("Result: ");
					Console.WriteLine(ml.SequenceFormatter.AsString(result));
					Console.WriteLine();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
				}
			}
			Console.WriteLine("Press any key");
			Console.ReadKey();
		}
	}
}
