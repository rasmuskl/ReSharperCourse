using System;
using System.Collections.Generic;

namespace Basic.InspectThis
{
    public class Inspecter
    {
        public static void Main()
        {
            int importantValue = 42;

            foreach (var value in GetAllValues(importantValue))
            {
                Console.WriteLine("Value: {0}", value);
            }

            importantValue *= 2;
            Console.Out.WriteLine(importantValue);
        }

        private static IEnumerable<int> GetAllValues(int importantValue)
        {
            var valueList = new List<int>();

            valueList.Add(importantValue);
            valueList.AddRange(GetOtherValues());

            return valueList;
        }

        private static IEnumerable<int> GetOtherValues()
        {
            return new[] { 1, 3, 5, 7 };
        }
    }
}