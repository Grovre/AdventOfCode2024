using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;
public abstract class Day<TPart1, TPart2>
{
    public string SessionId { get; set; } = Environment.GetEnvironmentVariable("aoc-session-id") ?? "";

    public abstract Task Setup();
    public abstract TPart1 Solve1();
    public abstract TPart2 Solve2();
}