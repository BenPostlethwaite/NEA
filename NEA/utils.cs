namespace utils;

public class Utils
{
    public List<Operator> operators;
    public Utils()
    {
        SetUpOperators();
    }
    public void SetUpOperators()
    {
        operators = new List<Operator>();
        Add add = new Add();
        Subtract subtract = new Subtract();
        Multiply multiply = new Multiply();
        Divide divide = new Divide();
        Power power = new Power();

        add.inverse = subtract;
        subtract.inverse = add;
        multiply.inverse = divide;
        divide.inverse = multiply;
        power.inverse = null;

        add.higherOrder = multiply;
        add.lowerOrder = subtract;
        multiply.higherOrder = power;
        multiply.lowerOrder = add;

        operators.Add(add);
        operators.Add(subtract);
        operators.Add(multiply);
        operators.Add(divide);
        operators.Add(power);
    }
    public bool IsOperator(string s)
    {
        return operators.Select(x => x.symbol).Contains(s);
    }
    public bool IsAssociative(string s)
    {
        return operators.Where(x => x.symbol == s).Select(x => x.associative).First();
    }
    public int GetPrecedence(string s)
    {
        return operators.Where(x => x.symbol == s).Select(x => x.precedence).First();
    }
}
public abstract class Operator
    {
        public string symbol;
        public bool commutative;
        public bool associative;
        public bool distributive;
        public int precedence;
        public Operator higherOrder;
        public Operator lowerOrder;
        public Operator inverse;

        public override string ToString()
        {
            return this.symbol;
        }
        virtual public double Evaluate(List<double> operands)
        {
            throw new Exception("Invalid operator");
        }
    }
    public class Add : Operator
    {
        public Add()
        {
            this.symbol = "+";
            this.commutative = true;
            this.associative = true;
            this.precedence = 1;
        }
        override public double Evaluate(List<double> operands)
        {
            double result = 0;
            foreach (double operand in operands)
            {
                result += operand;
            }
            return result;
        }
    }
    public class Subtract : Operator
    {
        public Subtract()
        {
            this.symbol = "-";
            this.commutative = false;
            this.associative = false;
            this.precedence = 1;
        }
        override public double Evaluate(List<double> operands)
        {
            double result = operands[0];
            for (int i = 1; i < operands.Count; i++)
            {
                result -= operands[i];
            }
            return result;
        }
    }
    public class Multiply : Operator
    {
        public Multiply()
        {
            this.symbol = "*";
            this.commutative = true;
            this.associative = true;
            this.precedence = 2;
        }
        override public double Evaluate(List<double> operands)
        {
            double result = 1;
            foreach (double operand in operands)
            {
                result *= operand;
            }
            return result;
        }
    }
    public class Divide : Operator
    {
        public Divide()
        {
            this.symbol = "/";
            this.commutative = false;
            this.associative = false;
            this.precedence = 2;
        }
        override public double Evaluate(List<double> operands)
        {
            double result = operands[0];
            for (int i = 1; i < operands.Count; i++)
            {
                result /= operands[i];
            }
            return result;
        }
    }
    public class Power : Operator
    {
        public Power()
        {
            this.symbol = "^";
            this.commutative = false;
            this.associative = false;
            this.precedence = 3;
        }
        override public double Evaluate(List<double> operands)
        {
            double result = operands[0];
            for (int i = 1; i < operands.Count; i++)
            {
                result = Math.Pow(result, operands[i]);
            }
            return result;
        }
    }
