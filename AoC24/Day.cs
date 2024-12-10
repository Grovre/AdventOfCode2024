using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;
public abstract class Day<TPart1, TPart2>
{
    public string SessionId { get; set; } = Environment.GetEnvironmentVariable("aoc-session-id") ?? string.Empty;

    [GlobalSetup]
    public async Task Setup()
    {
        await GetInput();
        ParseInput();
    }

    protected abstract Task GetInput();
    protected abstract void ParseInput();
    public abstract TPart1 Solve1();
    public abstract TPart2 Solve2();

#pragma warning disable S1133 // Deprecated code should be removed
    [Obsolete("Use Setup instead")]
    [Benchmark]
    public void BdnParseInput()
    {
        ParseInput();
    }

    [Obsolete("Use Solve1 instead")]
    [Benchmark]
    public void BdnSolve1()
    {
        Solve1();
    }

    [Obsolete("Use Solve2 instead")]
    [Benchmark]
    public void BdnSolve2()
    {
        Solve2();
    }
#pragma warning restore S1133 // Deprecated code should be removed
}