using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day10 : Day<int, int>
{
    private int[][] _heightMap = [];
    private Int2[] _startingPositions = [];

    [GlobalSetup]
    public override async Task Setup()
    {
        _heightMap = (await AdventOfCodeInput.For(2024, 10, SessionId))
            .Select(line =>
            {
                var row = new int[line.Length];
                for (var i = 0; i < line.Length; i++)
                    row[i] = line[i] - '0';
                return row;
            })
            .ToArray();

        _startingPositions = _heightMap
            .Index()
            .SelectMany(t =>
            {
                var positions = new List<Int2>();
                for (var j = 0; j < t.Item.Length; j++)
                {
                    if (t.Item[j] == 0)
                        positions.Add((t.Index, j));
                }

                return positions;
            })
            .ToArray();
    }

    private int CalculateTrailheadScore(Int2 position, int endHeight, HashSet<Int2>? endPositions)
    {
        Debug.Assert(position.InBounds(_heightMap));

        var currentHeight = position.ElementIn(_heightMap);

        if (currentHeight > endHeight)
            throw new InvalidOperationException("End height is lower than current height");

        if (currentHeight == endHeight)
        {
            endPositions?.Add(position);
            return 1;
        }

        // use for loops to calculate all position deltas, no diagonals
        ReadOnlySpan<Int2> deltas = [(1, 0), (0, 1), (-1, 0), (0, -1)];
        var sum = 0;
        
        foreach (var delta in deltas)
        {
            var nextPosition = position + delta;

            if (!nextPosition.InBounds(_heightMap) ||
                nextPosition.ElementIn(_heightMap) != currentHeight + 1)
                continue;

            sum += CalculateTrailheadScore(nextPosition, endHeight, endPositions);
        }

        return sum;
    }

    [Benchmark]
    public override int Solve1()
    {
        var sum = 0;

        Parallel.ForEach(_startingPositions, start =>
        {
            var endPositions = new HashSet<Int2>();
            CalculateTrailheadScore(start, 9, endPositions);
            Interlocked.Add(ref sum, endPositions.Count);
        });

        return sum;
    }

    [Benchmark]
    public override int Solve2()
    {
        var sum = 0;

        Parallel.ForEach(_startingPositions, start =>
        {
            var paths = CalculateTrailheadScore(start, 9, null);
            Interlocked.Add(ref sum, paths);
        });

        return sum;
    }
}
