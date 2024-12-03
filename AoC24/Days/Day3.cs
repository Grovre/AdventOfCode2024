using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC24.Days;

public partial class Day3 : Day<int, int>
{
    private string _line = string.Empty;

    [GlobalSetup]
    public override async Task Setup()
    {
        // Why wasn't
        var lines = await AdventOfCodeInput.For(2024, 3, SessionId);
        // Why wasn't this specified in the instructions or example?
        _line = string.Concat(lines);
    }

    [GeneratedRegex(@"mul\(\d+,\d+\)")]
    private static partial Regex MulRegex();

    public static int HandleMuls(ReadOnlySpan<char> str)
    {
        var sum = 0;

        foreach (var match in MulRegex().EnumerateMatches(str))
        {
            // Gets 'x,y' from 'mul(x,y)'
            var mulArgs = str[
                (match.Index + "mul(".Length)
                ..
                (match.Index + match.Length - ")".Length)];

            var xSpan = mulArgs[..mulArgs.IndexOf(',')];
            if (!int.TryParse(xSpan, out var x))
            {
                throw new InvalidOperationException("Invalid x");
            }

            var ySpan = mulArgs[(mulArgs.IndexOf(',') + 1)..];
            if (!int.TryParse(ySpan, out var y))
            {
                throw new InvalidOperationException("Invalid y");
            }
            sum += x * y;
        }

        return sum;
    }

    [Benchmark]
    public override int Solve1() => HandleMuls(_line);

    [Benchmark]
    public override int Solve2()
    {
        const string doo = "do()";
        const string dont = "don't()";

        var subLine = _line.AsSpan();
        var sum = 0;

        while (true)
        {
            var dontIdx = subLine.IndexOf(dont);
            if (dontIdx == -1)
            {
                sum += HandleMuls(subLine);
                break;
            }

            sum += HandleMuls(subLine[..dontIdx]);
            subLine = subLine[dontIdx..];
            var dooIdx = subLine.IndexOf(doo);
            if (dooIdx == -1)
            {
                break;
            }

            subLine = subLine[dooIdx..];
        }

        return sum;
    }
}
