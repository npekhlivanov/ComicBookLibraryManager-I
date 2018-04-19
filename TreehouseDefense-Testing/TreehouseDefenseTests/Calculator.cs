using System;
using System.Collections.Generic;

namespace TreehouseDefenseTests
{
    internal class Calculator
    {
        private double v;

        public Calculator(double v)
        {
            this.v = v;
        }

        public double Result { get; internal set; }

        internal void Add(double v)
        {
            throw new NotImplementedException();
        }
    }
}