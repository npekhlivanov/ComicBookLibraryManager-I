namespace TreehouseDefense
{
    public interface IMappable
    {
        MapLocation Location { get; }
    }
    public interface IMovable
    {
        void Move();
    }

    public interface IInvader : IMappable, IMovable
    {
        bool HasScored { get; }
        int Health { get; }
        bool IsNeutralized { get; }
        bool IsActive { get; }
        void DecreaseHealth(int factor);
    }
}
