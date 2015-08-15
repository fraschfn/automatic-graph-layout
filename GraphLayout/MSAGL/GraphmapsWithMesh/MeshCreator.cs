﻿using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace Microsoft.Msagl.GraphmapsWithMesh
{
    class MeshCreator
    {

        private static void CreateFourRaysPerVertex(Tiling G, int maxX, int maxY)
        {
            int nodeIndex;
            for (nodeIndex = 0; nodeIndex < G.N; nodeIndex++)
            {
                int x = G.VList[nodeIndex].XLoc, y = G.VList[nodeIndex].YLoc;
                int y_new;

                var xNew = x + 1;
                y_new = y;
                if (xNew >= 0 && xNew <= maxX && y_new >= 0 && y_new <= maxY)
                {
                    LineSegment ls = new LineSegment(new Point(x, y), new Point(xNew, y_new));
                    G.VList[nodeIndex].SegmentList.Add(ls, true);
                }
                xNew = x;
                y_new = y + 1;
                if (xNew >= 0 && xNew <= maxX && y_new >= 0 && y_new <= maxY)
                {
                    LineSegment ls = new LineSegment(new Point(x, y), new Point(xNew, y_new));
                    G.VList[nodeIndex].SegmentList.Add(ls, true);
                }
                xNew = x - 1;
                y_new = y;
                if (xNew >= 0 && xNew <= maxX && y_new >= 0 && y_new <= maxY)
                {
                    LineSegment ls = new LineSegment(new Point(x, y), new Point(xNew, y_new));
                    G.VList[nodeIndex].SegmentList.Add(ls, true);
                }
                xNew = x;
                y_new = y - 1;
                if (xNew >= 0 && xNew <= maxX && y_new >= 0 && y_new <= maxY)
                {
                    LineSegment ls = new LineSegment(new Point(x, y), new Point(xNew, y_new));
                    G.VList[nodeIndex].SegmentList.Add(ls, true);
                }
            }
        }


        static Point GrowOneUnit(LineSegment ls, int maxX, int maxY)
        {
            int xNew = 0;
            int yNew = 0;
            if ((int)ls.Start.X == (int)ls.End.X && (int)ls.Start.Y < (int)ls.End.Y)
            {
                xNew = (int)ls.End.X;
                yNew = (int)ls.End.Y + 1;
            }
            if ((int)ls.Start.X == (int)ls.End.X && (int)ls.Start.Y > (int)ls.End.Y)
            {
                xNew = (int)ls.End.X;
                yNew = (int)ls.End.Y - 1;
            }

            if ((int)ls.Start.Y == (int)ls.End.Y && (int)ls.Start.X < (int)ls.End.X)
            {
                xNew = (int)ls.End.X + 1;
                yNew = (int)ls.End.Y;
            }
            if ((int)ls.Start.Y == (int)ls.End.Y && (int)ls.Start.X > (int)ls.End.X)
            {
                xNew = (int)ls.End.X - 1;
                yNew = (int)ls.End.Y;
            }
            if (!(xNew >= 0 && xNew <= maxX && yNew >= 0 && yNew <= maxY)) return new Point(-1, -1);
            return new Point(xNew, yNew);
        }


        static int FindVertexClosestToSegmentEnd(Tiling g, LineSegment ls)
        {
            double distance = double.MaxValue;
            int nearestVertex = -1;

            if ((int)ls.Start.X == (int)ls.End.X) //vertical
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].XLoc != (int)ls.Start.X) continue;
                    if (ls.Start.Y < ls.End.Y && g.VList[nodeIndex].YLoc >= ls.Start.Y && g.VList[nodeIndex].YLoc < ls.End.Y)
                    {
                        if (distance > ls.End.Y - g.VList[nodeIndex].YLoc)
                        {
                            distance = ls.End.Y - g.VList[nodeIndex].YLoc;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (ls.Start.Y > ls.End.Y && g.VList[nodeIndex].YLoc <= ls.Start.Y && g.VList[nodeIndex].YLoc > ls.End.Y)
                    {
                        if (distance > g.VList[nodeIndex].YLoc - ls.End.Y)
                        {
                            distance = g.VList[nodeIndex].YLoc - ls.End.Y;
                            nearestVertex = nodeIndex;
                        }
                    }
                }
            }
            if ((int)ls.Start.Y == (int)ls.End.Y) //horizontal
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].YLoc != (int)ls.Start.Y) continue;
                    if (ls.Start.X < ls.End.X && g.VList[nodeIndex].XLoc >= ls.Start.X && g.VList[nodeIndex].XLoc < ls.End.X)
                    {
                        if (distance > ls.End.X - g.VList[nodeIndex].XLoc)
                        {
                            distance = ls.End.X - g.VList[nodeIndex].XLoc;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (ls.Start.X > ls.End.X && g.VList[nodeIndex].XLoc <= ls.Start.X && g.VList[nodeIndex].XLoc > ls.End.X)
                    {
                        if (distance > g.VList[nodeIndex].XLoc - ls.End.X)
                        {
                            distance = g.VList[nodeIndex].XLoc - ls.End.X;
                            nearestVertex = nodeIndex;
                        }
                    }
                }
            }
            if (distance == double.MaxValue)
                Console.WriteLine("No vertex Found Error");
            return nearestVertex;
        }


        static int FindClosestVertexWhileWalkingToStart(Tiling g, LineSegment ls, Point p)
        {
            double distance = double.MaxValue;
            int nearestVertex = -1;

            if ((int)ls.Start.X == (int)ls.End.X) //vertical
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].XLoc != (int)ls.Start.X) continue;
                    if (ls.Start.Y <= p.Y && g.VList[nodeIndex].YLoc >= ls.Start.Y && g.VList[nodeIndex].YLoc <= p.Y)
                    {
                        if (distance > p.Y - g.VList[nodeIndex].YLoc)
                        {
                            distance = p.Y - g.VList[nodeIndex].YLoc;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (ls.Start.Y >= p.Y && g.VList[nodeIndex].YLoc <= ls.Start.Y && g.VList[nodeIndex].YLoc >= p.Y)
                    {
                        if (distance > g.VList[nodeIndex].YLoc - p.Y)
                        {
                            distance = g.VList[nodeIndex].YLoc - p.Y;
                            nearestVertex = nodeIndex;
                        }
                    }
                }
            }
            if ((int)ls.Start.Y == (int)ls.End.Y) //horizontal
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].YLoc != (int)ls.Start.Y) continue;
                    if (ls.Start.X <= p.X && g.VList[nodeIndex].XLoc >= ls.Start.X && g.VList[nodeIndex].XLoc <= p.X)
                    {
                        if (distance > p.X - g.VList[nodeIndex].XLoc)
                        {
                            distance = p.X - g.VList[nodeIndex].XLoc;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (ls.Start.X >= p.X && g.VList[nodeIndex].XLoc <= ls.Start.X && g.VList[nodeIndex].XLoc >= p.X)
                    {
                        if (distance > g.VList[nodeIndex].XLoc - p.X)
                        {
                            distance = g.VList[nodeIndex].XLoc - p.X;
                            nearestVertex = nodeIndex;
                        }
                    }
                }
            }
            if (distance == double.MaxValue)
                Console.WriteLine("No vertex Found Error");
            return nearestVertex;
        }

        static int FindClosestVertexWhileWalkingToEnd(Tiling g, LineSegment ls, Point p)
        {
            double distance = double.MaxValue;
            var nearestVertex = -1;

            if ((int)ls.Start.X == (int)ls.End.X) //vertical
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].XLoc != (int)ls.Start.X) continue;
                    if (p.Y <= ls.End.Y && g.VList[nodeIndex].YLoc >= p.Y && g.VList[nodeIndex].YLoc <= ls.End.Y)
                    {
                        if (distance > g.VList[nodeIndex].YLoc - p.Y)
                        {
                            distance = g.VList[nodeIndex].YLoc - p.Y;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (p.Y >= ls.End.Y && g.VList[nodeIndex].YLoc <= p.Y && g.VList[nodeIndex].YLoc >= ls.End.Y)
                    {
                        if (distance > p.Y - g.VList[nodeIndex].YLoc)
                        {
                            distance = p.Y - g.VList[nodeIndex].YLoc;
                            nearestVertex = nodeIndex;
                        }
                    }

                }
            }
            if ((int)ls.Start.Y == (int)ls.End.Y) //horizontal
            {
                for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                {
                    if (g.VList[nodeIndex].YLoc != (int)ls.Start.Y) continue;
                    if (p.X <= ls.End.X && g.VList[nodeIndex].XLoc >= p.X && g.VList[nodeIndex].XLoc <= ls.End.X)
                    {
                        if (distance > g.VList[nodeIndex].XLoc - p.X)
                        {
                            distance = g.VList[nodeIndex].XLoc - p.X;
                            nearestVertex = nodeIndex;
                        }
                    }
                    if (p.X >= ls.End.X && g.VList[nodeIndex].XLoc <= p.X && g.VList[nodeIndex].XLoc >= ls.End.X)
                    {
                        if (distance > p.X - g.VList[nodeIndex].XLoc)
                        {
                            distance = p.X - g.VList[nodeIndex].XLoc;
                            nearestVertex = nodeIndex;
                        }
                    }
                }
            }

            return nearestVertex;
        }

        public static void CreateCompetitionMesh(Tiling g, Dictionary<int, Node> idToNode, int maxX, int maxY)
        {

            //for each node, create four line segments
            CreateFourRaysPerVertex(g, maxX, maxY);
            Console.WriteLine("Creating Mesh of size " + Math.Max(maxX, maxY));

            Dictionary<LineSegment, int> removeList = new Dictionary<LineSegment, int>();
            Dictionary<LineSegment, int> addList = new Dictionary<LineSegment, int>();

            for (int iteration = 0; iteration <= Math.Max(maxX, maxY); iteration++)
            {
                //for each line segment check whether it hits any other segment or point
                //if so then create a new junction at that point
                for (int nodeIndex1 = 0; nodeIndex1 < g.N; nodeIndex1++)
                {
                    foreach (LineSegment ls1 in g.VList[nodeIndex1].SegmentList.Keys)
                    {
                        if (g.VList[nodeIndex1].SegmentList[ls1] == false) continue;

                        for (int nodeIndex2 = 0; nodeIndex2 < g.N; nodeIndex2++)
                        {
                            if (nodeIndex1 == nodeIndex2) continue;
                            foreach (LineSegment ls2 in g.VList[nodeIndex2].SegmentList.Keys)
                            {


                                if (MsaglUtilities.PointIsOnAxisAlignedSegment(ls2, ls1.End))
                                {





                                    //if they are parallel then create an edge
                                    if (((int)ls1.Start.X == (int)ls1.End.X && (int)ls2.Start.X == (int)ls2.End.X)
                                        || ((int)ls1.Start.Y == (int)ls1.End.Y && (int)ls2.Start.Y == (int)ls2.End.Y))
                                    {

                                        if ((int)ls1.End.X == (int)ls2.Start.X && (int)ls1.End.Y == (int)ls2.Start.Y) continue;

                                        int a = FindVertexClosestToSegmentEnd(g, ls1);
                                        int b = FindVertexClosestToSegmentEnd(g, ls2);

                                        if (a == b)
                                        {
                                            Console.WriteLine("degenerate parallel collision");
                                            //Point coordinates multiplied by 4 so that this condition does not arise.
                                        }
                                        if (g.AddEdge(a, b))
                                        {
                                            LineSegment l = new LineSegment(ls1.Start, ls2.Start);
                                            if (!addList.ContainsKey(l)) addList.Add(l, -1);
                                            l = new LineSegment(ls2.Start, ls1.Start);
                                            if (!addList.ContainsKey(l)) addList.Add(l, -1);

                                            //PARTIAL SOLUTION 2 
                                            //if (!addList.ContainsKey(ls1)) addList.Add(ls1, -1);
                                            //if (!addList.ContainsKey(ls2)) addList.Add(ls2, -1);


                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1)) removeList.Add(ls1, nodeIndex1);
                                            if (!removeList.ContainsKey(ls2)) removeList.Add(ls2, nodeIndex2);
                                        }
                                    }

                                    //create a new node at the intersection point                 
                                    else
                                    {
                                        if (MsaglUtilities.PointIsOnAxisAlignedSegment(ls2, ls1.End) &&
                                            MsaglUtilities.PointIsOnAxisAlignedSegment(ls1, ls2.End) &&
                                            nodeIndex1 > nodeIndex2) continue;

                                        if (g.GetNode((int)ls1.End.X, (int)ls1.End.Y) == -1)
                                        {

                                            //create the edges
                                            int a = FindClosestVertexWhileWalkingToStart(g, ls2, ls1.End);
                                            int b = FindClosestVertexWhileWalkingToEnd(g, ls2, ls1.End);
                                            int c = FindVertexClosestToSegmentEnd(g, ls1);
                                            int d = g.NumOfnodes;

                                            g.VList[d] = new Vertex((int)ls1.End.X, (int)ls1.End.Y) { Id = d };
                                            g.NumOfnodes++;

                                            if (a >= 0) g.AddEdge(a, d);
                                            if (b >= 0) g.AddEdge(b, d);
                                            if (c >= 0) g.AddEdge(c, d);
                                            if (a == b)
                                                Console.WriteLine("degenerate orthogonal collision");
                                            if (a >= 0 && b >= 0) g.RemoveEdge(a, b);

                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1))
                                            {
                                                removeList.Add(ls1, nodeIndex1);
                                                if (!addList.ContainsKey(ls1)) addList.Add(ls1, -1);
                                            }

                                        }
                                        else
                                        {
                                            int a = g.GetNode((int)ls1.End.X, (int)ls1.End.Y);
                                            int b = FindVertexClosestToSegmentEnd(g, ls1);
                                            if (b == -1)
                                                Console.WriteLine("vertex not found error");
                                            g.AddEdge(a, b);


                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1))
                                            {
                                                removeList.Add(ls1, nodeIndex1);
                                                if (!addList.ContainsKey(ls1)) addList.Add(ls1, -1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var s in removeList)
                {
                    g.VList[s.Value].SegmentList.Remove(s.Key);
                }
                removeList.Clear();
                foreach (var s in addList)
                {
                    if (s.Value >= 0) g.VList[s.Value].SegmentList.Add(s.Key, true);
                    else g.VList[g.GetNode((int)s.Key.Start.X, (int)s.Key.Start.Y)].SegmentList.Add(s.Key, false);
                }
                addList.Clear();


                int nodeIndex;
                for (nodeIndex = 0; nodeIndex < g.N; nodeIndex++)
                {
                    foreach (LineSegment ls in g.VList[nodeIndex].SegmentList.Keys)
                    {

                        if (g.VList[nodeIndex].SegmentList[ls] == false) continue;
                        Core.Geometry.Point nextPoint = GrowOneUnit(ls, maxX + 1, maxY + 1);
                        if (nextPoint.X >= 0)
                        {
                            LineSegment l_new = new LineSegment(ls.Start, nextPoint);


                            if (!addList.ContainsKey(l_new)) addList.Add(l_new, nodeIndex);
                            if (!removeList.ContainsKey(ls)) removeList.Add(ls, nodeIndex);
                        }

                    }
                }

                foreach (var s in removeList)
                {
                    g.VList[s.Value].SegmentList.Remove(s.Key);
                }
                removeList.Clear();
                foreach (var s in addList)
                {
                    if (s.Value >= 0) g.VList[s.Value].SegmentList.Add(s.Key, true);
                    else g.VList[g.GetNode((int)s.Key.Start.X, (int)s.Key.Start.Y)].SegmentList.Add(s.Key, false);
                }
                addList.Clear();

            }

            FixMesh(g);
            Console.WriteLine("Mesh Created");
        }

        public static void FastCompetitionMesh(Tiling g, Dictionary<int, Node> idToNode, int maxX, int maxY)
        {
            /*
            // for testing purpose
            g.N = 10;
            g.VList[0].XLoc = 5; g.VList[0].YLoc = 1;
            g.VList[1].XLoc = 1; g.VList[1].YLoc = 5;
            g.VList[2].XLoc = 2; g.VList[2].YLoc = 7;
            g.VList[3].XLoc = 3; g.VList[3].YLoc = 3;
            g.VList[4].XLoc = 5; g.VList[4].YLoc = 3;
            g.VList[5].XLoc = 6; g.VList[5].YLoc = 6;
            g.VList[6].XLoc = 3; g.VList[6].YLoc = 2;
            g.VList[7].XLoc = 9; g.VList[7].YLoc = 8;
            g.VList[8].XLoc = 8; g.VList[8].YLoc = 0;
            g.VList[9].XLoc = 0; g.VList[9].YLoc = 0;
            */

            double[] Px = new double[g.N];
            double[] Py = new double[g.N];
            int[] Pid = new int[g.N];
            double[] temp = new double[g.N];
            for (int i = 0; i < g.N; i++)
            {
                Px[i] = g.VList[i].XLoc;
                Py[i] = g.VList[i].YLoc;
                Pid[i] = i;
                temp[i] = g.VList[i].YLoc; 
            }
            

             

            //sort the points by y coordinate
            iterativeMergesort(temp, Px, Py, Pid);
             
    
            //find the closest point in the Xth cone
            Dictionary<int, int> Neighbors1 = ManhattanNearestNeighbor(Px, Py, Pid, 1, maxX,   maxY);
            Dictionary<int, int> Neighbors8 = ManhattanNearestNeighbor(Px, Py, Pid, 8, maxX, maxY);
            Dictionary<int, int> Neighbors4 = ManhattanNearestNeighbor(Px, Py, Pid, 4, maxX, maxY);
            Dictionary<int, int> Neighbors5 = ManhattanNearestNeighbor(Px, Py, Pid, 5, maxX, maxY);

            //create the bounding box             
            int a = g.InsertVertex(0, 0);
            int b = g.InsertVertex(0, maxY);
            int c = g.InsertVertex(maxX, maxY);
            int d = g.InsertVertex(maxX, 0);
            g.AddEdge(a, b);
            g.AddEdge(b, c);
            g.AddEdge(c, d);
            g.AddEdge(d, a);
            //create segments
            LineSegment l = new LineSegment(0, 0, 0, maxY);
            g.VList[a].topRay = new Ray(l) { dead = true };            
            l = new LineSegment(0, maxY, maxX, maxY);
            g.VList[b].rightRay = new Ray(l);
            l = new LineSegment(maxX, maxY, maxX, 0);
            g.VList[c].bottomRay = new Ray(l);
            l = new LineSegment(maxX, 0, 0, 0);
            g.VList[d].leftRay = new Ray(l);

            /*//Process each top ray
            for (int i = 0; i < g.N; i++)
            {
                if (g.VList[i].topRay == null)
                {

                }
            }*/


        }
        public static Dictionary<int, int> ManhattanNearestNeighbor(double[] X, double[] Y, int[] ID, int ConeId, double maxX, double maxY)
        {
            double[] a = new double[ID.Length];
            double[] Px = new double[ID.Length];
            double[] Py = new double[ID.Length];
            int[] Pid = new int[ID.Length];

            for (int i = 0; i < ID.Length; i++){
                if(ConeId == 4){
                    Px[i] = X[i];
                    Py[i] = Y[i];
                }
                if(ConeId == 1){
                    Px[i] = -X[i]+maxX;
                    Py[i] = Y[i];
                }
                if(ConeId == 8){
                    Px[i] = -X[i]+maxX;
                    Py[i] = -Y[i]+maxY;
                }
                if(ConeId == 5){
                    Px[i] = X[i];
                    Py[i] = -Y[i]+maxY;
                }
                Pid[i] = ID[i];
                a[i] = Px[i] + Py[i]; 

            }

            Dictionary<int, int> neighborlist = new Dictionary<int, int>();
            double[] from = a;
            double[] to = new double[a.Length];
 
            int[] toPid = new int[a.Length];
            double[] toPx = new double[a.Length];
            double[] toPy = new double[a.Length];

            Dictionary<int, double> IdToX = new Dictionary<int, double>();
            for (int k = 0; k < a.Length; k++) IdToX.Add(Pid[k], Px[k]);
            Dictionary<int, double> IdToY = new Dictionary<int, double>();
            for (int k = 0; k < a.Length; k++) IdToY.Add(Pid[k], Py[k]);
            Dictionary<int, int> PosToId = new Dictionary<int, int>();
            for (int k = 0; k < a.Length; k++) PosToId.Add(k,Pid[k]);


            for (int blockSize = 1; blockSize < a.Length; blockSize *= 2)
            {
                for (int start = 0; start < a.Length; start += 2 * blockSize)
                    FindNeighbor(from, to, start, start + blockSize, start + 2 * blockSize, Px, Py, Pid, toPx, toPy, toPid, neighborlist, IdToX, IdToY, PosToId);
            }

            return neighborlist;
        }
        private static void FindNeighbor(double[] from, double[] to, int lo, int mid, int hi, double[] Px, double[] Py, int[] Pid, double[] toPx, double[] toPy, int[] toPid, Dictionary<int, int> neighborlist, Dictionary<int, double> IdToX, Dictionary<int, double> IdToY, Dictionary<int, int> PosToId)
        {
            if (mid > from.Length) mid = from.Length;
            if (hi > from.Length) hi = from.Length;
            int i = lo, j = mid;
            //sort all the points according to x+y
            for (int k = lo; k < hi; k++)
            {
                if (i == mid) { Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid); j++; }
                else if (j == hi) { Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid); i++; }
                else if (from[j] < from[i]) { Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid); j++; }
                else if (from[i] == from[j] && Px[i] < Px[j]) { Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid); i++; }
                else if (from[i] == from[j] && Px[i] >= Px[j]) { Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid); j++; }
                else { Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid); i++; }
            }

 

 


            //foreach point in x+y order
            double LargestXMinusY = double.MinValue;
            int CandidateNeighborId = -1;
            for (int k = lo; k < hi; k++)
            {
                int currentPointId = toPid[k];
 
                

                //find the neighbor
                if (currentPointId != CandidateNeighborId)
                {// if the point is on lower half
                    if (CandidateNeighborId >= 0)
                    {
 
                        if (neighborlist.ContainsKey(currentPointId))
                        {
                            //compare with the current neighbor
                            int currentneighborId = neighborlist[currentPointId];
  
                            double currentNeighborValue = IdToX[currentneighborId] - IdToY[currentneighborId];
                            if (currentNeighborValue < LargestXMinusY && IdToY[CandidateNeighborId] >= IdToY[currentPointId])
                            {
                                neighborlist[currentPointId] = CandidateNeighborId;
                            }
                        }
                        else
                        {
                            if (IdToY[CandidateNeighborId] >= IdToY[currentPointId])
                                neighborlist.Add(currentPointId, CandidateNeighborId);
                        }
                    }
                }

                //process current point
                if (mid == from.Length) --mid; 
                if (IdToY[currentPointId] >= IdToY[PosToId[mid]] && (IdToX[currentPointId] - IdToY[currentPointId] >= LargestXMinusY))
                {
                    LargestXMinusY = IdToX[currentPointId] - IdToY[currentPointId];
                    CandidateNeighborId = currentPointId;
                }

            }

            for (int k = lo; k < hi; k++)
                Assign(k, k, to, from, toPx, toPy, toPid, Px, Py, Pid);

        }


        public static void Assign(int k, int j, double[] from, double[] to, double[] Px, double[] Py, int[] Pid, double[] toPx, double[] toPy, int[] toPid)
        {
            to[k] = from[j]; toPid[k] = Pid[j]; toPx[k] = Px[j]; toPy[k] = Py[j];
        }
        public static void iterativeMergesort(double[] a, double[] Px, double[] Py, int[] Pid)
        {
            double[] from = a;
            double[] to = new double[a.Length];
            int[] toPid = new int[a.Length];
            double[] toPx = new double[a.Length];
            double[] toPy = new double[a.Length];
            for (int blockSize = 1; blockSize < a.Length; blockSize *= 2)
            {
                for (int start = 0; start < a.Length; start += 2 * blockSize)
                    merge(from, to, start, start + blockSize, start + 2 * blockSize,   Px,   Py,   Pid, toPx,toPy,toPid);                
            }
            for (int k = 0; k < a.Length; k++)
                    a[k] = from[k];
        }

        private static void merge(double[] from, double[] to, int lo, int mid, int hi, double[] Px, double[] Py, int[] Pid, double[]toPx, double[]toPy, int[] toPid)
        { 
            if (mid > from.Length) mid = from.Length;
            if (hi > from.Length) hi = from.Length;
            int i = lo, j = mid;
            for (int k = lo; k < hi; k++)
            {
                if (i == mid) {Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid);j++;}
                else if (j == hi) {Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid);i++;}
                else if (from[j] < from[i]) {Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid);j++;}
                else if (from[i] == from[j] && Px[i] < Px[j]) { Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid); i++; }
                else if (from[i] == from[j] && Px[i] >= Px[j]) { Assign(k, j, from, to, Px, Py, Pid, toPx, toPy, toPid); j++; }
                else { Assign(k, i, from, to, Px, Py, Pid, toPx, toPy, toPid); i++; }
            }
            for (int k = lo; k < hi; k++)
                Assign(k, k, to, from,  toPx, toPy, toPid, Px, Py, Pid);

        }

        private static void FixMesh(Tiling g)
        {
            Console.WriteLine("Fixing the mesh");

            for (int i = 0; i < g.NumOfnodes; i++)
            {
                if (g.DegList[i] == 0) continue;
                for (int j = g.N; j < g.NumOfnodes; j++)
                {
                    if (i == j) continue;
                    if (g.VList[i].XLoc == g.VList[j].XLoc && g.VList[i].YLoc == g.VList[j].YLoc)
                    {
                        for (int neighborIndex = 0; neighborIndex < g.DegList[j]; neighborIndex++)
                        {
                            if (g.DegList[g.EList[j, neighborIndex].NodeId] == 0) continue;
                            g.AddEdge(i, g.EList[j, neighborIndex].NodeId);
                        }
                        g.DegList[j] = 0;
                        g.VList[j].Invalid = true;
                    }
                }
            }


            for (int nodeIndex1 = 0; nodeIndex1 < g.N; nodeIndex1++)
            {
                foreach (LineSegment ls1 in g.VList[nodeIndex1].SegmentList.Keys)
                {
                    for (int nodeIndex2 = nodeIndex1 + 1; nodeIndex2 < g.N; nodeIndex2++)
                    {
                        foreach (LineSegment ls2 in g.VList[nodeIndex2].SegmentList.Keys)
                        {
                            if (ls1.Start.Equals(ls2.End) && ls2.Start.Equals(ls1.End))
                            {
                                List<Core.Geometry.Point> list = new List<Core.Geometry.Point>();
                                for (int index = 0; index < g.N; index++)
                                {
                                    Core.Geometry.Point p = new Core.Geometry.Point(g.VList[index].XLoc, g.VList[index].YLoc);
                                    if (MsaglUtilities.PointIsOnAxisAlignedSegment(ls1, p) ||
                                        MsaglUtilities.PointIsOnAxisAlignedSegment(ls2, p))
                                        list.Add(p);
                                }
                                if (list.Count > 2)
                                {
                                    list.Sort();
                                    //foreach (var p in list)
                                    //{
                                    //    Console.WriteLine(p);
                                    //}
                                    //Console.WriteLine("fixing");
                                    Core.Geometry.Point[] points = list.ToArray();

                                    for (int i = 0; i < points.Length; i++)
                                        for (int j = i + 1; j < points.Length; j++)
                                            g.RemoveEdge(g.GetNode((int) points[i].X, (int) points[i].Y),
                                                g.GetNode((int) points[j].X, (int) points[j].Y));

                                    for (int i = 0; i < points.Length - 1; i++)
                                    {
                                        g.AddEdge(g.GetNode((int) points[i].X, (int) points[i].Y),
                                            g.GetNode((int) points[i + 1].X, (int) points[i + 1].Y));
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int nodeIndex = 0; nodeIndex < g.NumOfnodes; nodeIndex++)
                g.nodeTree.Add(new Rectangle(new Core.Geometry.Point(g.VList[nodeIndex].XLoc, g.VList[nodeIndex].YLoc)),
                    nodeIndex);


            bool searchNew = true;
            while (searchNew)
            {
                searchNew = false;
                for (int nodeIndex1 = 0; nodeIndex1 < g.NumOfnodes; nodeIndex1++)
                {
                    for (int neighborIndex = 0; neighborIndex < g.DegList[nodeIndex1]; neighborIndex++)
                    {
                        int neighborId = g.EList[nodeIndex1, neighborIndex].NodeId;

                        Core.Geometry.Point a = new Core.Geometry.Point(g.VList[nodeIndex1].XLoc, g.VList[nodeIndex1].YLoc);
                        Core.Geometry.Point b = new Core.Geometry.Point(g.VList[neighborId].XLoc, g.VList[neighborId].YLoc);

                        int[] intersectedVertices = g.nodeTree.GetAllIntersecting(new Rectangle(a, b));

                        //check if there is any other node on this edge
                        for (int nodeIndex2 = 0; nodeIndex2 < intersectedVertices.Length; nodeIndex2++)
                        {
                            int currentVertexId = intersectedVertices[nodeIndex2];
                            if (g.VList[currentVertexId].Invalid) continue;
                            if (currentVertexId == nodeIndex1 || currentVertexId == neighborId) continue;

                            Core.Geometry.Point p = new Point(g.VList[currentVertexId].XLoc, g.VList[currentVertexId].YLoc);
                            LineSegment ls = new LineSegment(a, b);

                            if (MsaglUtilities.PointIsOnSegment(ls, p))
                            {
                                g.RemoveEdge(nodeIndex1, neighborId);
                                g.AddEdge(nodeIndex1, currentVertexId);
                                g.AddEdge(currentVertexId, neighborId);
                                searchNew = true;
                                break;
                            }
                        }
                        if (searchNew) break;
                    }
                    if (searchNew) break;
                }
            }
        }


        public static void CreateCompetitionMeshWithLeftPriority(Tiling g, Dictionary<int, Node> idToNode, int maxX, int maxY)
        {

            //for each node, create four line segments
            CreateFourRaysPerVertex(g, maxX, maxY);
            Console.WriteLine("Creating Mesh of size " + Math.Max(maxX, maxY));

            Dictionary<LineSegment, int> removeList = new Dictionary<LineSegment, int>();
            Dictionary<LineSegment, int> addList = new Dictionary<LineSegment, int>();

            for (int iteration = 0; iteration <= Math.Max(maxX, maxY); iteration++)
            {
                //for each line segment check whether it hits any other segment or point
                //if so then create a new junction at that point
                for (int nodeIndex1 = 0; nodeIndex1 < g.N; nodeIndex1++)
                {
                    foreach (LineSegment ls1 in g.VList[nodeIndex1].SegmentList.Keys)
                    {
                        if (g.VList[nodeIndex1].SegmentList[ls1] == false) continue;

                        for (int nodeIndex2 = 0; nodeIndex2 < g.N; nodeIndex2++)
                        {
                            if (nodeIndex1 == nodeIndex2) continue;
                            foreach (LineSegment ls2 in g.VList[nodeIndex2].SegmentList.Keys)
                            {


                                if (MsaglUtilities.PointIsOnAxisAlignedSegment(ls2, ls1.End))
                                {

                                    //if they are parallel then create an edge
                                    if (((int)ls1.Start.X == (int)ls1.End.X && (int)ls2.Start.X == (int)ls2.End.X)
                                        || ((int)ls1.Start.Y == (int)ls1.End.Y && (int)ls2.Start.Y == (int)ls2.End.Y))
                                    {

                                        if ((int)ls1.End.X == (int)ls2.Start.X && (int)ls1.End.Y == (int)ls2.Start.Y) continue;

                                        int a = FindVertexClosestToSegmentEnd(g, ls1);
                                        int b = FindVertexClosestToSegmentEnd(g, ls2);

                                        if (a == b)
                                        {
                                            Console.WriteLine("degenerate parallel collision");
                                            //Point coordinates multiplied by >=3 so that this condition does not arise.
                                        }
                                        if (g.AddEdge(a, b))
                                        {
                                            LineSegment l = new LineSegment(ls1.Start, ls2.Start);
                                            if (!addList.ContainsKey(l)) addList.Add(l, -1);
                                            l = new LineSegment(ls2.Start, ls1.Start);
                                            if (!addList.ContainsKey(l)) addList.Add(l, -1);


                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1)) removeList.Add(ls1, nodeIndex1);
                                            if (!removeList.ContainsKey(ls2)) removeList.Add(ls2, nodeIndex2);
                                        }
                                    }

                                    //create a new node at the intersection point                 
                                    else
                                    {
                                        if (MsaglUtilities.PointIsOnAxisAlignedSegment(ls2, ls1.End) &&
                                            MsaglUtilities.PointIsOnAxisAlignedSegment(ls1, ls2.End) &&
                                            nodeIndex1 > nodeIndex2) continue;

                                        if (g.GetNode((int)ls1.End.X, (int)ls1.End.Y) == -1)
                                        {

                                            //create the edges
                                            int a = FindClosestVertexWhileWalkingToStart(g, ls2, ls1.End);
                                            int b = FindClosestVertexWhileWalkingToEnd(g, ls2, ls1.End);
                                            int c = FindVertexClosestToSegmentEnd(g, ls1);
                                            int d = g.NumOfnodes;

                                            g.VList[d] = new Vertex((int)ls1.End.X, (int)ls1.End.Y) { Id = d };
                                            g.NumOfnodes++;

                                            if (a >= 0) g.AddEdge(a, d);
                                            if (b >= 0) g.AddEdge(b, d);
                                            if (c >= 0) g.AddEdge(c, d);
                                            if (a == b)
                                                Console.WriteLine("degenerate");
                                            if (a >= 0 && b >= 0) g.RemoveEdge(a, b);

                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1) && MsaglUtilities.HittedSegmentComesFromLeft(ls2, ls1))
                                            {
                                                removeList.Add(ls1, nodeIndex1);
                                                if (!addList.ContainsKey(ls1)) addList.Add(ls1, -1);
                                            }

                                        }
                                        else
                                        {
                                            int a = g.GetNode((int)ls1.End.X, (int)ls1.End.Y);
                                            int b = FindVertexClosestToSegmentEnd(g, ls1);
                                            if (b == -1)
                                                Console.WriteLine("vertex not found error");
                                            g.AddEdge(a, b);


                                            //if ((idToNode[nodeIndex1].ToString().Equals("System V.2") && idToNode[nodeIndex2].ToString().Equals("TS 4.0")) ||
                                            //    (idToNode[nodeIndex2].ToString().Equals("System V.2") && idToNode[nodeIndex1].ToString().Equals("TS 4.0"))
                                            //    )
                                            //    Console.WriteLine();

                                            if (!removeList.ContainsKey(ls1) && MsaglUtilities.HittedSegmentComesFromLeft(ls2, ls1))
                                            {
                                                removeList.Add(ls1, nodeIndex1);
                                                if (!addList.ContainsKey(ls1)) addList.Add(ls1, -1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var s in removeList)
                {
                    g.VList[s.Value].SegmentList.Remove(s.Key);
                }
                removeList.Clear();
                foreach (var s in addList)
                {
                    if (s.Value >= 0) g.VList[s.Value].SegmentList.Add(s.Key, true);
                    else g.VList[g.GetNode((int)s.Key.Start.X, (int)s.Key.Start.Y)].SegmentList.Add(s.Key, false);
                }
                addList.Clear();


                int nodeIndex;
                for (nodeIndex = 0; nodeIndex < g.N; nodeIndex++)
                {
                    foreach (LineSegment ls in g.VList[nodeIndex].SegmentList.Keys)
                    {

                        if (g.VList[nodeIndex].SegmentList[ls] == false) continue;
                        var nextWeightedPoint = GrowOneUnit(ls, maxX + 1, maxY + 1);
                        if (nextWeightedPoint.X >= 0)
                        {
                            LineSegment l_new = new LineSegment(ls.Start, nextWeightedPoint);


                            if (!addList.ContainsKey(l_new)) addList.Add(l_new, nodeIndex);
                            if (!removeList.ContainsKey(ls)) removeList.Add(ls, nodeIndex);
                        }

                    }
                }

                foreach (var s in removeList)
                {
                    g.VList[s.Value].SegmentList.Remove(s.Key);
                }
                removeList.Clear();
                foreach (var s in addList)
                {
                    if (s.Value >= 0) g.VList[s.Value].SegmentList.Add(s.Key, true);
                    else g.VList[g.GetNode((int)s.Key.Start.X, (int)s.Key.Start.Y)].SegmentList.Add(s.Key, false);
                }
                addList.Clear();

            }
            FixMesh(g);



            Console.WriteLine("Mesh Created");
        }


    }
}
