using System;

namespace TreehouseDefense
{
    public class Point
    {
        public readonly int X;
        public readonly int Y;
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int DistanceTo(int x, int y)
        {
            return (int)Math.Sqrt(Math.Pow(X-x, 2) + Math.Pow(Y-y, 2));
        }
        
        public int DistanceTo(Point point)
        {
            return DistanceTo(point.X, point.Y);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        public override bool Equals(object obj)
        {
            var that = obj as Point;
            return that == null ? false : that.X == this.X && that.Y == this.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}