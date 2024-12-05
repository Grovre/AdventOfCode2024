using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day4 : Day<int, int>
{
    private const string Xmas = "XMAS";
    private static readonly (int A, int B)[] AllDirections = [
        (0, 1), (1, 0), (1, 1), (1, -1),
        (0, -1), (-1, 0), (-1, -1), (-1, 1)
        ];
    private string[] _lines = [];

    [GlobalSetup]
    public override async Task Setup()
    {
        _lines = await AdventOfCodeInput.For(2024, 4, SessionId);
    }

    private bool IsWithinBounds(int i, int j)
    {
        return i >= 0 && i < _lines.Length && j >= 0 && j < _lines[i].Length;
    }

    [Benchmark]
    public int Solve1Sequential()
    {
        var sum = 0;

        for (var i = 0; i < _lines.Length; i++)
        {
            for (var j = 0; j < _lines[i].Length; j++)
            {
                if (_lines[i][j] != Xmas[0])
                    continue;

                foreach (var (a, b) in AllDirections)
                {
                    var nextI = i + a;
                    var nextJ = j + b;
                    var found = true;
                    for (var k = 1; k < Xmas.Length; k++)
                    {
                        if (IsWithinBounds(nextI, nextJ) && _lines[nextI][nextJ] == Xmas[k])
                        {
                            nextI += a;
                            nextJ += b;
                        }
                        else
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        sum++;
                }
            }
        }

        return sum;
    }

    [Benchmark]
    public override int Solve1()
    {
        var sum = 0;

        Parallel.For(0, _lines.Length, i =>
        {
            var localSum = 0;

            for (var j = 0; j < _lines[i].Length; j++)
            {
                if (_lines[i][j] != Xmas[0])
                    continue;

                foreach (var (a, b) in AllDirections)
                {
                    var nextI = i + a;
                    var nextJ = j + b;
                    var found = true;
                    for (var k = 1; k < Xmas.Length; k++)
                    {
                        if (IsWithinBounds(nextI, nextJ) && _lines[nextI][nextJ] == Xmas[k])
                        {
                            nextI += a;
                            nextJ += b;
                        }
                        else
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        localSum++;
                }
            }

            Interlocked.Add(ref sum, localSum);
        });

        Debug.Assert(sum == Solve1Sequential());

        return sum;
    }

    [Benchmark]
    public override int Solve2()
    {
        var sum = 0;

        for (var i = 1; i < _lines.Length - 1; i++)
        {
            for (var j = 1; j < _lines[i].Length - 1; j++)
            {
                if (_lines[i][j] != 'A')
                    continue;

                // Unsafe if any other chars existed lol
                const int ms = 'M' + 'S';

                if (_lines[i - 1][j - 1] + _lines[i + 1][j + 1] == ms &&
                    _lines[i + 1][j - 1] + _lines[i - 1][j + 1] == ms)
                    sum++;
            }
        }

        return sum;
    }
}
