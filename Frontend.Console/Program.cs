using Backend;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

string[] examples =
{
    "4*5/(4+6)",
    "144^(1/2)",
    "(3.1416+70)/(300^(1/3.2))",
    "12.5*4+3.75",
    "2^3^2"
};

foreach (string expression in examples)
{
    Console.WriteLine($"{expression} = {ExpressionEvaluator.Evaluate(expression):G15}");
}
