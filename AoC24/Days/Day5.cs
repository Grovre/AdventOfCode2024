using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Collections.Frozen;
using System.Diagnostics;
using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace AoC24.Days;

public class Day5 : Day<int, int>
{
    private int[][] _pages = [];
    private readonly PageOrderingRulesComparer _comparer = new();

    [GlobalSetup]
    public override async Task Setup()
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

    private static bool InOrder(int[] page, PageOrderingRulesComparer comparer)
    {
        for (var i = 1; i < page.Length; i++)
        {
            if (comparer.Compare(page[i - 1], page[i]) > 0)
                return false;
        }
        return true;
    }

    [Benchmark]
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

    [Benchmark]
    public int Solve2_0()
    {
        var sum = 0;

        foreach (var page in _pages)
        {
            var hash1 = 0;
            foreach (var n in page)
                hash1 = HashCode.Combine(hash1, n);

            var page2 = page.ToArray();
            Array.Sort(page2, _comparer);

            var hash2 = 0;
            foreach (var n in page2)
                hash2 = HashCode.Combine(hash2, n);

            if (hash1 != hash2)
                sum += page2[page2.Length / 2];
        }

        return sum;
    }

    [Benchmark]
    public override int Solve2()
    {
        var sum = 0;

        Parallel.ForEach(_pages, page =>
        {
            var hash = _XxHash32<int>(page);

            var page2 = page.ToArray();
            Array.Sort(page2, _comparer);

            var hash2 = _XxHash32<int>(page2);

            if (hash != hash2)
                Interlocked.Add(ref sum, page2[page2.Length / 2]);
        });

        Debug.Assert(sum == Solve2_0());

        return sum;
    }

    private static int _XxHash32<T>(ReadOnlySpan<T> src) where T : unmanaged
    {
        var srcBytes = MemoryMarshal.AsBytes(src);
        int hash, wrote;
        unsafe
        {
            int* pHash = &hash;
            var dstBytes = new Span<byte>(pHash, sizeof(int));
            wrote = XxHash32.Hash(srcBytes, dstBytes);
        }

        Debug.Assert(wrote == sizeof(int));

        return hash;
    }

    private sealed class PageOrderingRulesComparer : IComparer<int>
    {
        private FrozenSet<(int, int)> _rules = [];

        public void ImportRules(params IEnumerable<(int A, int B)> rules) =>
            _rules = rules.ToFrozenSet();

        public int Compare(int a, int b)
        {
            if (_rules.Contains((a, b)))
                return -1;
            if (_rules.Contains((b, a)))
                return 1;

            return 0;
        }
    }
}
