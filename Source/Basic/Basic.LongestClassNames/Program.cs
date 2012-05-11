using System;
using System.Collections.Generic;
using System.Linq;

namespace Basic.LongestClassNames
{
    public class Program
    {
        public static void Main()
        {
            var assembly = typeof (string).Assembly;

            var orderedTypes = assembly.GetTypes()
                .Where(x => x.IsPublic)
                .OrderByDescending(x => x.Name.Length)
                .Select(x => string.Format("{0} [{1}]", x.Name, x.Name.Length))
                .Take(25).ToList();

            orderedTypes.ForEach(Console.WriteLine);
        }
    }
}
