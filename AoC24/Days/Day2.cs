using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;
public class Day2() : Day<int, int>(2024, 2)
{
    private string[] _lines = [];
    private int[][] _reports = [];
    private int[] _report = [];

    protected override async Task GetInput()
    {
        _lines = await AdventOfCodeInput.For(PuzzleYear, PuzzleDay, SessionId);

    }

    protected override void ParseInput()
    {
        _reports = _lines
            .Select(line => line
                .Split(' ')
                .Select(int.Parse)
                .ToArray())
            .ToArray();

        _report = _reports[0];
    }

    public override int Solve1()
    {
        var safe = 0;
        foreach (var report in _reports)
        {
            _report = report;
            if (IsSafe())
                safe++;
        }

        return safe;
    }

    public override int Solve2()
    {
        var safe = 0;
        foreach (var report in _reports)
        {
            _report = report;
            if (IsSafe())
            {
                safe++;
                continue;
            }

            var toleratedLevels = new int[report.Length - 1];
            for (var i = 0; i < report.Length; i++)
            {
                Array.Copy(report, 0, toleratedLevels, 0, i);
                Array.Copy(report, i + 1, toleratedLevels, i, report.Length - i - 1);
                _report = toleratedLevels;
                if (IsSafe())
                {
                    safe++;
                    break;
                }
            }
        }

        return safe;
    }

    [Benchmark]
    public bool IsSafeOriginal()
    {
        var differs = new int[_report.Length - 1];
        for (var i = 0; i < _report.Length - 1; i++)
            differs[i] = _report[i + 1] - _report[i];

        var isMonotonic = Array.TrueForAll(differs, x => x > 0) || Array.TrueForAll(differs, x => x < 0);
        var isInRange = Array.TrueForAll(differs, x => Math.Abs(x) is >= 1 and <= 3);

        return isMonotonic && isInRange;
    }

    [Benchmark]
    public bool IsSafe()
    {
        var diffSign0 = int.Sign(_report[1] - _report[0]);
        var actual = true;
        for (var i = 1; i < _report.Length; i++)
        {
            var diff = _report[i] - _report[i - 1];
            if (int.Abs(diff) is < 1 or > 3)
            {
                actual = false;
                break;
            }
            if (int.Sign(diff) != diffSign0)
            {
                actual = false;
                break;
            }
        }

        Debug.Assert(IsSafeOriginal() == actual);
        return actual;
    }

    [Benchmark]
    public bool IsSafeVectorized()
    {
        var actual = true;
        int i;
        var vsign0 = Vector.CopySign(Vector<int>.One, new Vector<int>(_report[1] - _report[0]));
        ref var n0 = ref _report[0];
        for (i = 0; i <= _report.Length - Vector<int>.Count - 1; i += Vector<int>.Count)
        {
            var vx = Unsafe.As<int, Vector<int>>(ref Unsafe.Add(ref n0, i));
            var vy = Unsafe.As<int, Vector<int>>(ref Unsafe.Add(ref n0, i + 1));
            var vdiff = vy - vx;

            var vabsDiff = Vector.Abs(vdiff);
            var vabsDiffLt1 = Vector.LessThan(vabsDiff, Vector<int>.One);
            var vabsDiffGt3 = Vector.GreaterThan(vabsDiff, new Vector<int>(3));
            var vabsDiffOr = Vector.BitwiseOr(vabsDiffLt1, vabsDiffGt3);
            if (Vector.LessThanAny(vabsDiffOr, Vector<int>.Zero))
            {
                actual = false;
                break;
            }
            if (Vector.CopySign(Vector<int>.One, vdiff) != vsign0)
            {
                actual = false;
                break;
            }
        }

        if (actual)
            for (; i < _report.Length - 1; i++)
            {
                var diff = _report[i + 1] - _report[i];
                if (Math.Abs(diff) is < 1 or > 3)
                {
                    actual = false;
                    break;
                }
                if (Math.Sign(diff) != Math.Sign(_report[1] - _report[0]))
                {
                    actual = false;
                    break;
                }
            }

        Debug.Assert(IsSafe() == actual);

        return actual;
    }
}
