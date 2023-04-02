namespace expression;


public class Expression : ICloneable
{
    private Stack<string> rpnStack;
    private static Dictionary<string, int> operators = new Dictionary<string, int>()
    {
        {"+", 1},
        {"-", 1},
        {"*", 2},
        {"/", 2},
        {"^", 3},
        {"sqrt", 4}
    };
    public override string ToString()
    {
        return string.Join(" ", rpnStack.ToArray());
        //return ConvertToInfix(rpnStack.ToArray());
    }
    public Expression(string equation, string type = "postfix")
    {
        if (type == "postfix")
        {
            string[] split = equation.Split(' ');
            rpnStack = new Stack<string>(split.Reverse());
        }
        else if (type == "infix")
        {
            string[] split = equation.Split(' ');
            rpnStack = new Stack<string>(ConvertToPostfix(split));
        }
        else
        {
            throw new Exception("Invalid type");
        }
    }  
    public object Clone()
    {
        Expression other = new Expression(String.Join(" ", rpnStack.ToArray()));
        return other;
    }
    public double Evaluate()
    {
        Stack<string> newStack = new Stack<string>(rpnStack.Reverse());
        Stack<string> temp = new Stack<string>();
        while (newStack.Count > 0)
        {
            string s = newStack.Pop();
            if (operators.Keys.Contains(s))
            {
                double result = 0;
                switch (s)
                {
                    case "+":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = double.Parse(temp.Pop());
                        result = a + b;
                        break;
                    }
                    case "-":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = double.Parse(temp.Pop());
                        result = a - b;
                        break;
                    }
                    case "*":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = double.Parse(temp.Pop());
                        result = a * b;
                        break;
                    }
                    case "/":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = double.Parse(temp.Pop());
                        result = a / b;
                        break;
                    }
                    case "^":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = double.Parse(temp.Pop());
                        result = (double)Math.Pow(a, b);
                        break;
                    }
                    case "sqrt":
                    {
                        double a = double.Parse(temp.Pop());
                        result = Math.Sqrt(a);
                        break;
                    }
                }
                temp.Push(result.ToString());
            }
            else
            {
                temp.Push(s);
            }
        }
        return double.Parse(temp.Pop());
    }
    public void Simplify()
    {
        Stack<string> temp = new Stack<string>();
        while (rpnStack.Count > 0)
        {
            string s = rpnStack.Pop();
            if (operators.Keys.Contains(s))
            {
                double result = 0;
                switch (s)
                {
                    case "+":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = int.Parse(temp.Pop());
                        temp.Push((a + b).ToString());
                        break;
                    }
                    case "-":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = int.Parse(temp.Pop());
                        temp.Push((a - b).ToString());
                        break;
                    }
                    case "*":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = int.Parse(temp.Pop());
                        temp.Push((a * b).ToString());
                        break;
                    }
                    case "/":
                    {  
                        double b = double.Parse(temp.Pop());
                        double a = int.Parse(temp.Pop());
                        result = a / b;
                        if (result % 1 == 0)
                        {
                            temp.Push(result.ToString());
                        }
                        else
                        {
                            double gcd = Expression.GCD(a, b);
                            temp.Push((a / gcd).ToString());
                            temp.Push((b / gcd).ToString());
                            temp.Push("/");
                        }
                        break;
                    }
                    case "^":
                    {
                        double b = double.Parse(temp.Pop());
                        double a = int.Parse(temp.Pop());
                        result = (double)Math.Pow(a, b);
                        if (result % 1 == 0)
                        {
                            temp.Push(result.ToString());
                        }
                        else
                        {
                            temp.Push(a.ToString());
                            temp.Push(b.ToString());
                            temp.Push("^");
                        }
                        break;
                    }
                    case "sqrt":
                    {
                        double a = double.Parse(temp.Pop());
                        result = Math.Sqrt(a);
                        if (result % 1 == 0)
                        {
                            temp.Push(result.ToString());
                        }
                        else
                        {
                            temp.Push(a.ToString());
                            temp.Push("sqrt");
                        }
                        break;
                    }
                }
                
            }
            else
            {
                temp.Push(s);
            }
        }
        rpnStack = new Stack<string>(temp);
    }
    private static double GCD(double a, double b)
    {
        if (a == 0)
            return b;
        return Expression.GCD(b % a, a);
    }
    public static string ConvertToInfix(string[] exp)
    {
        Stack<string> s = new Stack<string>();
        for (int i = 0; i < exp.Length; i++)
        {
            if (!operators.Keys.Contains(exp[i]))
            {
                s.Push(exp[i] + "");
            }
            else
            {
                String op1 = (String) s.Peek();
                s.Pop();
                String op2 = (String) s.Peek();
                s.Pop();
                s.Push($"( {op2} {exp[i]} {op1} )");
            }
        }
        return (String)s.Peek();
    }
    public static string[] ConvertToPostfix(string[] exp)
    {
        Stack<string> s = new Stack<string>();
        List<string> output = new List<string>();
        foreach (string c in exp)
        {
            if (c == "(")
            {
                s.Push(c);
            }
            else if (c == ")")
            {
                while (s.Peek() != "(")
                {
                    output.Add(s.Pop());
                }
                s.Pop();
            }
            else if (!operators.Keys.Contains(c))
            {
                output.Add(c);
            }
            else
            {
                if (s.Peek() == "(" || operators[s.Peek()] < operators[c] )
                {
                    s.Push(c);
                }
                else
                {
                    while (s.Peek() != "(" || operators[s.Peek()] >= operators[c])
                    {
                        output.Add(s.Pop());
                    }
                    s.Push(c);
                }
            }
        }
        while (s.Count > 0)
        {
            output.Add(s.Pop());
        }
        return output.ToArray();

    }
}