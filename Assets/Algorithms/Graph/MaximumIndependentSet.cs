using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityMCP.Algorithms.Graph
{
    public static class MaximumIndependentSet
    {
        public static List<int> FindExact(List<int>[] graph)
        {
            var candidates = new HashSet<int>(Enumerable.Range(0, graph.Length));
            var best = new List<int>();
            var current = new List<int>();
            search(graph, candidates, current, best);
            best.Sort();
            return best;
        }

        public static List<int> FindGreedy(List<int>[] graph)
        {
            var remaining = new HashSet<int>(Enumerable.Range(0, graph.Length));
            var result = new List<int>();
            while (remaining.Count > 0)
            {
                int v = remaining.OrderBy(u => graph[u].Count).First();
                result.Add(v);
                remaining.Remove(v);
                foreach (var w in graph[v]) remaining.Remove(w);
            }
            result.Sort();
            return result;
        }

        public static List<int>[] BuildAdjacencyList(int n, IEnumerable<(int, int)> edges)
        {
            var g = new List<int>[n];
            for (int i = 0; i < n; i++) g[i] = new List<int>();
            foreach (var e in edges)
            {
                var a = e.Item1;
                var b = e.Item2;
                if (a == b) continue;
                g[a].Add(b);
                g[b].Add(a);
            }
            for (int i = 0; i < n; i++) g[i] = g[i].Distinct().ToList();
            return g;
        }

        public static bool IsIndependentSet(List<int>[] graph, IEnumerable<int> set)
        {
            var s = new HashSet<int>(set);
            foreach (var v in s)
            {
                foreach (var u in graph[v])
                {
                    if (s.Contains(u)) return false;
                }
            }
            return true;
        }

        static void search(List<int>[] graph, HashSet<int> candidates, List<int> current, List<int> best)
        {
            if (current.Count + candidates.Count <= best.Count) return;
            if (candidates.Count == 0)
            {
                if (current.Count > best.Count)
                {
                    best.Clear();
                    best.AddRange(current);
                }
                return;
            }
            int v = chooseVertex(graph, candidates);
            current.Add(v);
            var nextCandInc = new HashSet<int>(candidates);
            nextCandInc.Remove(v);
            foreach (var w in graph[v]) nextCandInc.Remove(w);
            search(graph, nextCandInc, current, best);
            current.RemoveAt(current.Count - 1);
            var nextCandExc = new HashSet<int>(candidates);
            nextCandExc.Remove(v);
            search(graph, nextCandExc, current, best);
        }

        static int chooseVertex(List<int>[] graph, HashSet<int> candidates)
        {
            int bestV = -1;
            int bestDeg = -1;
            foreach (var v in candidates)
            {
                int d = 0;
                foreach (var u in graph[v])
                {
                    if (candidates.Contains(u)) d++;
                }
                if (d > bestDeg)
                {
                    bestDeg = d;
                    bestV = v;
                }
            }
            return bestV;
        }
    }

    public class MaximumIndependentSetDemo : MonoBehaviour
    {
        [ContextMenu("Run MIS Demo")]
        public void RunMISDemo()
        {
            int n = 6;
            var edges = new List<(int, int)>
            {
                (0,1),(0,2),(1,3),(2,3),(3,4),(4,5)
            };
            var g = MaximumIndependentSet.BuildAdjacencyList(n, edges);
            var exact = MaximumIndependentSet.FindExact(g);
            var greedy = MaximumIndependentSet.FindGreedy(g);
            Debug.Log($"Exact: {string.Join(",", exact)} size={exact.Count}");
            Debug.Log($"Greedy: {string.Join(",", greedy)} size={greedy.Count}");
        }
    }
}
