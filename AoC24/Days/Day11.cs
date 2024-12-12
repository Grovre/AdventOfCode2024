using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day11() : Day<long, long>(2024, 11)
{
    private string[] _lines = [];
    private long[] _stones = [];

    protected override async Task GetInput()
    {
        _lines = await AdventOfCodeInput.For(PuzzleYear, PuzzleDay, SessionId);
    }

    protected override void ParseInput()
    {
        var line = _lines[0].AsSpan();
        _stones = new long[line.Count(' ') + 1];

        var i = 0;
        foreach (var stoneRange in line.Split(' '))
        {
            _stones[i++] = long.Parse(line[stoneRange]);
        }
    }

    private long DoBlinks(int blinks)
    {
        var stoneMap = new Dictionary<long, long>();
        var tempStoneMap = new Dictionary<long, long>();

        // Prevents bug if there's multiple of the same stone in initial input
        foreach (var stone in _stones)
        {
            stoneMap.TryAdd(stone, 0);
            stoneMap[stone]++;
        }

        for (var i = 0; i < blinks; i++)
        {
            foreach (var (stone, count) in stoneMap)
            {
                Debug.Assert(count > 0);

                if (stone == 0)
                {
                    ref var entry1 = ref CollectionsMarshal.GetValueRefOrAddDefault(tempStoneMap, 1, out _);
                    entry1 += count;
                    continue;
                }

                var digitCount = (int)Math.Log10(stone) + 1;
                if (digitCount >= 2 && digitCount % 2 == 0)
                {
                    var stoneStr = stone.ToString().AsSpan();
                    var mid = stoneStr.Length / 2;
                    var left = long.Parse(stoneStr[..mid]);
                    var right = long.Parse(stoneStr[mid..]);
                    ref var leftEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(tempStoneMap, left, out _);
                    leftEntry += count;
                    ref var rightEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(tempStoneMap, right, out _);
                    rightEntry += count;
                    continue;
                }

                ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(tempStoneMap, stone * 2024, out _);
                entry += count;
            }

            (tempStoneMap, stoneMap) = (stoneMap, tempStoneMap);
            tempStoneMap.Clear();
        }

        return stoneMap.Values.Sum();
    }

    public override long Solve1()
    {
        return DoBlinks(25);
    }

    public override long Solve2()
    {
        return DoBlinks(75);
    }
}
