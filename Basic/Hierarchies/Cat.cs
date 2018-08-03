using System;

namespace Basic.Hierarchies
{
    public class Cat : AbstractAnimal
    {
        public override void Speak()
        {
            Purr();
        }

        public void Purr()
        {
            Console.WriteLine("Purrrrr");
        }
    }
}