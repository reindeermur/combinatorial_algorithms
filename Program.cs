using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication3
{
    internal class Node
    {
        public int Number { get; }
        public Dictionary<Node, int> Prices { get; }
        public int CurrentPrice { get; set; }
        public bool Used { get; set; }

        public Node(int number)
        {
            Number = number;
            Prices = new Dictionary<Node, int>();
            CurrentPrice = 20000;
        }

        public override bool Equals(object obj) => Number.Equals(((Node)obj).Number);

        public override string ToString() => Number.ToString();

        public override int GetHashCode() => Number.GetHashCode();
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var lines = File.ReadAllLines("in.txt");
            var nk = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var N = nk[0];
            var k = nk[1];
            var nodes = new List<Node>();
            for (var i = 1; i <= N; i++)
                nodes.Add(new Node(i));

            for (var i = 1; i <= k; i++)
            {
                var l = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                nodes[l[0] - 1].Prices.Add(nodes[l[1] - 1], l[2]);
                nodes[l[1] - 1].Prices.Add(nodes[l[0] - 1], l[2]);
            }
            var max = new int[N];
            nodes.ForEach(x =>
            {
                Djikstra(x);
                max[x.Number - 1] = nodes.Where(z => z.CurrentPrice != 20000).Max(y => y.CurrentPrice);
                nodes.ForEach(y =>
                {
                    y.Used = false;
                    y.CurrentPrice = 20000;
                });
            });
            var maxNodes = new List<int>();
            var min = int.MaxValue;
            for (var i = 0; i < N; i++)
            {
                if (max[i] > min) continue;
                if (max[i] == min) { maxNodes.Add(i + 1); continue; }
                min = max[i];
                maxNodes = new List<int> { i + 1 };
            }
            File.WriteAllText("out.txt", $"{min} {string.Join(" ", maxNodes)}");

        }

        private static void Djikstra(Node start)
        {
            start.CurrentPrice = 0;
            var queue = new Queue<Node>();
            queue.Enqueue(start);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                foreach (var e in current.Prices.Keys)
                {

                    if (e.CurrentPrice <= current.Prices[e] + current.CurrentPrice) continue;
                    e.CurrentPrice = current.Prices[e] + current.CurrentPrice;
                }
                current.Used = true;
                foreach (var e in current.Prices.Keys)
                {
                    if (e.Used == false)
                        queue.Enqueue(e);
                }
            }
        }
    }
}