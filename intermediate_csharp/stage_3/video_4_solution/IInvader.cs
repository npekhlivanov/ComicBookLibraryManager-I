namespace TreehouseDefense
{
    interface IInvader
    {
        MapLocation Location { get; }
        
        // True if the invader has reached the end of the path
        bool HasScored { get; }
        
        int Health { get; }
        
        bool IsNeutralized { get; }
        
        bool IsActive { get; }
        
        void Move();
        
        void DecreaseHealth(int factor);
    }
}