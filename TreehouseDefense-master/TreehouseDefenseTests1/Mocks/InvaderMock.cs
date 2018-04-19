using System;
using TreehouseDefense;

namespace TreehouseDefenseTests1.Mocks
{
    public class InvaderMock : IInvader
    {
        public MapLocation Location { get; set; }

        public bool HasScored => false;

        public int Health { get; private set; } = 2;
            
        public bool IsNeutralized => false;

        public bool IsActive => true;

        public void DecreaseHealth(int factor)
        {
            Health -= factor;
        }

        public void Move()
        {
            throw new NotImplementedException();
        }
    }
}
