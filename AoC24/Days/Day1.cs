using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day1() : Day<int, int>(2024, 1)
{
    private string[] _lines = [];
    private int[] _a1 = [];
    private int[] _a2 = [];

    protected override async Task GetInput()
    {
        _lines = await AdventOfCodeInput.For(PuzzleYear, PuzzleDay, SessionId);
    }

    protected override void ParseInput()
    {
        var l1 = new List<int>();
        var l2 = new List<int>();

        foreach (var line in _lines)
        {
            var parts = line.Split("  ");
            l1.Add(int.Parse(parts[0]));
            l2.Add(int.Parse(parts[1]));
        }

        _a1 = l1.ToArray();
        _a2 = l2.ToArray();
        Array.Sort(_a1);
        Array.Sort(_a2);
    }

    [Benchmark]
    public int Solve1Linq()
    {
        return _a1.Zip(_a2, (x, y) => Math.Abs(x - y)).Sum();
    }

    public override int Solve1()
    {
        if (_a1.Length == 0)
            return 0;

        if (_a1.Length != _a2.Length)
            throw new InvalidOperationException(
                "Spans must have the same length");

        ref var a1i0 = ref _a1[0];
        ref var a2i0 = ref _a2[0];
        var vsum = Vector<int>.Zero;
        int i;
        var end = _a1.Length - Vector<int>.Count;

        for (i = 0; i <= end; i += Vector<int>.Count)
        {
            ref var a1i = ref Unsafe.Add(ref a1i0, i);
            ref var a2i = ref Unsafe.Add(ref a2i0, i);
            var va1 = Unsafe.As<int, Vector<int>>(ref a1i);
            var va2 = Unsafe.As<int, Vector<int>>(ref a2i);
            vsum += Vector.Abs(va1 - va2);
        }

        var sum = Vector.Sum(vsum);
        for (; i < _a1.Length; i++)
            sum += Math.Abs(_a1[i] - _a2[i]);

        Debug.Assert(sum == Solve1Linq());

        return sum;
    }

    public override int Solve2()
    {
        return _a1.AsParallel().Sum(x => x * _a2.Count(y => y == x));
    }
}
