Console.Write("Podaj wzór wielomianu n-stopnia(n > 1): W(x) = ");
Polynomial W = new(Console.ReadLine());

Console.WriteLine("W(x) = " + W);

var roots = W.Roots();

for (int i = 0; i < roots.Length; ++i)
    Console.WriteLine("z{0} = {1}", i, roots[i]);