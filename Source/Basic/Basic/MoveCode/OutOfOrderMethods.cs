using System;

namespace Basic.MoveCode
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
            int theOtherNumber = 13 * theNumber;
            
            if (42 == theNumber)
            {
                try
                {
                    if (null != data)
                    {
                        Console.WriteLine("Processing data");
                    }
                }
                catch
                {
                    Console.WriteLine("Failed.");
                }

                Console.WriteLine("Mult is {0}.", theOtherNumber);
            }
            else
            {
                Console.WriteLine("It's not 42!");
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