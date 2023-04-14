using System;
using expression;
namespace complex
{
    
    // public class Complex : Expression
    // {
    //     public Complex(double real, double imaginary) : base($"{real}+{imaginary}*i", "infix")
    //     {

    //     }
    //     public double Real
    //     {
    //         get
    //         {
    //             Expression realParts = this.Clone() as Expression;
    //             realParts.Substitute("i", 0);
                
    //             return realParts.Evaluate();
    //         }
    //     }
    //     public double Imaginary
    //     {
    //         get
    //         {
    //             Expression total = this.Clone() as Expression;
    //             total.Substitute("i", 1);
    //             double realPlusImaginary = total.Evaluate();
    //             return realPlusImaginary - this.Real;
    //         }
    //     }
    //     public double Magnitude
    //     {
    //         get
    //         {
    //             return Math.Sqrt(Math.Pow(this.Real, 2) + Math.Pow(this.Imaginary, 2));
    //         }
    //     } 
    //     private static void RepaceRealWithZero(Expression imaginaryParts)
    //     {
    //         string[] rpnArray = imaginaryParts.rpnStack.ToArray();
    //         for (int i = 0; i < rpnArray.Length; i++)
    //         {
    //             if (double.TryParse(rpnArray[i], out double d))
    //             {
    //                 rpnArray[i] = "0";
    //             }
    //         }
    //         imaginaryParts.rpnStack = new Stack<string>(rpnArray);
    //     }
    // }
}