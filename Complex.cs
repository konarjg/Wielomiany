using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Complex
{
    public double Real { get; set; }
    public double Imaginal { get; set; }

    public double Module { get => Math.Sqrt(Real * Real + Imaginal * Imaginal); }
    public double Argument
    {
        get
        {
            if (Real == 0)
                return Imaginal / (-Imaginal) * (Math.PI / 2);
            else if (Real > 0)
                return Math.Atan(Imaginal / Real);
            else if (Real < 0)
                return Math.Atan(Imaginal / Real) + Math.PI;

            return 0;
        }
    }

    public Complex(double real, double imaginal)
    {
        Real = real;
        Imaginal = imaginal;
    }

    public Complex[] Roots(int n)
    {
        var result = new Complex[n];

        for (int k = 0; k < n; ++k)
        {
            var fi = Argument + 2 * Math.PI * k;
            var x = Math.Pow(Module, 1d / n) * Math.Cos(fi / n);
            var y = Math.Pow(Module, 1d / n) * Math.Sin(fi / n);

            result[k] = new Complex(x, y);
        }

        return result;
    }

    public Complex[] Factors()
    {
        var result = new List<Complex>();
        var r1 = this % new Complex(1, 0);
        var r2 = this % new Complex(-1, 0);

        if (Math.Round(r1.Real) == 0 && Math.Round(r1.Imaginal) == 0)
            result.Add(new Complex(1, 0));
        else if (Math.Round(r2.Real) == 0 && Math.Round(r2.Imaginal) == 0)
            result.Add(new Complex(-1, 0));

        for (int x = 0; x <= Math.Abs(Real); ++x)
        {
            for (int y = 0; y <= Math.Abs(Imaginal); ++y)
            {
                var factors = new Complex[0];

                if (x == 0 && y == 0)
                    continue;
                else if (x == 0)
                {
                    factors = new Complex[]
                    {
                        new Complex(x, -y),
                        new Complex(x, y)
                    };
                }
                else if (y == 0)
                {
                    factors = new Complex[]
                    {
                        new Complex(-x, y),
                        new Complex(x, y)
                    };
                }
                else
                {

                    factors = new Complex[]
                    {
                        new Complex(-x, -y),
                        new Complex(-x, y),
                        new Complex(x, -y),
                        new Complex(x, y)
                    };
                }

                for (int i = 0; i < factors.Length; ++i)
                {
                    var r = this % factors[i];

                    if (Math.Round(r.Real) == 0 && Math.Round(r.Imaginal) == 0)
                        result.Add(factors[i]);
                }
            }
        }

        result.Sort((z1, z2) => z1.CompareTo(z2));
        return result.ToArray();
    }

    public static Complex Parse(string s)
    {
        var x = 0d;
        s = s.Replace("<", "").Replace(">", "").Replace(" ", "");

        if (double.TryParse(s, out x))
            return new Complex(x, 0);
        else if (s.Contains('i'))
        {
            if (s == "i")
                return new Complex(0, 1);
            else if (s == "-i")
                return new Complex(0, -1);

            return new Complex(0, double.Parse(s.Replace("i", "")));
        }

        return new Complex(double.Parse(s.Split(';')[0]), double.Parse(s.Split(';')[1]));
    }

    public int CompareTo(Complex b)
    {
        if (this > b)
            return 1;
        else if (this < b)
            return -1;

        return 0;
    }

    public override string ToString()
    {
        if (Imaginal == 0)
            return Real.ToString();
        else if (Real == 0)
        {
            if (Imaginal == 1)
                return "i";
            else if (Imaginal == -1)
                return "-i";

            return string.Format("{0}i", Imaginal);
        }

        return string.Format("<{0}; {1}>", Real, Imaginal);
    }

    public string ToAlgebraic()
    {
        return string.Format("{0} + {1}i", Real, Imaginal);
    }

    public string ToTrigonometric()
    {
        return string.Format("{0} <cos({1}), sin({1})>", Real, Imaginal);
    }

    public string ToExponential()
    {
        return string.Format("{0}e^{1}i", Module, Argument);
    }

    public static bool operator ==(Complex a, Complex b)
    {
        return a.Real == b.Real && a.Imaginal == b.Imaginal;
    }

    public static bool operator !=(Complex a, Complex b)
    {
        return a.Real != b.Real || a.Imaginal != b.Imaginal;
    }

    public static bool operator >(Complex a, Complex b)
    {
        if (a.Real == 0)
            return a.Imaginal > b.Imaginal;
        else if (a.Imaginal == 0)
            return a.Real > b.Real;
        else if (a.Real == b.Real)
            return a.Imaginal > b.Imaginal;
        else if (a.Imaginal == b.Imaginal)
            return a.Real > b.Real;

        return a.Real > b.Real;
    }

    public static bool operator <(Complex a, Complex b)
    {
        if (a.Real == 0)
            return a.Imaginal < b.Imaginal;
        else if (a.Imaginal == 0)
            return a.Real < b.Real;
        else if (a.Real == b.Real)
            return a.Imaginal < b.Imaginal;
        else if (a.Imaginal == b.Imaginal)
            return a.Real < b.Real;

        return a.Real < b.Real;
    }

    public static bool operator >=(Complex a, Complex b)
    {
        return a > b || a == b;
    }

    public static bool operator <=(Complex a, Complex b)
    {
        return a < b || a == b;
    }

    public static Complex operator -(Complex a)
    {
        return new Complex(-a.Real, -a.Imaginal);
    }

    public static Complex operator !(Complex a)
    {
        var x = a.Real * a.Real + a.Imaginal * a.Imaginal;
        return new Complex(a.Real / x, -a.Imaginal / x);
    }

    public static Complex operator +(Complex a, Complex b)
    {
        return new Complex(a.Real + b.Real, a.Imaginal + b.Imaginal);
    }

    public static Complex operator -(Complex a, Complex b)
    {
        return a + -b;
    }

    public static Complex operator *(Complex a, Complex b)
    {
        return new Complex(a.Real * b.Real - a.Imaginal * b.Imaginal, a.Imaginal * b.Real + a.Real * b.Imaginal);
    }

    public static Complex operator /(Complex a, Complex b)
    {
        return a * !b;
    }

    public static Complex operator %(Complex a, Complex b)
    {
        var c = a / b;
        c.Real = Math.Floor(c.Real);
        c.Imaginal = Math.Floor(c.Imaginal);

        return a - b * c;
    }

    public static Complex operator ^(Complex a, double n)
    {
        var x = Math.Pow(a.Module, n) * Math.Cos(n * a.Argument);
        var y = Math.Pow(a.Module, n) * Math.Sin(n * a.Argument);

        return new Complex(x, y);
    }

    public static Complex operator +(Complex a, double b)
    {
        return new Complex(a.Real + b, a.Imaginal);
    }

    public static Complex operator -(Complex a, double b)
    {
        return new Complex(a.Real - b, a.Imaginal);
    }

    public static Complex operator *(Complex a, double b)
    {
        return new Complex(a.Real * b, a.Imaginal * b);
    }

    public static Complex operator /(Complex a, double b)
    {
        return new Complex(a.Real / b, a.Imaginal / b);
    }
}
