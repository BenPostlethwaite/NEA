using System.Threading;
using expression;
namespace equation
{
    class Equation
    {
        public expression.Expression left;
        public expression.Expression right;
        public Equation(string equation)
        {
            string[] split = equation.Split('=');
            left = Expression.ExpressionFromString(split[0], "infix");
            right = Expression.ExpressionFromString(split[1], "infix");
        }

        public void Simplify()
        {
            left.Simplify();
            right.Simplify();
            left = new Expression("-", new List<Expression> { left, right });
            left.Simplify();
            right = new Expression("0");
        }
        //TODO
        public void Solve(string var)
        {
            Simplify();
            if (!left.isLeaf && left.op.symbol == "+")
            {
                
            }
        }
        public override string ToString()
        {
            return left.ToString() + "=" + right.ToString();
        }
    }
}
