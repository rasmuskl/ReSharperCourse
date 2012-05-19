using System;

namespace Basic.Rename
{
    public class H
    {
        public static void Main(string[] args)
        {
            int[] somethings = A(10);

            foreach (var something in somethings)
            {
                Console.WriteLine(something);
            }
        }

        public static int[] A(int j)
        {
            var b = new Random();
            var c = new int[j];

            for (var d = 0; d < j; d++)
            {
                c[d] = d + 1;
            }

            var e = c.Length;

            while (e > 1)
            {
                var f = b.Next(e);

                e = e - 1;

                if (e != f)
                {
                    var g = c[f];
                    c[f] = c[e];
                    c[e] = g;
                }
            }

            return c;
        }  
    }
}