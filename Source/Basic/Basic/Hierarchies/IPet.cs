namespace Basic.Hierarchies
{
    public interface IPet : IEntity
    {
        string Name { get; set; }
        void Speak();
    }
}