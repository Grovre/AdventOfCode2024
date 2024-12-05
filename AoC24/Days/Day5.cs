using BenchmarkDotNet.Attributes;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    public override int Solve2()
    {
        var sum = 0;

        foreach (var page in _pages)
        {
            var hashCodeCombiner = HashCodeCombiner.Start();
            foreach (var n in page)
                hashCodeCombiner.Add(n);
            var hashCode = hashCodeCombiner.CombinedHash;

            Array.Sort(page, _comparer);

            hashCodeCombiner = HashCodeCombiner.Start();
            foreach (var n in page)
                hashCodeCombiner.Add(n);

            if (hashCode != hashCodeCombiner.CombinedHash)
                sum += page[page.Length / 2];
        }

        return sum;
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
