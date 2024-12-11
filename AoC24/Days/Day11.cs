using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day11() : Day<long, long>(2024, 11)
{
    private string[] _lines = [];
    private List<long> _stones = [];

    protected override async Task GetInput()
    {
        _lines = await AdventOfCodeInput.For(PuzzleYear, PuzzleDay, SessionId);
    }

    protected override void ParseInput()
    {
        _stones = [];
        var line = _lines[0].AsSpan();
        foreach (var stoneRange in line.Split(' '))
        {
            _stones.Add(long.Parse(line[stoneRange]));
        }
    }

    private long DoBlinks(int blinks)
    {
        var stoneMap = _stones.ToDictionary(s => s, _ => 1L);
        var tempStoneMap = new Dictionary<long, long>();

        for (var i = 0; i < blinks; i++)
        {
            foreach (var (stone, count) in stoneMap)
            {
                if (count == 0)
                    continue;

                if (stone == 0)
                {
                    tempStoneMap.TryAdd(1, 0);
                    tempStoneMap[1] += count;
                    continue;
                }

                // If digit count is even. Stone is a long
                var digitCount = (long)Math.Log10(stone) + 1;
                if (digitCount != 0 && digitCount % 2 == 0)
                {
                    var stoneStr = stone.ToString().AsSpan();
                    var mid = stoneStr.Length / 2;
                    var left = long.Parse(stoneStr[..mid]);
                    var right = long.Parse(stoneStr[mid..]);
                    tempStoneMap.TryAdd(left, 0);
                    tempStoneMap[left] += count;
                    tempStoneMap.TryAdd(right, 0);
                    tempStoneMap[right] += count;
                    continue;
                }

                tempStoneMap.TryAdd(stone * 2024, 0);
                tempStoneMap[stone * 2024] += count;
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
