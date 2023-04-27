using System;
using expression;
namespace complex
{ 
    public class Complex : Expression
    {
        public Complex(double real, double imaginary)
        {
            CloneFrom(ExpressionFromString((real.ToString() + " + " + imaginary.ToString() + "i"), "infix"));
        }
        public Complex(string complexNumber)
        {
            CloneFrom(ExpressionFromString(complexNumber, "infix"));
        }
        public Complex() : base(){}
        public void SimplifyComplex()
        {
            ExpandAndSimplify();
            ReplaceExponentsOfI();
            ExpandAndSimplify();
        }
        private void ReplaceExponentsOfI()
        {
            if (!isLeaf && op.symbol == "^")
            {
                if (children[0].isLeaf && children[0].value.ToString() == "i")
                {
                    if (children[1].isNumeric)
                    {
                        int exponent = int.Parse(children[1].value.ToString());
                        int mod = exponent % 4;
                        if (mod == 0)
                        {
                            this.CloneFrom(ExpressionFromString("1", "infix"));
                        }
                        else if (mod == 1)
                        {
                            this.CloneFrom(ExpressionFromString("i", "infix"));
                        }
                        else if (mod == 2)
                        {
                            this.CloneFrom(ExpressionFromString("0-1", "infix"));
                        }
                        else if (mod == 3)
                        {
                            this.CloneFrom(ExpressionFromString("-i", "infix"));
                        }
                    }
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                Complex c = new Complex();
                c.CloneFrom(children[i]);                
                c.ReplaceExponentsOfI();
                children[i] = c as Expression;
            }
        }
        private static Complex ExpressionToComplex(Expression e)
        {
            Complex c = new Complex();
            c.CloneFrom(e);
            return c;
        }
        public static Complex operator *(Complex a, Complex b)
        => ExpressionToComplex((Expression)a*(Expression)b);
        public static Complex operator +(Complex a, Complex b)
        => ExpressionToComplex((Expression)a+(Expression)b);
        public static Complex operator -(Complex a, Complex b)
        => ExpressionToComplex((Expression)a-(Expression)b);
        public static Complex operator /(Complex a, Complex b)
        => ExpressionToComplex((Expression)a/(Expression)b);
        public static Complex operator ^(Complex a, Complex b)
        => ExpressionToComplex((Expression)a^(Expression)b);

        public static Complex operator *(Complex a, double b)
        => ExpressionToComplex((Expression)a*b);
        public static Complex operator +(Complex a, double b)
        => ExpressionToComplex((Expression)a+b);
        public static Complex operator -(Complex a, double b)
        => ExpressionToComplex((Expression)a-b);
        public static Complex operator /(Complex a, double b)
        => ExpressionToComplex((Expression)a/b);
        public static Complex operator ^(Complex a, double b)
        => ExpressionToComplex((Expression)a^b);

        public static Complex operator *(double a, Complex b)
        => ExpressionToComplex(a*(Expression)b);
        public static Complex operator +(double a, Complex b)
        => ExpressionToComplex(a+(Expression)b);
        public static Complex operator -(double a, Complex b)
        => ExpressionToComplex(a-(Expression)b);
        public static Complex operator /(double a, Complex b)
        => ExpressionToComplex(a/(Expression)b);
        public static Complex operator ^(double a, Complex b)
        => ExpressionToComplex(a^(Expression)b);
    }
}