using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day7 : Day<long, long>
{
    private (long Expected, long[] Numbers)[] _testValues = Array.Empty<(long, long[])>();
    private static Func<long, long, long> Add = (a, b) => a + b;
    private static Func<long, long, long> Mul = (a, b) => a * b;
    private static Func<long, long, long> Concat = (a, b) =>
    {
        //return long.Parse(a.ToString() + b.ToString());
        // concat b to a
        var t = b;
        while (b > 0)
        {
            a *= 10;
            b /= 10;
        }
        return a + t;
    };

    [GlobalSetup]
    public override async Task GetInput()
    {
        _testValues = (await AdventOfCodeInput.For(2024, 7, SessionId))
            .Select(line =>
            {
                var split = line.Split(':');
                var tv = long.Parse(split[0]);

                var numbers = split[1]
                    .Trim()
                    .Split(' ')
                    .Select(long.Parse)
                    .ToArray();

                return (tv, numbers);
            })
            .ToArray();
    }

    private static bool CanMeetExpected(long actual, long expected, ReadOnlySpan<long> numbers, params ReadOnlySpan<Func<long, long, long>> ops)
    {
        // 2nd part of this was not stated in instructions...
        if (actual == expected && numbers.Length == 0)
            return true;

        if (actual > expected || numbers.Length == 0)
            return false;

        foreach (var op in ops)
        {
            if (CanMeetExpected(op(actual, numbers[0]), expected, numbers[1..], ops))
                return true;
        }

        return false;
    }

    [Benchmark]
    public override long Solve1()
    {
        var sum = 0L;

        Parallel.ForEach(_testValues, tv =>
        {
            ReadOnlySpan<long> span = tv.Numbers;
            if (CanMeetExpected(span[0], tv.Expected, span[1..], Add, Mul))
                Interlocked.Add(ref sum, tv.Expected);
        });

        return sum;
    }

    [Benchmark]
    public override long Solve2()
    {
        var sum = 0L;

        Parallel.ForEach(_testValues, tv =>
        {
            ReadOnlySpan<long> span = tv.Numbers;
            if (CanMeetExpected(span[0], tv.Expected, span[1..], Add, Mul, Concat))
                Interlocked.Add(ref sum, tv.Expected);
        });

        return sum;
    }
}
