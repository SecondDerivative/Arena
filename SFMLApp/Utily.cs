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

        public static int Hypot2(int x, int y)
        {
            return x * x + y * y;
        }

        public static double Hypot2(double x, double y)
        {
            return x * x + y * y;
        }

        public static string GetTag(int len)
        {
            string ans = "";
            for (int i = 0; i < len; ++i)
                ans += (char)('a' + Next() % 26);
            return ans;
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
    }
}
