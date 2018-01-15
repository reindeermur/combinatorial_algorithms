using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication2
{
    public enum Part
    {
        X,
        Y
    }

    public class Node
    {
        public Node(int number, Part part)
        {
            Number = number;
            Part = part;
            Edges = new List<Edge>();
        }

        public Part Part { get; }
        public Node Pair { get; set; }
        public List<Edge> Edges { get; }
        private int Number { get; }

        public override string ToString()
        {
            return Number.ToString();
        }

        public override int GetHashCode()
        {
            return Number;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Node))
                return false;
            return ((Node) obj).Number == Number && Part == ((Node) obj).Part;
        }
    }

    public class Edge
    {
        public Edge(Node nodeX, Node nodeY)
        {
            NodeY = nodeY;
            NodeX = nodeX;
            NodeX.Edges.Add(this);
            NodeY.Edges.Add(this);
        }

        public bool IsUsed { get; set; }
        public Node NodeX { get; }
        public Node NodeY { get; }

        public override string ToString()
        {
            return NodeX + "-->" + NodeY;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var lines = File.ReadAllLines("in.txt")
                .Select(x =>
                    x.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray())
                .ToArray();
            var n = lines[0][0];
            var YNodes = new List<Node>();
            var max = 0;
            lines.Skip(1).ToList().ForEach(x =>
            {
                var m = x.Max(y => y);
                if (m > max) max = m;
            });
            for (var i = 0; i < max + 1; i++)
                YNodes.Add(new Node(i, Part.Y));
            var Xnodes = new List<Node>();
            for (var i = 1; i <= n; i++)
            {
                Xnodes.Add(new Node(i, Part.X));
                for (var j = 0; j < lines[i].Length; j++)
                    new Edge(Xnodes[i - 1], YNodes[lines[i][j]]);
            }

            Pairs(Xnodes, YNodes);
        }

        private static void Pairs(List<Node> xNodes, List<Node> yNodes)
        {
            DFS(yNodes);
            File.WriteAllText("out.txt",
                xNodes.Any(x => x.Pair == null)
                    ? "N"
                    : "Y\r\n" + string.Join(" ", xNodes.Select(x => x.Pair)));
        }


        private static void DFS(List<Node> Y)
        {
            foreach (var node in Y.Where(y => y.Pair == null))
            {
                var path = new List<Node>();
                var stack = new Stack<Node>();
                stack.Push(node);
                var current = stack.Peek();
                while (stack.Count != 0)
                {
                    current = stack.Pop();

                    if (path.Count > 0 && path.Last().Part == current.Part)
                        break;
                    path.Add(current);
                    if (current.Part == Part.X && current.Pair == null)
                    {
                        ReverseEdges(path);
                        break;
                    }

                    foreach (var edge in current.Edges)
                    {
                        if (current.Part == Part.Y)
                        {
                            if (edge.IsUsed) continue;

                            var nod = edge.NodeY.Part == Part.X ? edge.NodeY : edge.NodeX;
                            stack.Push(nod);
                            break;
                        }

                        if (current.Part == Part.X)
                        {
                            if (!edge.IsUsed) continue;
                            var nod = edge.NodeY.Part == Part.X ? edge.NodeX : edge.NodeY;
                            stack.Push(nod);
                        }
                    }
                }
            }
        }

        private static void ReverseEdges(List<Node> path)
        {
            for (var i = 0; i < path.Count - 1; i++)
            {
                var node = path[i];
                var nextNode = path[i + 1];
                var edge = node.Edges.First(x => Equals(x.NodeY, nextNode) || Equals(x.NodeX, nextNode));
                if (Equals(node.Pair, nextNode))
                {
                    edge.IsUsed = false;
                }
                else
                {
                    node.Pair = nextNode;
                    nextNode.Pair = node;
                    edge.IsUsed = true;
                    i++;
                }
            }
        }
    }
}