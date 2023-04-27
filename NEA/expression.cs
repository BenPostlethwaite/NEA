using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Emit;
using System.Runtime;
using utils;
namespace expression;
public class Expression : ICloneable
{
    public static utils.Utils utils = new utils.Utils();
    //public Stack<string> rpnStack{get => UpdateRPNStack();}
    public Operator op;
    public Operand value;
    public Expression parent;
    public List<Expression> children;
    public bool isRoot => parent == null;
    public bool isLeaf => children.Count == 0;
    public bool isVariable => isLeaf && value is Variable;
    public bool isNumeric => isLeaf && value is Number;
    public Expression(string value = "", List<Expression> children = null, Expression parent = null)
    {
        if (utils.IsOperator(value))
        {
            this.op = utils.operators[value];
        }
        else if (double.TryParse(value, out double _))
        {
            this.value = new Number(int.Parse(value));
        }
        else
        {
            this.value = new Variable(value);
        }
        this.children = children;
        this.parent = parent;

        if (this.children == null)
        {
            this.children = new List<Expression>();
        }
    }
    public static Expression ExpressionFromString(string expression, string type)
    {
        Stack<string> setUpRpnStack;
        if (type == "postfix")
        {
            List<string> split = BasicFormat(expression);
            split.Reverse();
            setUpRpnStack = new Stack<string>(split);
            return ExpressionTreeFromRPN(setUpRpnStack);
        }
        else if (type == "infix")
        {
            List<string> split = FormatInfix(expression);
            return ExpressionTreeFromInfix(split);
            //setUpRpnStack = new Stack<string>(InfixToPostfix(split.ToArray()));
        }
        else
        {
            throw new Exception("Invalid type");
        }
    }
    private static List<string> BasicFormat(string expression)
    {
        foreach (string op in utils.operators.Keys)
        {
            expression = expression.Replace(op, " " + op + " ");
        }
        expression = expression.Replace("(", " ( ");
        expression = expression.Replace(")", " ) ");

        List<string> split = expression.Split(' ').Where(s => s != "").ToList();
        return split;
    }
    private static List<string> FormatInfix(string expression)
    {
        List<string> split = BasicFormat(expression);            
        List<string> newSplit = new List<string>();
        
        foreach (string s in split)
        {
            if (s.Length == 1)
            {
                newSplit.Add(s);
                continue;
            }
            string newS = "";
            foreach (char c in s)
            {
                if (char.IsLetter(c))
                {
                    if (newS != "")
                    {
                        newSplit.Add(newS);
                        newS = "";
                    }
                    newSplit.Add(c.ToString());
                    newS = "";
                }
                else
                {
                    newS += c;
                }
            }
        }
        split = newSplit;

        newSplit = new List<string>();

        for (int i = 0; i < split.Count; i++)
        {
            if ((i != 0 && split[i] == "(" && !utils.operators.Keys.Contains(split[i-1]) && split[i-1] != "(")
            || (i != 0 && char.IsLetter(split[i-1][0]) && !utils.operators.Keys.Contains(split[i]) && split[i] != ")")
            || (i != 0 && split[i-1] == ")" && !utils.operators.Keys.Contains(split[i]) && split[i] != ")")
            || (i != 0 && char.IsLetter(split[i][0]) && !utils.operators.Keys.Contains(split[i-1])) && split[i-1] != "(")
            {
                newSplit.Add("*");
            }
            newSplit.Add(split[i]);
        }            
        return newSplit;
    }
    private static Expression ExpressionTreeFromInfix(List<string> split)
    {
        Stack<Expression> expressionStack = new Stack<Expression>();
        Stack<string> operatorStack = new Stack<string>();
        foreach (string s in split)
        {
            if (utils.IsOperator(s))
            {
                while (operatorStack.Count > 0 && (operatorStack.Peek() == "(" || utils.operators[operatorStack.Peek()].precedence >= utils.operators[s].precedence))
                {
                    if (operatorStack.Peek() == "(")
                    {
                        break;
                    }
                    Expression right = expressionStack.Pop();
                    Expression left = expressionStack.Pop();
                    Expression newExpr = new Expression(operatorStack.Pop(), new List<Expression>() {left, right});
                    left.parent = newExpr;
                    right.parent = newExpr;
                    expressionStack.Push(newExpr);
                }
                operatorStack.Push(s);
            }
            else if (s == "(")
            {
                operatorStack.Push(s);
            }
            else if (s == ")")
            {
                while (operatorStack.Peek() != "(")
                {
                    Expression right = expressionStack.Pop();
                    Expression left = expressionStack.Pop();
                    Expression newExpr = new Expression(operatorStack.Pop(), new List<Expression>() {left, right});
                    left.parent = newExpr;
                    right.parent = newExpr;
                    expressionStack.Push(newExpr);
                }
                operatorStack.Pop();
            }
            else
            {
                expressionStack.Push(new Expression(s));
            }
        }
        while (operatorStack.Count > 0)
        {
            Expression right = expressionStack.Pop();
            Expression left = expressionStack.Pop();
            Expression newExpr = new Expression(operatorStack.Pop(), new List<Expression>() {left, right}, null);
            left.parent = newExpr;
            right.parent = newExpr;
            expressionStack.Push(newExpr);
        }
        return expressionStack.Pop();
    }
    public override string ToString()
    {
        if (isLeaf)
        {
            return value.ToString();
        }
        string output = "";
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

            if (i == children.Count - 1 || (op.symbol == "*" && children[i + 1].isVariable))
            {
                continue;
            }
            else{output += op.ToString();}
        }
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
    
        for (int i = 0; i < output.Length; i++)
        {
            if (output[i] == '*' && i < output.Length - 1 && char.IsLetter(output[i + 1]))
            {
                output = output.Remove(i, 1);
            }
        }

        return output;
    }
    public static bool operator ==(Expression a, Expression b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Expression a, Expression b)
    {
        return !a.Equals(b);
    }
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        try
        {
            obj = (Expression) obj;
        }
        catch
        {
            return false;
        }
        Expression other = (Expression) obj;

        if (this.isLeaf && other.isLeaf)
        {
            if (!value.Equals(other.value))
            {
                return false;
            }
            else {return true;}
        }
        else if (this.isLeaf || other.isLeaf)
        {
            return false;
        }
        else if (!op.Equals(other.op))
        {
            return false;
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
        Expression clone;
        if (isLeaf)
        {
            clone = new Expression(this.value.ToString());
        }
        else
        {
            clone = new Expression(this.op.ToString());
        }
        foreach (Expression child in children)
        {
            clone.children.Add(child.Clone() as Expression);
        }
        return clone;
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
        Stack<string> output = new Stack<string>();
        for (int i = 0; i < expression.children.Count-1; i++)
        {
            output.Push(expression.value.ToString());
        }
        for (int i = expression.children.Count-1; i>=0; i--)
        {
            if (expression.children[i].isLeaf)
            {
                output.Push(expression.children[i].value.ToString());
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
    // public double EvaluateFromRpn()
    // {
    //     Stack<string> newStack = new Stack<string>(rpnStack.Reverse());
    //     Stack<string> temp = new Stack<string>();
    //     while (newStack.Count > 0)
    //     {
    //         string s = newStack.Pop();
    //         if (utils.IsOperator(s))
    //         {
    //             double b = double.Parse(temp.Pop());
    //             double a = double.Parse(temp.Pop());
    //             double result = 0;
    //             switch (s)
    //             {
    //                 case "+":
    //                 {                        
    //                     result = a + b;
    //                     break;
    //                 }
    //                 case "-":
    //                 {
    //                     result = a - b;
    //                     break;
    //                 }
    //                 case "*":
    //                 {
    //                     result = a * b;
    //                     break;
    //                 }
    //                 case "/":
    //                 {
    //                     result = a / b;
    //                     break;
    //                 }
    //                 case "^":
    //                 {
    //                     result = (double)Math.Pow(a, b);
    //                     break;
    //                 }
    //             }
    //             temp.Push(result.ToString());
    //         }
    //         else
    //         {
    //             temp.Push(s);
    //         }
    //     }
    //     return double.Parse(temp.Pop());
    // }
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
    public static Expression ExpressionTreeFromRPN(Stack<string> rpn)
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
    public void Substitute(string variable, string subIn)
    {
        Expression var = ExpressionFromString(variable, "infix");
        if (Equals(var))
        {
            CloneFrom(new Expression(subIn));
        }
        else
        {
            foreach (Expression child in children)
            {
                child.Substitute(variable, subIn);
            }
        }
    }
    public void Substitute(string variable, Expression subIn)
    {
        if (isLeaf)
        {
            if (value.ToString() == variable)
            {
                value = subIn.value;
                children = subIn.children;
            }
        }
        else
        {
            foreach (Expression child in children)
            {
                child.Substitute(variable, subIn);
            }
        }
    }
    public void ExpandAndSimplify()
    {
        Expression newRoot;
        do
        {
            newRoot = (Expression)this.Clone();
            Simplify();
            Distribute();
            ExpandPowers();
            Simplify();

            foreach (Expression child in children)
            {
                if (!child.isLeaf)
                {
                    child.ExpandAndSimplify();
                }
            }
        } while (!newRoot.Equals(this));
    }
    public virtual void Simplify()
    {
        Expression newRoot;
        do
        {
            newRoot = (Expression)this.Clone();
            CombineAssociativeOperators();
            GeneralSimplify();
            CombineLikeTerms();
            CombineAssociativeOperators();
        } while (!newRoot.Equals(this));
    }
    private void CombineNumericTerms()
    {
        foreach (Expression child in children)
        {
            if (!child.isLeaf && child.op.commutative)
            {
                child.CombineNumericTerms();
            }
        }
        List<Expression> numericChildren = children.Where(x => x.isNumeric).ToList();
        List<Expression> remainingChildren = children.Where(x => !x.isNumeric).ToList();
        if (numericChildren.Count <= 1)
        {
            return;
        }
        Expression coefficient = new Expression(op.symbol, numericChildren, parent);
        coefficient.GeneralSimplify();
        if (remainingChildren.Count == 0)
        {
            value = coefficient.value;
            children = new List<Expression>();
            return;
        }
        if (coefficient.value.ToString() == "0" && op.ToString() == "*")
        {
            value = new Number(0);
            children = new List<Expression>();
            return;
        }
        children = new List<Expression>();
        if (op.ToString() != "*" || coefficient.value.ToString() != "1")
        {
            children.Add(coefficient);
        }
        children.AddRange(remainingChildren);
        if (children.Count == 1)
        {
            value = children[0].value;
            op = children[0].op;
            children = children[0].children;
        }
    }
    private bool AllChildrenAreNumeric()
    {
        foreach (Expression child in children)
        {
            if (!child.isLeaf)
            {
                if (!child.isNumeric)
                {
                    return false;
                }
                else if (!child.AllChildrenAreNumeric())
                {
                    return false;
                }
            }
        }
        return true;
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
        if (op.symbol.ToString() == "-")
        {
            if (children.Count == 1)
            {
                value = null;
                op = utils.operators["*"];
                children.Insert(0, new Expression("-1"));
            }
            else
            {
                value = null;
                op = utils.operators["+"];
                for (int i = 1; i < children.Count; i++)
                {
                    Expression newChild = new Expression("*", new List<Expression>() { new Expression("-1"), children[i] }, this);
                    newChild.GeneralSimplify();
                    children[i] = newChild;
                }
            }
        }
        if (op.ToString() == "/" && children.Count == 2)
        {
            SimplifyFraction();
            return;           
        }
        if (children.Count == 0)
        {
            return;
        }
        if (op.commutative)
        {
            if (children.All(x => x.isNumeric))
            {
                int intValue = (int)op.Evaluate(children.Select(x => double.Parse(x.value.ToString())).ToList());
                value = new Number(intValue);
                op = null;
                children = new List<Expression>();
                return;
            }
            else
            {
                CombineNumericTerms();
            }
        }
        if (isLeaf)
        {
            return;
        }
        if (op.symbol == "^")
        {
            for (int i = 1; i < children.Count; i++)
            {
                if (children[i].isLeaf && children[i].value.ToString() == "0")
                {
                    value = new Number(1);
                    op = null;
                    children = new List<Expression>();
                    return;
                }
                else if (children[i].isLeaf && children[i].value.ToString() == "1")
                {
                    children.RemoveAt(i);
                    if (children.Count == 1)
                    {
                        op = children[0].op;
                        value = children[0].value;
                        children = children[0].children;
                        return;
                    }
                }
            }
            if (!children[0].isLeaf && children[0].op.symbol == "^")
            {
                Expression base1 = children[0].children[0];
                Expression exponent1 = children[0].children[1];
                Expression exponent2 = children[1];
                children = new List<Expression>();
                base1.parent = this;
                children.Add(base1);
                children.Add(new Expression("*", new List<Expression>() { exponent1, exponent2 }, this));
            }
        }
        if (children.Count == 1)
        {
            op = children[0].op;
            value = children[0].value;
            children = children[0].children;
        }
        if (op.symbol.ToString() == "*")
        {
            children.RemoveAll(c => c.isLeaf && c.value.ToString() == "1");
            if (children.Any(c => c.isLeaf && c.value.ToString() == "0"))
            {
                value = new Number(0);
                children = new List<Expression>();
                return;
            }
            if (children.All(c=> c==children[0]))
            {
                int exponent = children.Count;
                children = new List<Expression>() { children[0], new Expression(exponent.ToString())};
                op = utils.operators["^"];
                value = null;
                return;
            }
            if (children.Any(c => !c.isLeaf && c.op.symbol == "/"))
            {
                List<Expression> numerators = children.Where(c => !c.isLeaf && c.op.symbol == "/").Select(c => c.children[0]).ToList();
                List<Expression> denominators = children.Where(c => !c.isLeaf && c.op.symbol == "/").Select(c => c.children[1]).ToList();
                List<Expression> notFraction = children.Where(c => c.isLeaf || (c.op.symbol != "/")).ToList();
                List<Expression> newChildren = new List<Expression>();
                numerators.AddRange(notFraction);

                Expression newNumerator = new Expression("*", numerators, this);
                Expression newDenominator = new Expression("*", denominators, this);
                
                if (newNumerator.children.Count == 1)
                {
                    newNumerator = newNumerator.children[0];
                }
                else if (newNumerator.children.Count == 0)
                {
                    newNumerator = new Expression("1", parent: this);
                }
                if (newDenominator.children.Count == 1)
                {
                    newDenominator = newDenominator.children[0];
                }
                else if (newDenominator.children.Count == 0)
                {
                    CloneFrom(newNumerator);
                    return;
                }
                
                newChildren.Add(newNumerator);
                newChildren.Add(newDenominator);
                children = newChildren;
                op = utils.operators["/"];
                value = null;
                return;
            }
        }
        else if (op.symbol.ToString() == "+")
        {
            children.RemoveAll(c => c.isLeaf && c.value.ToString() == "0");
        }
        if (children.Count == 1)
        {
            value = children[0].value;
            children = children[0].children;
            return;
        }
        for(int childNo = 0; childNo < children.Count; childNo++)
        {
            Expression child = children[childNo];
            if (!isLeaf && child.isNumeric)
            {
                if ((child.value.ToString() == "1" && op.ToString() == "*") ||
                 (child.value.ToString() == "0" && op.ToString() == "+"))
                {
                    children.Remove(child);
                }
                if (child.value.ToString() == "0" && op.ToString() == "*")
                {
                    value = new Number(0);
                    children = new List<Expression>();
                    return;
                }
                if (child.value.ToString() == "0" && op.ToString() == "^")
                {
                    value = new Number(1);
                    children = new List<Expression>();
                    return;
                }
            }
            if (children.Count == 1)
            {
                value = children[0].value;
                op = children[0].op;
                children = children[0].children;
                return;
            }
        }
        if (children.All(c => c.isNumeric))
        {
            double evaluation = op.Evaluate(children.Select(x => double.Parse(x.value.ToString())).ToList());
            if (evaluation % 1 == 0)
            {
                op = null;
                value = new Number((int)evaluation);
                children = new List<Expression>();
                return;
            }
        }
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
        if (isLeaf || !op.associative)
        {
            return;
        }
        List<Expression> newChildren = new List<Expression>();
        foreach (Expression child in children)
        {
            if (!child.isLeaf && child.op == op)
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
        if (isLeaf || op.symbol != "*")
        {
            return;
        }
        if (!children.All(c => c.isLeaf || c.op.commutative))
        {
            return;
        }
        List<List<Expression>> combinations = GetCombinations();
        value = null;
        op = utils.operators["+"];
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
            op = children[0].op;
            value = children[0].value;
            children = children[0].children;
        }
        return;
    }  
    private void ExpandPowers()
    {
        if (isLeaf || op.ToString() != "^")
        {
            return;
        }
        while (children.Count > 2)
        {
            Expression child1 = new Expression(op.symbol.ToString(), new List<Expression>() { children[0], children[1] }, this);
            children.RemoveRange(0, 2);
            children.Insert(0, child1);
            
        }
        if (children[0].isLeaf)
        {
            return;
        }
        while (children[0].children.Count > 2)
        {
            Expression child1 = new Expression(children[0].op.symbol.ToString(), new List<Expression>() { children[0].children[0], children[0].children[1] }, this);
            children[0].children.RemoveRange(0, 2);
            children[0].children.Insert(0, child1);
        }
        if (!children[1].isNumeric)
        {
            return;
        }
        int exponent = int.Parse(children[1].value.ToString());
        Expression baseExpression = children[0];

        //binomial expansion
        if (baseExpression.op.ToString() == "+")
        {
            value = null;
            op = utils.operators["+"];
            children = new List<Expression>();
            for (int i = 0; i <= exponent; i++)
            {
                Expression newChild = new Expression("*", new List<Expression>(), this);

                Expression coefficient = new Expression(utils.Combination(exponent, i).ToString(), new List<Expression>(), newChild);
                Expression part1 = new Expression("^", new List<Expression>() { baseExpression.children[0], new Expression(i.ToString()) }, newChild);
                Expression part2 = new Expression("^", new List<Expression>() { baseExpression.children[1], new Expression((exponent - i).ToString()) }, newChild);
                
                newChild = new Expression("*", new List<Expression>() { coefficient, part1, part2 }, this);
                children.Add(newChild);
            }
        }
        else if (baseExpression.op.ToString() == "*")
        {
            value = null;
            op = utils.operators["*"];
            children = new List<Expression>();
            foreach (Expression child in baseExpression.children)
            {
                children.Add(new Expression("^", new List<Expression>() { child, new Expression(exponent.ToString()) }, this));
            }
            
        }
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
                    newCombinations.Add(newCombination);
                }
            }
            currentCombinations = newCombinations;
        }
        return currentCombinations;
    }
    public void CombineLikeTerms()
    {
        //this sucks
        for (int i = 0; i < children.Count; i++)
        {
            if (!children[i].isLeaf)
            {
                children[i].CombineLikeTerms();
            }
        }
        if (!isLeaf && !op.commutative)
        {
            return;
        }

        Operator oldOperator = op;

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
            if (term.coefficient.isLeaf && term.coefficient.value.ToString() == "1")
            {
                children.Add(term.remaining);
            }
            else
            {
                children.Add(new Expression(oldOperator.higherOrder.symbol, new List<Expression>() { term.remaining, term.coefficient}));
            }
        }
        return;
    }
    public void CloneFrom(Expression expression)
    {
        op = expression.op;
        value = expression.value;
        children = new List<Expression>();
        foreach (Expression child in expression.children)
        {
            children.Add(child.Clone() as Expression);
        }
    }
    private static List<Expression> CommonFactors(List<Expression> list)
    {
        for (int i=0; i < list.Count; i++)
        {
            if (list[i].isLeaf || list[i].op.ToString() != "*")
            {
                list[i] = new Expression("*", new List<Expression>(){list[i]});
            }
        }

        List<Expression> commonFactors = new List<Expression>();
        for (int i = 0; i < list[0].children.Count; i++)
        {
            if (list[0].children.Count == 0)
            {
                return commonFactors;
            }
            Expression factor;
            //Improve line below, maybe check if exponent is numeric, or somehow if not test the whole thing as a factor
            if (!list[0].children[i].isLeaf && list[0].children[i].op.ToString() == "^")
            {
                factor = list[0].children[i].children[0];
            }
            else
            {
                factor = list[0].children[i];
            }
            bool isCommon = true;
            for (int j = 1; j < list.Count; j++)
            {
                if (list[j].children.Count == 0)
                {
                    return commonFactors;
                }

                bool commonForThisExpression = false;
                for (int k = 0; k < list[j].children.Count; k++)
                {
                    Expression part = list[j].children[k];
                    if (!part.isLeaf && part.op.ToString() == "^")
                    {
                        if (part.children[0].Equals(factor))
                        {
                            commonForThisExpression = true;
                            break;
                        }
                    }
                    else if (part.Equals(factor))
                    {
                        commonForThisExpression = true;
                        break;
                    }

                }
                if (!commonForThisExpression)
                {
                    isCommon = false;
                    break;
                }
            }
            if (isCommon)
            {
                commonFactors.Add(factor);
                i--;
                foreach (Expression expression in list)
                {
                    foreach (Expression part in expression.children)
                    {
                        if (!part.isLeaf && part.op.ToString() == "^")
                        {
                            if (part.children[0].Equals(factor))
                            {
                                part.children[1] = (Expression)part.children[1].Clone() - new Expression("1");
                                part.Simplify();
                            }
                            break;
                        }
                        else if (part.Equals(factor))
                        {
                            expression.children.Remove(part);
                            break;
                        }
                    }
                }
            }
        }
        return commonFactors;
    }
    public void Factorise()
    {
        if (isLeaf)
        {
            return;
        }
        
        Expression copy = Clone() as Expression;
        copy.Simplify();
        copy.ConstantsToPrimeFactors();
        copy.CombineAssociativeOperators();
        if (copy.op.symbol != "+")
        {
            CloneFrom(copy);
            return;
        }
        List<Expression> commonFactors = CommonFactors(copy.children);
        for (int i = 0; i < copy.children.Count; i++)
        {
            if (copy.children[i].children.Count == 0)
            {
                copy.children[i] = new Expression("1");
            }
            else if (copy.children[i].children.Count == 1)
            {
                copy.children[i] = copy.children[i].children[0];
            }
        }
        Expression commonFactorsMultiplied;
        copy.Simplify();
        if (commonFactors.Count == 0)
        {
            return;
        }
        else if (commonFactors.Count == 1)
        {
            commonFactorsMultiplied = commonFactors[0];
        }
        else
        {
            commonFactorsMultiplied = new Expression("*", commonFactors);
            commonFactorsMultiplied.Simplify();
        }
        CloneFrom(commonFactorsMultiplied*copy);
        Simplify();
    }
    private void ConstantsToPrimeFactors()
    {
        foreach (Expression child in children)
        {
            child.ConstantsToPrimeFactors();
        }
        if (isNumeric)
        {
            List<int> primeFactors = (value as Number).primeFactors;
            children = new List<Expression>();
            foreach (int factor in primeFactors)
            {
                children.Add(new Expression(factor.ToString()));
            }
            if (children.Count == 1)
            {
                CloneFrom(children[0]);
            }
            else
            {
                value = null;
                op = utils.operators["*"];
            }
        }

    }
    private void SimplifyFraction()
    {
        if (op.symbol != "/")
        {
            return;
        }
        Expression numerator = children[0];
        Expression denominator = children[1];

        numerator.Factorise();
        denominator.Factorise();

        
        if (numerator.isLeaf)
        {
            numerator = new Expression("*", new List<Expression>() {numerator.Clone() as Expression});
        }
        else if (numerator.op.symbol != "*")
        {
            return;
        }
        if (denominator.isLeaf || denominator.op.symbol != "*")
        {
            denominator = new Expression("*", new List<Expression>() { denominator.Clone() as Expression });
        }
        else if (denominator.op.symbol != "*")
        {
            return;
        }
        List<Expression> commonFactors = new List<Expression>();

        for (int i = 0; i < numerator.children.Count; i++)
        {
            for (int j = 0; j < denominator.children.Count; j++)
            {
                if (numerator.children[i].Equals(denominator.children[j]))
                {
                    commonFactors.Add(numerator.children[i]);
                    numerator.children.RemoveAt(i);
                    denominator.children.RemoveAt(j);
                    i--;
                    break;
                }
            }
        }
        if (numerator.children.Count == 0)
        {
            numerator = new Expression("1");
        }
        else if (numerator.children.Count == 1)
        {
            numerator = numerator.children[0];
        }

        if (denominator.children.Count == 0)
        {
            CloneFrom(numerator);
            return;
        }
        else if (denominator.children.Count == 1)
        {
            denominator = denominator.children[0];
        }
        CloneFrom(new Expression("/", new List<Expression>() { numerator, denominator }));
    }   
    //TODO: Rationalise Denominator
    public void RationaliseDenominator()
    {
        if (isLeaf)
        {
            return;
        }
        if (op.symbol != "/")
        {
            return;
        }
        Expression numerator = children[0];
        Expression denominator = children[1];
        if (denominator.isLeaf)
        {
            return;
        }


    }
    public static Expression operator +(Expression a, Expression b)
    {
        Expression toReturn = new Expression("+", new List<Expression>() { a, b });
        a.parent = toReturn;
        b.parent = toReturn;
        return toReturn;
    }
    public static Expression operator -(Expression a, Expression b)
    {
        Expression toReturn = new Expression("-", new List<Expression>() { a, b });
        a.parent = toReturn;
        b.parent = toReturn;
        return toReturn;
    }
    public static Expression operator *(Expression a, Expression b)
    {
        Expression toReturn = new Expression("*", new List<Expression>() { a, b });
        a.parent = toReturn;
        b.parent = toReturn;
        return toReturn;
    }
    public static Expression operator /(Expression a, Expression b)
    {
        Expression toReturn = new Expression("/", new List<Expression>() { a, b });
        a.parent = toReturn;
        b.parent = toReturn;
        return toReturn;
    }
    public static Expression operator ^(Expression a, Expression b)
    {
        Expression toReturn = new Expression("^", new List<Expression>() { a, b });
        a.parent = toReturn;
        b.parent = toReturn;
        return toReturn;
    }
    public static Expression operator +(Expression a, double b)
    => a + new Expression(b.ToString());
    public static Expression operator -(Expression a, double b)
    => a - new Expression(b.ToString());
    public static Expression operator *(Expression a, double b)
    => a * new Expression(b.ToString());
    public static Expression operator /(Expression a, double b)
    => a / new Expression(b.ToString());
    public static Expression operator ^(Expression a, double b)
    => a ^ new Expression(b.ToString());
    public static Expression operator +(double a, Expression b)
    => new Expression(a.ToString()) + b;
    public static Expression operator -(double a, Expression b)
    => new Expression(a.ToString()) - b;
    public static Expression operator *(double a, Expression b)
    => new Expression(a.ToString()) * b;
    public static Expression operator /(double a, Expression b)
    => new Expression(a.ToString()) / b;
    public static Expression operator ^(double a, Expression b)
    => new Expression(a.ToString()) ^ b;
    public static Expression operator -(Expression a)
    => new Expression("0") - a;

}
class Number : Operand, IComparable<Number>
{
    public int value;
    public bool IsPositive => value >= 0;
    public bool IsNegative => value < 0;
    public bool isNatural => value > 0;
    public List<int> primeFactors{get
    {
        List<int> factors = new List<int>();
        int n = (int)value;
        if (n < 0)
        {
            n = -n;
            factors.Add(-1);
        }
        for (int i = 2; i <= n / i; i++)
        {
            while (n % i == 0)
            {
                factors.Add(i);
                n /= i;
            }
        }
        if (n > 1)
        {
            factors.Add(n);
        }
        return factors;
    }}
    public Number(int value)
    {
        this.value = value;
    }
    public int CompareTo(Number other)
    {
        return value.CompareTo(other.value);
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public override bool Equals(object obj)
    {
        if (obj is Number)
        {
            return value == ((Number)obj).value;
        }
        return false;
    }
}
class Variable : Operand
{
    string name;
    public Variable(string name)
    {
        this.name = name;
    }
    public override string ToString()
    {
        return name;
    }
    public override bool Equals(object obj)
    {
        if (obj is Variable)
        {
            return name == ((Variable)obj).name;
        }
        return false;
    }
}
public interface Operand
{
    public string ToString();
    public bool Equals(object obj);
}