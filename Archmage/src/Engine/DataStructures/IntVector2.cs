using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Engine.DataStructures
{
    public class IntVector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntVector2()
        {
            X = 0;
            Y = 0;
        }

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVector2(IntVector2 obj)
        {
            X = obj.X;
            Y = obj.Y;
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.X + b.X, a.Y + b.Y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(Object obj)
        {
            IntVector2 other = obj as IntVector2;
            if ((object)other == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (X == other.X && Y == other.Y);
        }

        public int Magnitude()
        {
            int magnitude;
            if (Math.Abs(X) > Math.Abs(Y))
            {
                magnitude = Math.Abs(X);
                magnitude += Math.Abs(Y / 2);
            }
            else
            {
                magnitude = Math.Abs(Y);
                magnitude += Math.Abs(X / 2);
            }
            return magnitude;
        }

        public static List<IntVector2> LineBetweenPoints(IntVector2 a, IntVector2 b)
        {
            //Bresenham algorithm
            List<IntVector2> line = new List<IntVector2>();

            //Current point we are looking at
            IntVector2 currentPoint = new IntVector2(a);

            //Get the difference between the points
            IntVector2 delta = new IntVector2(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
            int errorValue = delta.X - delta.Y;

            IntVector2 step = new IntVector2(1, 1);

            if (a.X > b.X)
            {
                step.X = -1;
            }

            if (a.Y > b.Y)
            {
                step.Y = -1;
            }

            while (!currentPoint.Equals(b))
            {
                line.Add(new IntVector2(currentPoint));
                int e2 = 2 * errorValue;
                if (e2 > (0 - delta.Y))
                {
                    errorValue = errorValue - delta.Y;
                    currentPoint.X += step.X;
                }
                if (e2 < delta.X)
                {
                    errorValue = errorValue + delta.X;
                    currentPoint.Y += step.Y;
                }
            }

            line.Add(new IntVector2(b));

            return line;
        }

        public static List<IntVector2> ExtendedLineBetweenPoints(IntVector2 a, IntVector2 b, int length)
        {
            //Bresenham algorithm
            List<IntVector2> line = new List<IntVector2>();

            //Current point we are looking at
            IntVector2 currentPoint = new IntVector2(a);

            //Get the difference between the points
            IntVector2 delta = new IntVector2(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
            int errorValue = delta.X - delta.Y;

            IntVector2 step = new IntVector2(1, 1);

            if (a.X > b.X)
            {
                step.X = -1;
            }

            if (a.Y > b.Y)
            {
                step.Y = -1;
            }

            while (line.Count != length)
            {
                line.Add(new IntVector2(currentPoint));
                int e2 = 2 * errorValue;
                if (e2 > (0 - delta.Y))
                {
                    errorValue = errorValue - delta.Y;
                    currentPoint.X += step.X;
                }
                if (e2 < delta.X)
                {
                    errorValue = errorValue + delta.X;
                    currentPoint.Y += step.Y;
                }
            }

            return line;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }

}
