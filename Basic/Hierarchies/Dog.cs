using System;

namespace Basic.Hierarchies
{
    public class Dog : AbstractAnimal
    {
        public override void Speak()
        {
            Bark();
        }

        public void Bark()
        {
            Console.WriteLine("Wof wof!");
        }

        public void Fetch()
        {
            Console.WriteLine("Fetching stick!");
        }
    }
}