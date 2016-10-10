using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    public static class Utily
    {
        static Random random = new Random();
        static int LastTag = 0;

        public static int Next()
        {
            return random.Next();
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        public static void ChangeSeed(long seed)
        {
            random = new Random((int)seed);
        }

        public static double Hypot(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static double Hypot(Tuple<double, double> v)
        {
            return Hypot(v.Item1, v.Item2);
        }

        public static long Hypot2(long x, long y)
        {
            return x * x + y * y;
        }

        public static double Hypot2(double x, double y)
        {
            return x * x + y * y;
        }

        public static int GetTag()
        {
            return LastTag++;
        }

        public static bool DoubleIsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < 1e-10;
        }

        public static bool DoubleIsEqual(double d1, double d2, double eps)
        {
            return Math.Abs(d1 - d2) < eps;
        }

        public static Tuple<T, T> MakePair<T>(T a, T b)
        {
            return new Tuple<T, T>(a, b);
        }

        public static Tuple<T1, T2> MakePair<T1, T2>(T1 a, T2 b)
        {
            return new Tuple<T1, T2>(a, b);
        }

        public static Tuple<double, double> Normalizing(Tuple<double, double> v, double Length)
        {
            double len = Hypot(v);
            if (len == 0)
                return v;
            return MakePair<double>(v.Item1 * Length / len, v.Item2 * Length / len);
        }
        public static bool IntToBool(int i)
        {
            return i != 0;
        }
    }
}
