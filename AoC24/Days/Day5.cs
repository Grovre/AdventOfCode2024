using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AoC24.Days;

public class Day5 : Day<int, int>
{
    private int[][] _pages = [];
    private readonly PageOrderingRulesComparer<int> _comparer = new();

    protected override async Task GetInput()
    {
        var lines = await AdventOfCodeInput.For(2024, 5, SessionId);

        // (int A, int B)[]
        var orderingRules = lines
            .TakeWhile(line => line.Contains('|'))
            .Select(line =>
            {
                var parts = line.Split('|');
                var a = int.Parse(parts[0]);
                var b = int.Parse(parts[1]);
                return (a, b);
            })
            .ToArray();

        _comparer.ImportRules(orderingRules);

        _pages = lines
            .SkipWhile(line => line.Contains('|'))
            .Select(line => line
                .Split(',')
                .Select(int.Parse)
                .ToArray())
            .ToArray();
    }

    private static bool InOrder<T, TCmp>(T[] page, TCmp comparer) where TCmp : IComparer<T>
    {
        for (var i = 1; i < page.Length; i++)
        {
            if (comparer.Compare(page[i - 1], page[i]) > 0)
                return false;
        }
        return true;
    }

    public override int Solve1()
    {
        var sum = 0;

        foreach (var page in _pages)
        {
            if (InOrder(page, _comparer))
                sum += page[page.Length / 2];
        }

        return sum;
    }

    public override int Solve2()
    {
        var sum = 0;

        Parallel.ForEach(_pages, page =>
        {
            var hashCode = new HashCode();
            hashCode.AddBytes(MemoryMarshal.AsBytes<int>(page));
            var hash1 = hashCode.ToHashCode();

            // If `page2 = page` then the solution will still work because of hashing.
            // This removal of the side effect is for the sake of benchmarking which adds time :(
            int[] page2 = [.. page];
            Array.Sort(page2, _comparer);

            hashCode = new HashCode();
            hashCode.AddBytes(MemoryMarshal.AsBytes<int>(page2));
            var hash2 = hashCode.ToHashCode();

            if (hash1 != hash2)
                Interlocked.Add(ref sum, page2[page2.Length / 2]);
        });

        return sum;
    }

    private sealed class PageOrderingRulesComparer<T> : IComparer<T> where T : struct
    {
        private FrozenSet<(T, T)> _rules = [];

        public void ImportRules(params IEnumerable<(T A, T B)> rules) =>
            _rules = rules.ToFrozenSet();

        public int Compare(T x, T y)
        {
            if (_rules.Contains((x, y)))
                return -1;
            if (_rules.Contains((y, x)))
                return 1;

            return 0;
        }
    }
}
