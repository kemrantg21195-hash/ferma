using System;
using System.Diagnostics;
using System.Numerics;

public static class FermatFactorization
{
    public static (BigInteger Factor1, BigInteger Factor2) Factorize(BigInteger n)
    {
        if (n <= 1)
            throw new ArgumentException("Число должно быть больше 1.", nameof(n));

        if (n.IsEven)
            return (2, n / 2);

        if (IsPerfectSquare(n, out BigInteger a))
            return (a, a);

        if (IsProbablePrime(n))
            return (1, n);

        a += 1;
        BigInteger b2 = a * a - n;
        BigInteger b;

        while (!IsPerfectSquare(b2, out b))
        {
            b2 += (a << 1) + 1;
            a += 1;
        }

        return (a - b, a + b);
    }

    private static bool IsPerfectSquare(BigInteger n, out BigInteger root)
    {
        if (n < 0)
        {
            root = BigInteger.Zero;
            return false;
        }

        root = Isqrt(n);
        return root * root == n;
    }

    
    private static BigInteger Isqrt(BigInteger n)
    {
        if (n == 0) return 0;
        if (n <= 3) return 1;

        int bitLength = (int)n.GetBitLength();
        BigInteger x = BigInteger.One << ((bitLength + 1) >> 1);

        while (true)
        {
            BigInteger y = (x + n / x) >> 1;
            if (y >= x)
                return x;
            x = y;
        }
    }

    private static bool IsProbablePrime(BigInteger n, int witnesses = 16)
    {
        if (n < 2) return false;
        if (n == 2 || n == 3) return true;
        if (n.IsEven) return false;

        BigInteger d = n - 1;
        int s = 0;
        while (d.IsEven)
        {
            d >>= 1;
            s++;
        }

        byte[] bytes = new byte[n.GetByteCount()];
        for (int i = 0; i < witnesses; i++)
        {
            Random.Shared.NextBytes(bytes);
            BigInteger a = (new BigInteger(bytes, isUnsigned: true) % (n - 3)) + 2;

            BigInteger x = BigInteger.ModPow(a, d, n);
            if (x == 1 || x == n - 1)
                continue;

            bool composite = true;
            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    composite = false;
                    break;
                }
            }

            if (composite)
                return false;
        }

        return true;
    }
}

public class Program
{
    public static void Main()
    {
        Console.Write("Введите целое число больше 1: ");
        string? input = Console.ReadLine()?.Trim();

        if (!BigInteger.TryParse(input, out BigInteger n) || n <= 1)
        {
            Console.WriteLine("Ошибка: введите корректное целое число больше 1.");
            return;
        }

        var sw = Stopwatch.StartNew();
        var (f1, f2) = FermatFactorization.Factorize(n);
        sw.Stop();

        Console.WriteLine($"Результат: {n} = {f1} * {f2}");
        Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");
    }
}