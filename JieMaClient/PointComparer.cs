using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public struct Point_Struct
{
    public string name;
    public Point point;
}

namespace JieMaClient
{
    internal class PointComparer : IComparer<Point_Struct>{ // Can be put outside, in this case, inner class may be better
        public int Compare(Point_Struct p1, Point_Struct p2)
        {
            if (p1.point.X > p2.point.X)
                return 1;
            if (p1.point.X == p2.point.X)
            {
                return 0;
            }
            return -1;
        }

        public int Compare(object x, object y)
        {
            Point p1 = (Point)x;
            Point p2 = (Point)y;
            if (p1.X > p2.X)
                return 1;
            if (p1.X == p2.X)
            {
                return 0;
            }
            return -1;
        }
    }
}