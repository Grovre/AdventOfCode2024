using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;
public abstract class Day<TPart1, TPart2> where TPart1 : unmanaged where TPart2 : unmanaged
{
    public string SessionId { get; set; } = Environment.GetEnvironmentVariable("aoc-session-id") ?? string.Empty;

    [GlobalSetup]
    public async Task Setup()
    {
        await GetInput();
        ParseInput();
    }

    private string CheckAnswer(bool? correct) =>
        correct switch
        {
            true => "Correct",
            false => "Incorrect",
            _ => "Already solved, don't know without manually adding entry (see user temp files: $TEMP$)"
        };

    public async Task<string> CheckAnswer1(TPart1 ans, int year, int day, int part) =>
        CheckAnswer(await AdventOfCodeInput.Answer.For(ans, year, day, part, SessionId));

    public async Task<string> CheckAnswer2(TPart2 ans, int year, int day, int part) =>
        CheckAnswer(await AdventOfCodeInput.Answer.For(ans, year, day, part, SessionId));

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