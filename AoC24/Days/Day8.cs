using BenchmarkDotNet.Attributes;
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
    private string[] _map = [];
    private FrozenDictionary<char, Int2[]> _sources = null!;

    [GlobalSetup]
    public override async Task GetInput()
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