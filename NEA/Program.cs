using equation;
using expression;
using complex;
namespace NEA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Complex c = new Complex("1+2i");
            Complex d = c^(new Complex("1/2"));
        }
    }
}