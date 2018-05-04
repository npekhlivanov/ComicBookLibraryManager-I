using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreehouseDefense
{
    class StrongInvader : Invader
    {
        public override int Health { get; protected set; } = 2; // => 2; if lambda is used, no need to specify a setter with matching access modifier

        public StrongInvader(Path path) : base(path)
        {
        }

    }
}
