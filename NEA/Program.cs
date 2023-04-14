using System.Numerics;
using expression;
using complex;
namespace NEA;


public class Program
{
    public static void Main(string[] args)
    {
        Expression e = Expression.ExpressionFromRPN("(x+2*a)*(x-2)", "infix");
        Console.WriteLine(e);
        e.Simplify();
        Console.WriteLine(e);
    }
}