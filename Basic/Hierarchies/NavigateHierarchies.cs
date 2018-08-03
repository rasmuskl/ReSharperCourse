using System;

namespace Basic.Hierarchies
{
    public class NavigateHierarchies
    {
        public void Navigate()
        {
            IPet dog = new Dog();
            dog.Speak();
            PokePet(dog);

            var entity = dog as IEntity;
            Console.WriteLine("Dog: " + entity.Id);

            var cat = new Cat();
            PokePet(cat);
        }

        public void PokePet(IPet pet)
        {
            pet.Speak();
        }
    }
}