using System;
using System.Numerics;
namespace matrix;
public class Matrix
{
    public double[,] matrix;
    public bool isSquare { get { return matrix.GetLength(0) == matrix.GetLength(1); } }
    public void Print()
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write(matrix[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
    public Matrix(int rows, int columns)
    {
        matrix = new double[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = 0;
            }
        }
    }
    public Matrix(double[,] matrix)
    {
        this.matrix = matrix;
    }
    public static Matrix operator +(Matrix a, Matrix b)
    {
        double[,] result = new double[a.matrix.GetLength(0), a.matrix.GetLength(1)];
        for (int i = 0; i < a.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < a.matrix.GetLength(1); j++)
            {
                result[i, j] = a.matrix[i, j] + b.matrix[i, j];
            }
        }
        return new Matrix(result);
    }
    public static Matrix operator -(Matrix a, Matrix b)
    {
        double[,] result = new double[a.matrix.GetLength(0), a.matrix.GetLength(1)];
        for (int i = 0; i < a.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < a.matrix.GetLength(1); j++)
            {
                result[i, j] = a.matrix[i, j] - b.matrix[i, j];
            }
        }
        return new Matrix(result);
    }
    public static Matrix operator *(Matrix a, Matrix b)
    {
        double[,] result = new double[a.matrix.GetLength(0), b.matrix.GetLength(1)];
        for (int i = 0; i < a.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < b.matrix.GetLength(1); j++)
            {
                for (int k = 0; k < a.matrix.GetLength(1); k++)
                {
                    result[i, j] += a.matrix[i, k] * b.matrix[k, j];
                }
            }
        }
        return new Matrix(result);
    }
    public static Matrix operator *(Matrix a, double b)
    {
        double[,] result = new double[a.matrix.GetLength(0), a.matrix.GetLength(1)];
        for (int i = 0; i < a.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < a.matrix.GetLength(1); j++)
            {
                result[i, j] = a.matrix[i, j] * b;
            }
        }
        return new Matrix(result);
    }
    public static Matrix operator *(double a, Matrix b)
    {
        double[,] result = new double[b.matrix.GetLength(0), b.matrix.GetLength(1)];
        for (int i = 0; i < b.matrix.GetLength(0); i++)
        {
            for (int j = 0; j < b.matrix.GetLength(1); j++)
            {
                result[i, j] = b.matrix[i, j] * a;
            }
        }
        return new Matrix(result);
    }

    public double determinant
    {
        get
        {
            if (!isSquare)
            {
                throw new Exception("Matrix is not square");
            }
            if (matrix.GetLength(0) == 1)
            {
                return matrix[0, 0];
            }
            else if (matrix.GetLength(0) == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }
            else
            {
                double result = 0;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    result += matrix[0, i] * Cofactor(0, i);
                }
                return result;
            }
        }
    }
    private double Cofactor(int row, int column)
    {
        double[,] temp = new double[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];
        int tempRow = 0;
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            if (i == row)
            {
                continue;
            }
            int tempColumn = 0;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (j == column)
                {
                    continue;
                }
                temp[tempRow, tempColumn] = matrix[i, j];
                tempColumn++;
            }
            tempRow++;
        }
        return new Matrix(temp).determinant * Math.Pow(-1, row + column);
    }
    public Matrix Inverse
    {
        get
        {
            double[,] result = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    result[i, j] = Cofactor(i, j) / determinant;
                }
            }
            return new Matrix(result);
        }
    }
}