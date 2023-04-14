using utils;
namespace expression;


public class Expression : ICloneable
{
    public static Utils utils = new Utils();
    public Stack<string> rpnStack{get => UpdateRPNStack();}
    public string value;
    public Expression parent;
    public List<Expression> children = new List<Expression>();
    public bool isRoot => parent == null;
    public bool isLeaf => children.Count == 0;
    public bool isVariable => isLeaf && !double.TryParse(value, out double d);
    public bool isNumeric => isLeaf && double.TryParse(value, out double d);
    public Operator op{get
    {
        if (isLeaf)
        {
            return null;
        }
        return utils.operators.Where(o => o.symbol == value).First();
    }}
    public Expression(string value = "", Expression parent = null)
    {
        this.value = value;
    }
    public Expression(string value, List<Expression> children, Expression parent = null)
    {
        this.value = value;
        this.children = children;
    }
    public static Expression ExpressionFromRPN(string equation, string type)
    {
        Stack<string> setUpRpnStack;
        List<string> split = equation.Split(' ').ToList();
        FormatInput(split);
        if (type == "postfix")
        {
            split.Reverse();
            setUpRpnStack = new Stack<string>(split);
        }
        else if (type == "infix")
        {
            setUpRpnStack = new Stack<string>(InfixToPostfix(split.ToArray()));
        }
        else
        {
            throw new Exception("Invalid type");
        }

        return CreateExpressionTree(setUpRpnStack);
    }
    public override string ToString()
    {
        if (isLeaf)
        {
            return value;
        }
        string output = "";
        // if (!isRoot && op.precedence < parent.op.precedence)
        // {
        //     output += "(";
        // }
        for (int i = 0; i < children.Count; i++)
        {
            if (!children[i].isLeaf && children[i].op.precedence < op.precedence)
            {
                output += "(";
            }
            output += children[i].ToString();
            if (!children[i].isLeaf && children[i].op.precedence < op.precedence)
            {
                output += ")";
            }
            // if (i < children.Count - 1 && op.symbol == "*" && children[i].isNumeric && (children[i + 1].isVariable || children[i+1].op.precedence < op.precedence))
            // {
            //     continue;
            // }
            if (i != children.Count - 1)
            {
                output += value;
            }
        }
        // if (!isRoot && op.precedence < parent.op.precedence)
        // {
        //     output += ")";
        // }
        output = output.Replace("+-", "-");
        output = output.Replace(")*(", ")(");
        output = output.Replace("*(", "(");
        output = output.Replace(")*", ")");
        output = output.Replace("-1*", "-");
        
        output = output.Replace("^2", "²");
        output = output.Replace("^3", "³");
        output = output.Replace("^4", "⁴");
        output = output.Replace("^5", "⁵");
        output = output.Replace("^6", "⁶");
        output = output.Replace("^7", "⁷");
        output = output.Replace("^8", "⁸");
        output = output.Replace("^9", "⁹");

        
        

        return output;
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (obj.GetType() != typeof(Expression))
        {
            return false;
        }
        Expression other = (Expression) obj;

        if (value != other.value)
        {
            return false;
        }

        if (this.isLeaf && other.isLeaf)
        {
            return true;
        }
        foreach (Expression child in children)
        {
            if (!other.children.Contains(child))
            {
                return false;
            }
        }

        Expression otherCopy = other.Clone() as Expression;
        foreach (Expression child in children)
        {
            if (otherCopy.children.Contains(child))
            {
                otherCopy.children.Remove(child);
            }
            else {return false;}
        }
        return true;
    }
    public object Clone()
    {
        Expression clone = new Expression(value);
        foreach (Expression child in children)
        {
            clone.children.Add(child.Clone() as Expression);
        }
        return clone;
    }
    private static void FormatInput(List<string> split)
    {
        for (int i = 0; i < split.Count; i++)
        {
            if (!double.TryParse(split[i], out _) && !utils.IsOperator(split[i]))
            {
                if (split[i].Contains("+"))
                {
                    split[i] = split[i].Replace("+", " + ");
                }
                if (split[i].Contains("-"))
                {
                    split[i] = split[i].Replace("-", " - ");
                }
                if (split[i].Contains("*"))
                {
                    split[i] = split[i].Replace("*", " * ");
                }
                if (split[i].Contains("/"))
                {
                    split[i] = split[i].Replace("/", " / ");
                }
                if (split[i].Contains("^"))
                {
                    split[i] = split[i].Replace("^", " ^ ");
                }
                if (split[i].Contains("("))
                {
                    split[i] = split[i].Replace("(", " ( ");
                }
                if (split[i].Contains(")"))
                {
                    split[i] = split[i].Replace(")", " ) ");
                }

                List<string> toAdd = split[i].Split(' ').Where(x => x != "").ToList();
                split.RemoveAt(i);
                split.InsertRange(i, toAdd);
            }
        }
    }
    public static string PostfixToInfix(string[] exp)
    {
        Stack<string> s = new Stack<string>();
        for (int i = 0; i < exp.Length; i++)
        {
            if (!utils.IsOperator(exp[i]))
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
    public static string[] InfixToPostfix(string[] exp)
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
            else if (!utils.IsOperator(c))
            {
                output.Add(c);
            }
            else
            {
                if (s.Count == 0 || s.Peek() == "(" || utils.GetPrecedence(s.Peek()) < utils.GetPrecedence(c))
                {
                    s.Push(c);
                }
                else
                {
                    while (s.Count != 0 && (s.Peek() != "(" || utils.GetPrecedence(s.Peek()) >= utils.GetPrecedence(c)))
                    {
                        output.Add(s.Pop());
                    }
                    s.Push(c);
                }
            }
        }
        while (s.Count != 0)
        {
            output.Add(s.Pop());
        }
        output.Reverse();
        return output.ToArray();
    }
    private Stack<string> UpdateRPNStack()
    {
        return UpdateRPNStack(this);
    }
    private static Stack<string> UpdateRPNStack(Expression expression)
    {
        //Fix this so that it works for any number of children >= 2
        Stack<string> output = new Stack<string>();
        for (int i = 0; i < expression.children.Count-1; i++)
        {
            output.Push(expression.value);
        }
        for (int i = expression.children.Count-1; i>=0; i--)
        {
            if (expression.children[i].isLeaf)
            {
                output.Push(expression.children[i].value);
            }
            else
            {
                Stack<string> childOutput = UpdateRPNStack(expression.children[i]);
                foreach (string s in childOutput)
                {
                    output.Push(s);
                }
            }
        }
        return output;
    }
    public double EvaluateFromRpn()
    {
        Stack<string> newStack = new Stack<string>(rpnStack.Reverse());
        Stack<string> temp = new Stack<string>();
        while (newStack.Count > 0)
        {
            string s = newStack.Pop();
            if (utils.IsOperator(s))
            {
                double b = double.Parse(temp.Pop());
                double a = double.Parse(temp.Pop());
                double result = 0;
                switch (s)
                {
                    case "+":
                    {                        
                        result = a + b;
                        break;
                    }
                    case "-":
                    {
                        result = a - b;
                        break;
                    }
                    case "*":
                    {
                        result = a * b;
                        break;
                    }
                    case "/":
                    {
                        result = a / b;
                        break;
                    }
                    case "^":
                    {
                        result = (double)Math.Pow(a, b);
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
    private static double GCD(double a, double b)
    {
        if (a == 0)
            return b;
        return Expression.GCD(b % a, a);
    }
    public static Stack<String> ReduceToExactValueFromRpn(Stack<string> rpn)
    {
        Stack<string> temp = new Stack<string>();
        while (rpn.Count > 0)
        {
            string s = rpn.Pop();
            if (utils.IsOperator(s))
            {
                double result = 0;

                bool bIsVar = !double.TryParse(temp.Peek(), out double b);
                string bString = temp.Pop();
                bool aIsVar = !double.TryParse(temp.Peek(), out double a);
                string aString = temp.Pop();

                if (bIsVar || aIsVar)
                {
                    temp.Push(aString);
                    temp.Push(bString);
                    temp.Push(s);
                }
                else
                {
                    switch (s)
                    {
                        case "+":
                        {
                            temp.Push((a + b).ToString());
                            break;
                        }
                        case "-":
                        {
                            temp.Push((a - b).ToString());
                            break;
                        }
                        case "*":
                        {
                            temp.Push((a * b).ToString());
                            break;
                        }
                        case "/":
                        {
                            result = a / b;
                            if (result % 1 == 0)
                            {
                                temp.Push(result.ToString());
                            }
                            else
                            {
                                double gcd = GCD(a, b);
                                temp.Push((a / gcd).ToString());
                                temp.Push((b / gcd).ToString());
                                temp.Push("/");
                            }
                            break;
                        }
                        case "^":
                        {
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
                    }
                }
            }
            else
            {
                temp.Push(s);
            }
        }
        rpn = new Stack<string>(temp);
        return rpn;
    }
    public static Expression CreateExpressionTree(Stack<string> rpn)
    {
        Stack<Expression> stack = new Stack<Expression>();
        foreach (string s in rpn)
        {
            if (utils.IsOperator(s))
            {
                Expression right = stack.Pop();
                Expression left = stack.Pop(); 
                stack.Push(new Expression(s, new List<Expression>(){left, right}));
                right.parent = left.parent = stack.Peek();
            }
            else
            {
                stack.Push(new Expression(s));
            }
        }
        return stack.Pop();
    }     
    public static void Substitute(Expression Expression, string variable, double value)
    {
        if (Expression.isLeaf)
        {
            if (Expression.value == variable)
            {
                Expression.value = value.ToString();
            }
        }
        else
        {
            foreach (Expression child in Expression.children)
            {
                Substitute(child, variable, value);
            }
        }
    }
    public void Simplify()
    {
        Expression newRoot;
        do
        {
            newRoot = (Expression)this.Clone();
            CombineAssociativeOperators();
            GeneralSimplify();
            CombineLikeTerms();
        } while (!newRoot.Equals(this));
        Distribute();
        newRoot = null;
        do
        {
            newRoot = (Expression)this.Clone();
            CombineAssociativeOperators();
            GeneralSimplify();
            CombineLikeTerms();
        } while (!newRoot.Equals(this));
    }
    private void CombineNumericTerms()
    {
        List<Expression> numericChildren = children.Where(x => x.isNumeric).ToList();
        List<Expression> remainingChildren = children.Where(x => !x.isNumeric).ToList();
        if (numericChildren.Count == 0)
        {
            return;
        }
        Expression coefficient = new Expression(op.symbol, new List<Expression>(), parent);
        coefficient.value = op.Evaluate(numericChildren.Select(x => double.Parse(x.value)).ToList()).ToString();
        if (remainingChildren.Count == 0)
        {
            value = coefficient.value;
            children = new List<Expression>();
            return;
        }
        if (coefficient.value == "0" && value == "*")
        {
            value = "0";
            children = new List<Expression>();
            return;
        }
        children = new List<Expression>();
        if (value != "*" || coefficient.value != "1")
        {
            children.Add(coefficient);
        }
        children.AddRange(remainingChildren);
        if (children.Count == 1)
        {
            value = children[0].value;
            children = children[0].children;
        }
    }
    public void GeneralSimplify()
    {
        if (isLeaf)
        {
            return;
        }
        for (int childNo = 0; childNo < children.Count; childNo++)
        {
            Expression child = children[childNo];
            if (!child.isLeaf)
            {
                child.GeneralSimplify();
            }
        }
        if (value == "-")
        {
            value = "+";
            for (int i = 1; i < children.Count; i++)
            {
                Expression newChild = new Expression("*", new List<Expression>() { new Expression("-1"), children[i] }, this);
                newChild.GeneralSimplify();
                children[i] = newChild;
            }
        }
        if (op.commutative)
        {
            CombineNumericTerms();
        }
        if (isLeaf)
        {
            return;
        }
        if (op.symbol == "^")
        {
            for (int i = 1; i < children.Count; i++)
            {
                if (children[i].value == "0")
                {
                    value = "1";
                    children = new List<Expression>();
                }
                else if (children[i].value == "1")
                {
                    children.RemoveAt(i);
                    if (children.Count == 1)
                    {
                        value = children[0].value;
                        children = children[0].children;
                    }
                }
            }
        }
        if (children.Count == 1)
        {
            value = children[0].value;
            children = children[0].children;
        }
        //if all children are the same, and the operator is associative, then we can simplify
        else if (children.All(c => c.Equals(children[0])))
        {
            if (op != null)
            {
                if (op.associative)
                {
                    value = op.higherOrder.symbol;
                    children = new List<Expression>() { children[0], new Expression(children.Count().ToString()) };
                }
            }
        }
        else if (children.Any(c => c.value == "0") && value == "*")
        {
            value = "0";
            children = new List<Expression>();
        }      
        else if (children.Count == 2)
        {
            if (children[0].value == "1")
            {
                if (value == "*")
                {
                    value = children[1].value;
                    children = new List<Expression>();
                    return;                    
                }
            }
            if (children[1].value == "1")
            {
                if (value == "*")
                {
                    value = children[0].value;
                    children = new List<Expression>();
                    return;
                }
                if (value == "/")
                {
                    value = children[0].value;
                    children = new List<Expression>();
                    return;
                }
            }
            if (children[0].value == "0")
            {
                if (value == "+")
                {
                    value = children[1].value;
                    children = new List<Expression>();
                    return;
                }
                if (value == "-")
                {
                    value = "*";
                    children = new List<Expression>(){new Expression("-1"), children[1]};
                }
            }
            if (children[1].value == "0")
            {
                if (value == "*")
                {
                    value = children[0].value;
                    children = new List<Expression>();
                    return;
                }
                if (value == "/")
                {
                    value = children[0].value;
                    children = new List<Expression>();
                    return; 
                }
            }
        }
        if (op.symbol == "*")
        {
            foreach (Expression child in children)
            {
                if (child.value == "0")
                {
                    value = "0";
                    children = new List<Expression>();
                    return;
                }
                if (child.value == "1")
                {
                    children.Remove(child);
                    if (children.Count == 1)
                    {
                        value = children[0].value;
                        children = children[0].children;
                    }
                    return;
                }
            }
        }
        return;
    }
    public void CombineAssociativeOperators()
    {
        for (int childNo = 0; childNo < children.Count; childNo++)
        {
            Expression child = children[childNo];
            if (!child.isLeaf)
            {
                child.CombineAssociativeOperators();
            }
        }
    
        List<Expression> newChildren = new List<Expression>();
        foreach (Expression child in children)
        {
            if (child.value == value)
            {
                newChildren.AddRange(child.children);
            }
            else
            {
                newChildren.Add(child);
            }
        }
        children = newChildren;
        
        return;
    }
    private void Distribute()
    {
        foreach (Expression child in children)
        {
            if (!child.isLeaf)
            {
                child.Distribute();
            }
        }
        if (value != "*")
        {
            return;
        }
        List<List<Expression>> combinations = GetCombinations();
        value = "+";
        children = new List<Expression>();
        foreach (List<Expression> combination in combinations)
        {
            Expression subTotal = new Expression("*");
            foreach (Expression Expression in combination)
            {
                subTotal.children.Add(Expression);
            }
            children.Add(subTotal);
        }
        if (children.Count == 1)
        {
            value = children[0].value;
            children = children[0].children;
        }
        return;
    }  
    private List<List<Expression>> GetCombinations()
    {
        List<List<Expression>> currentCombinations = new List<List<Expression>>();


        for (int index = 0; index < children.Count; index++)
        {
            if (index == 0)
            {
                if (children[index].isLeaf)
                {
                    currentCombinations.Add(new List<Expression>() { children[index] });
                }
                else
                {
                    foreach (Expression child in children[index].children)
                    {
                        currentCombinations.Add(new List<Expression>() { child });
                    }
                }
                continue;
            }
            List<List<Expression>> newCombinations = new List<List<Expression>>();
            foreach (List<Expression> combination in currentCombinations)
            {
                if (children[index].isLeaf)
                {
                    List<Expression> newCombination = new List<Expression>(combination);
                    newCombination.Add(children[index]);
                    newCombinations.Add(newCombination);
                }
                else if (children[index].op.symbol == "+")
                {
                    foreach (Expression child in children[index].children)
                    {
                    
                        List<Expression> newCombination = new List<Expression>(combination);
                        newCombination.Add(child);
                        newCombinations.Add(newCombination);
                    }
                }
                else
                {
                    List<Expression> newCombination = new List<Expression>(combination);
                    newCombination.Add(children[index]);
                }
            }
            currentCombinations = newCombinations;
        }
        return currentCombinations;
    }
    public void CombineLikeTerms()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (!children[i].isLeaf)
            {
                children[i].CombineLikeTerms();
            }
        }

        if (value != "+")
        {
            return;
        }
        List<(Expression remaining, Expression coefficient)> terms = new List<(Expression, Expression)>();
        foreach (Expression child in children)
        {
            if (child.isLeaf)
            {
                if (terms.Any(t => t.remaining.Equals(child)))
                {
                    (Expression remaining, Expression coefficient) match = terms.Where(t => t.remaining.Equals(child)).First();
                    match.coefficient = new Expression("+", new List<Expression>(){match.coefficient, new Expression("1")});
                    match.coefficient.GeneralSimplify();
                    for (int i = 0; i < terms.Count; i++)
                    {
                        if (terms[i].remaining.Equals(child))
                        {
                            terms[i] = match;
                        }
                    }
                }
                else
                {
                    terms.Add((child, new Expression("1")));
                }
            }          
            else if (child.op.symbol == "*")
            {
                Expression coefficient = new Expression("*", child.children.Where(c => c.isNumeric).ToList());
                Expression remaining = new Expression("*", child.children.Where(c => !c.isNumeric).ToList());
                if (remaining.children.Count == 0)
                {
                    remaining = new Expression("1");
                }
                if (coefficient == null)
                {
                    coefficient = new Expression("1");
                }
                if (remaining.children.Count == 1)
                {
                    remaining = remaining.children[0];
                }
                if (coefficient.children.Count == 1)
                {
                    coefficient = coefficient.children[0];
                }
                remaining.GeneralSimplify();
                coefficient.GeneralSimplify();
                
                if (terms.Any(t => t.remaining.Equals(remaining)))
                {
                    (Expression remaining, Expression coefficient) match = terms.Where(t => t.remaining.Equals(remaining)).FirstOrDefault();
                    if (match.coefficient.isLeaf)
                    {
                        match.coefficient = new Expression("+", new List<Expression>(){match.coefficient, coefficient});
                    }
                    else
                    {
                        match.coefficient.children.Add(coefficient);
                    }
                    match.coefficient.GeneralSimplify();
                    for (int i = 0; i < terms.Count; i++)
                    {
                        if (terms[i].remaining.Equals(remaining))
                        {
                            terms[i] = match;
                        }
                    }
                }
                else
                {
                    terms.Add((remaining, coefficient));
                }
            }
            else
            {
                if (terms.Any(t => t.remaining.Equals(child)))
                {
                    (Expression remaining, Expression coefficient) match = terms.Where(t => t.remaining.Equals(child)).First();
                    match.coefficient = new Expression("+", new List<Expression>(){match.coefficient, new Expression("1")});
                    match.coefficient.GeneralSimplify();
                }
                else
                {
                    terms.Add((child, new Expression("1")));
                }
            }
        }
        children = new List<Expression>();
        foreach (var term in terms)
        {
            if (term.coefficient.value == "1")
            {
                children.Add(term.remaining);
            }
            else
            {
                children.Add(new Expression("*", new List<Expression>() { term.coefficient, term.remaining}));
            }
        }
        return;
    }
}