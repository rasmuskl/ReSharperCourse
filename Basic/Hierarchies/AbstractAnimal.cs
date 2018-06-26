namespace Basic.Hierarchies
{
    public abstract class AbstractAnimal : IPet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public abstract void Speak();
    }
}