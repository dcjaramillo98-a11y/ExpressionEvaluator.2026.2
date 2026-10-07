using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evaluate(string infix)
    {
        if (string.IsNullOrWhiteSpace(infix))
            throw new ArgumentException("Ingrese una expresión.");

        return EvaluatePostfix(ToPostfix(infix));
    }

    // Se conserva el nombre usado por el código original del docente.
    public static double Evalute(string infix) => Evaluate(infix);

    private static List<string> ToPostfix(string infix)
    {
        var output = new List<string>();
        var operators = new Stack<string>();
        int i = 0;
        bool expectsNumber = true;

        while (i < infix.Length)
        {
            char c = infix[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                int start = i;
                bool decimalPoint = false;

                while (i < infix.Length &&
                       (char.IsDigit(infix[i]) || infix[i] == '.'))
                {
                    if (infix[i] == '.')
                    {
                        if (decimalPoint)
                            throw new ArgumentException("Número decimal inválido.");
                        decimalPoint = true;
                    }
                    i++;
                }

                string number = infix[start..i];
                if (number == "." ||
                    !double.TryParse(number, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException($"Número inválido: {number}");

                output.Add(number);
                expectsNumber = false;
                continue;
            }

            if (c == '(')
            {
                operators.Push("(");
                expectsNumber = true;
                i++;
                continue;
            }

            if (c == ')')
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                    output.Add(operators.Pop());

                if (operators.Count == 0)
                    throw new ArgumentException("Hay paréntesis sin pareja.");

                operators.Pop();
                expectsNumber = false;
                i++;
                continue;
            }

            if ("+-*/^".Contains(c))
            {
                string op = c.ToString();

                // Admite signos unarios, por ejemplo -5 o 3*(-2).
                if (expectsNumber && (c == '+' || c == '-'))
                    op = c == '-' ? "u-" : "u+";
                else if (expectsNumber)
                    throw new ArgumentException($"Falta un número antes de '{c}'.");

                // Un signo unario se aplica al número siguiente; no debe sacar
                // una potencia pendiente, como en 2^-2.
                if (op is not ("u-" or "u+"))
                {
                    while (operators.Count > 0 && operators.Peek() != "(" &&
                           (Precedence(operators.Peek()) > Precedence(op) ||
                            (Precedence(operators.Peek()) == Precedence(op) &&
                             !IsRightAssociative(op))))
                    {
                        output.Add(operators.Pop());
                    }
                }

                operators.Push(op);
                expectsNumber = true;
                i++;
                continue;
            }

            throw new ArgumentException($"Carácter no válido: '{c}'.");
        }

        if (expectsNumber)
            throw new ArgumentException("La expresión está incompleta.");

        while (operators.Count > 0)
        {
            string op = operators.Pop();
            if (op == "(")
                throw new ArgumentException("Hay paréntesis sin pareja.");
            output.Add(op);
        }

        return output;
    }

    private static int Precedence(string op) => op switch
    {
        "+" or "-" => 1,
        "*" or "/" => 2,
        "u+" or "u-" => 3,
        "^" => 4,
        _ => throw new ArgumentException("Operador inválido.")
    };

    private static bool IsRightAssociative(string op) => op is "^" or "u+" or "u-";

    private static double EvaluatePostfix(List<string> postfix)
    {
        var values = new Stack<double>();

        foreach (string token in postfix)
        {
            if (double.TryParse(token, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double number))
            {
                values.Push(number);
                continue;
            }

            if (token is "u-" or "u+")
            {
                if (values.Count < 1)
                    throw new ArgumentException("Expresión inválida.");
                double value = values.Pop();
                values.Push(token == "u-" ? -value : value);
                continue;
            }

            if (values.Count < 2)
                throw new ArgumentException("Expresión inválida.");

            double right = values.Pop();
            double left = values.Pop();

            double result = token switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => right == 0 ? throw new DivideByZeroException("No se puede dividir entre cero.") : left / right,
                "^" => Math.Pow(left, right),
                _ => throw new ArgumentException("Operador inválido.")
            };

            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new ArithmeticException("El resultado no es un número real finito.");

            values.Push(result);
        }

        if (values.Count != 1)
            throw new ArgumentException("La expresión no está bien formada.");

        return values.Pop();
    }
}
