namespace NEA;
using expression;
using matrix;
public class Program
{
    public static void Main(string[] args)
    {
        Expression e = new Expression("2 2 + 3 3 + *");
        Console.WriteLine(e.Evaluate());
        Matrix m = new Matrix(2, 2);
        m.Print();
    }
}