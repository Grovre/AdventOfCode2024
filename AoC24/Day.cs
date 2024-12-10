using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;
public abstract class Day<TPart1, TPart2>
{
    public string SessionId { get; set; } = Environment.GetEnvironmentVariable("aoc-session-id") ?? string.Empty;
    public bool IsReadyToSolve { get; protected set; } = false;

    [GlobalSetup]
    public async Task Setup()
    {
        await GetInput();
        ParseInput();
        IsReadyToSolve = true;
    }

    protected abstract Task GetInput();
    protected virtual void ParseInput() { throw new NotImplementedException(); }
    public abstract TPart1 Solve1();
    public abstract TPart2 Solve2();

    [Benchmark]
    public void BdnParseInput()
    {
        ParseInput();
    }

    [Benchmark]
    public void BdnSolve1()
    {
        Solve1();
    }

    [Benchmark]
    public void BdnSolve2()
    {
        Solve2();
    }
}