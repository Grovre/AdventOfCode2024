﻿using BenchmarkDotNet.Attributes;
using Iced.Intel;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day8 : Day<int, int>
{
    [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "None needed")]
    private readonly struct Int2(int i, int j)
    {
        public readonly int I = i;
        public readonly int J = j;

        public bool InBounds<T>(T[][] arr) => 
            0 <= I && I < arr.Length && 0 <= J && J < arr[0].Length;
        public bool InBounds<T>(ReadOnlySpan<T[]> span) => 
            0 <= I && I < span.Length && 0 <= J && J < span[0].Length;
        public bool InBounds(string[] str) => 
            0 <= I && I < str.Length && 0 <= J && J < str[0].Length;

        public T ElementIn<T>(T[][] arr) => arr[I][J];
        public T ElementIn<T>(ReadOnlySpan<T[]> span) => span[I][J];
        public char ElementIn(string[] str) => str[I][J];

        public override string ToString() => $"<{I}, {J}>";
        public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);
        public bool Equals(Int2 other) => I == other.I && J == other.J;
        public override int GetHashCode() => HashCode.Combine(I, J);

        public static implicit operator Int2((int I, int J) tuple) => new(tuple.I, tuple.J);
        public static implicit operator (int I, int J)(Int2 int2) => (int2.I, int2.J);

        public static Int2 operator +(Int2 a, Int2 b) => new(a.I + b.I, a.J + b.J);
        public static Int2 operator -(Int2 a, Int2 b) => new(a.I - b.I, a.J - b.J);
        public static Int2 operator *(Int2 a, Int2 b) => new(a.I * b.I, a.J * b.J);
        public static Int2 operator /(Int2 a, Int2 b) => new(a.I / b.I, a.J / b.J);
        public static Int2 operator %(Int2 a, Int2 b) => new(a.I % b.I, a.J % b.J);
        public static Int2 operator +(Int2 a, int b) => new(a.I + b, a.J + b);
        public static Int2 operator -(Int2 a, int b) => new(a.I - b, a.J - b);
        public static Int2 operator *(Int2 a, int b) => new(a.I * b, a.J * b);
        public static Int2 operator /(Int2 a, int b) => new(a.I / b, a.J / b);
        public static Int2 operator %(Int2 a, int b) => new(a.I % b, a.J % b);

        public static Int2 operator <(Int2 a, Int2 b) => new(a.I < b.I ? 1 : 0, a.J < b.J ? 1 : 0);
        public static Int2 operator >(Int2 a, Int2 b) => new(a.I > b.I ? 1 : 0, a.J > b.J ? 1 : 0);
        public static Int2 operator <=(Int2 a, Int2 b) => new(a.I <= b.I ? 1 : 0, a.J <= b.J ? 1 : 0);
        public static Int2 operator >=(Int2 a, Int2 b) => new(a.I >= b.I ? 1 : 0, a.J >= b.J ? 1 : 0);
        public static Int2 operator ==(Int2 a, Int2 b) => new(a.I == b.I ? 1 : 0, a.J == b.J ? 1 : 0);
        public static Int2 operator !=(Int2 a, Int2 b) => new(a.I != b.I ? 1 : 0, a.J != b.J ? 1 : 0);
        public static Int2 operator <(Int2 a, int b) => new(a.I < b ? 1 : 0, a.J < b ? 1 : 0);
        public static Int2 operator >(Int2 a, int b) => new(a.I > b ? 1 : 0, a.J > b ? 1 : 0);
        public static Int2 operator <=(Int2 a, int b) => new(a.I <= b ? 1 : 0, a.J <= b ? 1 : 0);
        public static Int2 operator >=(Int2 a, int b) => new(a.I >= b ? 1 : 0, a.J >= b ? 1 : 0);
        public static Int2 operator ==(Int2 a, int b) => new(a.I == b ? 1 : 0, a.J == b ? 1 : 0);
        public static Int2 operator !=(Int2 a, int b) => new(a.I != b ? 1 : 0, a.J != b ? 1 : 0);
    }

    private string[] _map = [];
    private FrozenDictionary<char, Int2[]> _sources = null!;

    [GlobalSetup]
    public override async Task Setup()
    {
        _map = await AdventOfCodeInput.For(2024, 8, SessionId);
        _sources = _map.SelectMany((line, i) =>
            {
                var srcs = Enumerable.Empty<(char Char, Int2 Int2)>();
                for (var j = 0; j < line.Length; j++)
                {
                    var c = line[j];
                    if (c is '.' or '#')
                        continue;

                    srcs = srcs.Append((c, new Int2(i, j)));
                }

                return srcs;
            })
            .GroupBy(t => t.Char)
            .ToFrozenDictionary(
            t => t.Key, 
            g => g
                .Select(t => t.Int2)
                .ToArray());
    }

    [Benchmark]
    public override int Solve1()
    {
        var antiNodes = new HashSet<Int2>();

        foreach (var srcs in _sources.Values)
        {
            for (var i = 0; i < srcs.Length; i++)
            {
                var src = srcs[i];
                var srcChar = src.ElementIn(_map);
                for (var j = i + 1; j < srcs.Length; j++)
                {
                    var dst = srcs[j];
                    var dstChar = dst.ElementIn(_map);

                    if (srcChar != dstChar)
                        continue;

                    var delta = dst - src;

                    var an1 = dst + delta;
                    if (an1.InBounds(_map))
                        antiNodes.Add(an1);

                    var an2 = src - delta;
                    if (an2.InBounds(_map))
                        antiNodes.Add(an2);
                }
            }
        }


        return antiNodes.Count;
    }

    [Benchmark]
    public override int Solve2()
    {
        var antiNodes = new HashSet<Int2>();

        foreach (var srcs in _sources.Values)
        {
            for (var i = 0; i < srcs.Length; i++)
            {
                var src = srcs[i];
                var srcChar = src.ElementIn(_map);
                var inLineCount = 0;

                for (var j = 0; j < srcs.Length; j++)
                {
                    if (i == j)
                        continue;

                    var dst = srcs[j];
                    var dstChar = dst.ElementIn(_map);

                    if (srcChar != dstChar)
                        continue;

                    var delta = dst - src;

                    var an1 = dst + delta;
                    while (an1.InBounds(_map))
                    {
                        antiNodes.Add(an1);
                        an1 += delta;
                    }

                    var an2 = src - delta;
                    while (an2.InBounds(_map))
                    {
                        antiNodes.Add(an2);
                        an2 -= delta;
                    }

                    inLineCount++;
                }

                if (inLineCount >= 2)
                    antiNodes.Add(src);
            }
        }

        return antiNodes.Count;
    }

}