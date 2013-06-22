using System;
using Microsoft.SPOT;

namespace tdw
{
    public class Point
    {

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }


    }

    public class PointAndSize
    {

        public PointAndSize(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            WIDTH = width;
            HEIGHT = height;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int WIDTH { get; set; }
        public int HEIGHT { get; set; }


    }
}
