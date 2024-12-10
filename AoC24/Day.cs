using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;
public abstract class Day<TPart1, TPart2>(int year, int day) where TPart1 : unmanaged where TPart2 : unmanaged
{
    public int PuzzleYear { get; } = year;
    public int PuzzleDay { get; } = day;
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
            _ => "Not reached, on cooldown or already solved.\nIf solved, try manually adding entry (search for 'aoc' in %TEMP%)"
        };

    public async Task<string> CheckAnswer1(TPart1 ans, int part) =>
        CheckAnswer(await AdventOfCodeInput.Answer.For(ans, PuzzleYear, PuzzleDay, part, SessionId));

    public async Task<string> CheckAnswer2(TPart2 ans, int part) =>
        CheckAnswer(await AdventOfCodeInput.Answer.For(ans, PuzzleYear, PuzzleDay, part, SessionId));

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