using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PolynomialUtils
{
    public static Complex ModifierOfElement(string element)
    {
        if (element.Contains('x'))
        {
            if (element.Contains('^'))
            {
                var n = PowerOfElement(element);

                if (element == string.Format("x^{0}", n))
                    return new Complex(1, 0);
                else if (element == string.Format("-x^{0}", n))
                    return new Complex(-1, 0);

                return Complex.Parse(element.Split('x')[0]);
            }

            if (element == "x")
                return new Complex(1, 0);
            else if (element == "-x")
                return new Complex(-1, 0);

            return Complex.Parse(element.Split('x')[0]);
        }

        return Complex.Parse(element);
    }

    public static string ParseElement(Complex a, double n)
    {
        if (n == 0)
            return a.ToString();
        else if (n == 1)
        {
            if (a.Real == 1 && a.Imaginal == 0)
                return "x";
            else if (a.Real == 1 && a.Imaginal == 0)
                return "-x";

            return string.Format("{0}x", a);
        }

        if (a.Real == 1 && a.Imaginal == 0)
            return string.Format("x^{0}", n);
        else if (a.Real == -1 && a.Imaginal == 0)
            return string.Format("-x^{0}", n);

        return string.Format("{0}x^{1}", a, n);
    }

    public static double PowerOfElement(string element)
    {
        if (element.Contains('x'))
        {
            if (element.Contains('^'))
                return double.Parse(element.Split('^')[1]);

            return 1;
        }

        return 0;
    }

    public static void AddPluses(ref string formula)
    {
        var s = "";

        for (int i = 0; i < formula.Length; ++i)
        {
            if (formula[i] == '-' && formula[i - 1] != '<' && formula[i - 1] != ';')
                s += "+" + formula[i];
            else
                s += formula[i];
        }

        formula = s;
    }

    public static void AddSpaces(ref string formula)
    {
        var s = "";

        for (int i = 0; i < formula.Length; ++i)
        {
            if (formula[i] == '+')
                s += " " + formula[i] + " ";
            else if (formula[i] == ';' && formula[i + 1] != ' ')
                s += formula[i] + " ";
            else
                s += formula[i];
        }

        formula = s;
    }

    public static int CompareElements(string a, string b)
    {
        if (PowerOfElement(a) < PowerOfElement(b))
            return 1;
        else if (PowerOfElement(a) > PowerOfElement(b))
            return -1;

        return 0;
    }

    public static string[] PrepareForHorner(Polynomial a)
    {
        var tokens = a.Elements.ToList();

        int k = 0;

        for (double n = a.MaxPower; n >= 2; --n)
        {
            if (tokens.Find(x => PowerOfElement(x) == n) == null)
                tokens.Insert(k, string.Format("0x^{0}", n));

            ++k;
        }

        if (tokens.Find(x => PowerOfElement(x) == 1) == null)
        {
            tokens.Insert(k, "0x");
            ++k;
        }

        if (tokens.Find(x => PowerOfElement(x) == 0) == null)
            tokens.Insert(k, "0");

        return tokens.ToArray();
    }

    public static Polynomial Derivative(Polynomial a)
    {
        var modifiers = new List<Complex>();
        var powers = new List<double>();
        var modifier = new Complex(1, 0);
        var power = 0d;
        var formula = "";

        for (int i = 0; i < a.Elements.Length; ++i)
        {
            power = a.Powers[i];
            modifier = a.Modifiers[i];

            if (power == 0)
                continue;

            modifier *= power;
            --power;

            powers.Add(power);
            modifiers.Add(modifier);
        }

        for (int i = 0; i < powers.Count; ++i)
            formula += ParseElement(modifiers[i], powers[i]) + "+";

        formula = formula.Remove(formula.Length - 1);
        AddSpaces(ref formula);

        return new Polynomial(formula);
    }

    public static Polynomial Horner(Polynomial a, Polynomial b)
    {
        var result = new List<Complex>();
        var formula = "";
        var x0 = -ModifierOfElement(b.Elements[1]);
        var elements = PrepareForHorner(a);

        result.Add(ModifierOfElement(a.Elements[0]));
        var k = 1;
        var n = a.MaxPower - 1; 

        for (int i = 0; i < elements.Length; ++i)
        {
            var z = x0 * result[k - 1] + ModifierOfElement(elements[i]);

            if (n == 0)
                break;

            result.Add(z);
            ++k;
            --n;
        }

        n = a.MaxPower - 1;

        for (int i = 0; i < result.Count; ++i)
        {
            if (Math.Round(result[i].Real) == 0 && Math.Round(result[i].Imaginal) == 0)
                continue;

            formula += ParseElement(result[i], n) + "+";
            --n;
        }

        formula = formula.Remove(formula.Length - 1);
        AddSpaces(ref formula);

        return new Polynomial(formula);
    }
}

public class Polynomial
{
    public string Formula { get; private set; }
    public double MaxPower { get; private set; }
    public string[] Elements { get => Formula.Replace(" ", "").Split('+'); }

    public Complex[] Modifiers
    {
        get
        {
            var result = new Complex[Elements.Length];

            for (int i = 0; i < result.Length; ++i)
                result[i] = PolynomialUtils.ModifierOfElement(Elements[i]);

            return result;
        }
    }

    public double[] Powers
    {
        get
        {
            var result = new double[Elements.Length];

            for (int i = 0; i < result.Length; ++i)
                result[i] = PolynomialUtils.PowerOfElement(Elements[i]);

            return result;
        }
    }

    public Polynomial(string formula)
    {
        MaxPower = 0;

        formula = formula.Replace(" ", ""); ;
        PolynomialUtils.AddPluses(ref formula); 
        Formula = formula;

        Console.WriteLine(formula);

        for (int i = 0; i < Elements.Length; ++i)
        {
            if (PolynomialUtils.PowerOfElement(Elements[i]) > MaxPower)
                MaxPower = PolynomialUtils.PowerOfElement(Elements[i]);
        }

        var list = Elements.ToList();
        list.Sort((x, y) => PolynomialUtils.CompareElements(x, y));

        formula = "";

        for (int i = 0; i < list.Count; ++i)
            formula += list[i] + "+";

        formula = formula.Remove(formula.Length - 1);
        PolynomialUtils.AddSpaces(ref formula);
        Formula = formula;
    }

    public Complex Value(Complex x)
    {
        Complex r = new(0, 0);

        for (int i = 0; i < Modifiers.Length; ++i)
            r += Modifiers[i] * (x ^ Powers[i]);

        return r;
    }

    public Complex Laguerre()
    {
        var n = new Complex(MaxPower, 0);
        Complex a = new(0, 0);
        Complex x = new(-100, 0);
        Complex y = new(0, 0);

        for (int i = 0; i < 1e9; ++i)
        {
            y = Value(x);

            if (Math.Round(y.Module) == 0)
                return x;

            var df = PolynomialUtils.Derivative(this);
            var G = df.Value(x) / Value(x);
            var H = (G ^ 2) - (PolynomialUtils.Derivative(df).Value(x) / Value(x));

            var d1 = G + (((H * n - (G^2)) * (n - 1))^0.5d);
            var d2 = G - (((H * n - (G^2)) * (n - 1))^0.5d);

            if (d1 > d2)
                a = n / d1;
            else
                a = n / d2;

            x = x - a;
            y = Value(x);

            if (Math.Round(y.Module) == 0)
                return x;
        }

        return x;
    }

    public Complex[] Roots()
    {
        var elements = PolynomialUtils.PrepareForHorner(this);
        Complex a, b, c;

        if (MaxPower == 1)
        {
            a = PolynomialUtils.ModifierOfElement(elements[0]);
            b = PolynomialUtils.ModifierOfElement(elements[1]);

            return new Complex[] { -b / a };
        }
        else if (MaxPower == 2)
        {
            a = PolynomialUtils.ModifierOfElement(elements[0]);
            b = PolynomialUtils.ModifierOfElement(elements[1]);
            c = PolynomialUtils.ModifierOfElement(elements[2]);

            var delta = (b ^ 2) - a * 4 * c;

            if (Math.Round(delta.Module) == 0)
                return new Complex[] { -b / (a * 2) };

            var x1 = (-b + (delta ^ 0.5d)) / (a * 2);
            var x2 = (-b - (delta ^ 0.5d)) / (a * 2);

            return new Complex[] { x1, x2 };
        }

        var result = new List<Complex>();

        var x0 = Laguerre();
        x0.Real = Math.Round(x0.Real);
        x0.Imaginal = Math.Round(x0.Imaginal);
        var formula = string.Format("x + {0}", -x0);

        result.Add(x0);
        result.AddRange(PolynomialUtils.Horner(this, new Polynomial(formula)).Roots());

        return result.ToArray();
    }

    public override string ToString()
    {
        return Formula;
    }
}
