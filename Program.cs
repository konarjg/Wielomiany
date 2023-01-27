Console.Write("Podaj wzór wielomianu n-stopnia(n > 1): W(x) = ");
Polynomial W = new(Console.ReadLine());

Console.WriteLine("W(x) = " + W);
Console.WriteLine("Q(x) = " + PolynomialUtils.Horner(W, new Polynomial("x - i")));