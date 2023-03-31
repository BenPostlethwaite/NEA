using System;
namespace NEA;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}
public abstract class RealNumber
{
    public virtual string ToString()
    {
        return "RealNumber";
    }
    public static RealNumber operator +(RealNumber a, RealNumber b)
    {
        return a+b;
    }
    public static RealNumber operator -(RealNumber a, RealNumber b)
    {
        return a-b;
    }
    public static RealNumber operator *(RealNumber a, RealNumber b)
    {
        return a*b;
    }
    public static RealNumber operator /(RealNumber a, RealNumber b)
    {
        return a/b;
    }
    public static RealNumber operator <(RealNumber a, RealNumber b)
    {
        return a<b;
    }
    public static RealNumber operator >(RealNumber a, RealNumber b)
    {
        return a>b;
    }
    public static RealNumber operator <=(RealNumber a, RealNumber b)
    {
        return a<=b;
    }
    public static RealNumber operator >=(RealNumber a, RealNumber b)
    {
        return a>=b;
    }
    public static RealNumber operator ==(RealNumber a, RealNumber b)
    {
        return a==b;
    }
    public static RealNumber operator !=(RealNumber a, RealNumber b)
    {
        return a!=b;
    }
    public static RealNumber Pow(RealNumber a, RealNumber b)
    {
        return 0;
    }
}
public class Fraction : RealNumber
{
    private int numerator;
    private int denominator;
    public Fraction(int numerator, int denominator)
    {
        this.numerator = numerator;
        this.denominator = denominator;
    }
    public override string ToString()
    {
        return numerator + "/" + denominator;
    }
    public static Fraction operator +(Fraction a, Fraction b)
    {
        return new Fraction(a.numerator * b.denominator + b.numerator * a.denominator, a.denominator * b.denominator);
    }

}
public class ComplexNumber
{
    private RealNumber real;
    private RealNumber imaginary;
    private modulus;
    private argument;

    private void Update()
    {
        this.modulus = Math.Sqrt(real * real + imaginary * imaginary);
        this.argument = Math.Atan(imaginary / real);
    }
    public ComplexNumber(RealNumber real, RealNumber imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }
    public override string ToString()
    {
        return real.ToString() + " + " + imaginary.ToString() + "i";
    }
    public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real + b.real, a.imaginary + b.imaginary);
    }
    public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real - b.real, a.imaginary - b.imaginary);
    }
    public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber(a.real * b.real - a.imaginary * b.imaginary, a.real * b.imaginary + a.imaginary * b.real);
    }
    public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
    {
        return new ComplexNumber((a.real * b.real + a.imaginary * b.imaginary) / (b.real * b.real + b.imaginary * b.imaginary), (a.imaginary * b.real - a.real * b.imaginary) / (b.real * b.real + b.imaginary * b.imaginary));
    }
    public static bool operator <(ComplexNumber a, ComplexNumber b)
    {
        return a.real < b.real && a.imaginary < b.imaginary;
    }
    public static bool operator >(ComplexNumber a, ComplexNumber b)
    {
        return a.real > b.real && a.imaginary > b.imaginary;
    }
    public static bool operator <=(ComplexNumber a, ComplexNumber b)
    {
        return a.real <= b.real && a.imaginary <= b.imaginary;
    }
    public static bool operator >=(ComplexNumber a, ComplexNumber b)
    {
        return a.real >= b.real && a.imaginary >= b.imaginary;
    }
    public static bool operator ==(ComplexNumber a, ComplexNumber b)
    {
        return a.real == b.real && a.imaginary == b.imaginary;
    }
    public static bool operator !=(ComplexNumber a, ComplexNumber b)
    {
        return a.real != b.real && a.imaginary != b.imaginary;
    }
}