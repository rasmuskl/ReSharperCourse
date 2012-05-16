using System;
using System.Collections.Generic;
using System.Linq;

namespace Basic.ScratchPad
{
    public class OutOfOrderMethods
    {
        public void D()
        {
        }

        public void C()
        {
        }

        public void E(int theNumber, string[] data)
        {
            int theOtherNumber = theNumber * 13;

            if (theNumber == 42)
            {
                try
                {
                    if (data != null)
                    {
                        Console.WriteLine("Processing data");
                    }
                }
                catch
                {
                    Console.WriteLine("Failed.");
                    throw;
                }

                Console.WriteLine("It's 42!");
            }
            else
            {
                Console.WriteLine("Mult is {0}.", theOtherNumber);
            }
        }

        public void B()
        {
        }

        public void A()
        {
        }
    }
}