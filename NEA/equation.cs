using expression;
namespace equation
{
    class Equation
    {
        public expression.Expression left;
        public expression.Expression right;
        public Equation(string equation, char var = 'x')
        {
            string[] split = equation.Split('=');
            left = Expression.ExpressionFromRPN(split[0], "infix");
            right = Expression.ExpressionFromRPN(split[1], "infix");            
        }
    }
}
