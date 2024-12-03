// See https://aka.ms/new-console-template for more information

using AoC24;
using AoC24.Days;

var day = new Day2
{
    SessionId = Environment.GetEnvironmentVariable("aoc-session-id") ?? throw new InvalidOperationException("No session-id")
};

await day.Setup();
Console.WriteLine("Part 1: " + day.Solve1());
Console.WriteLine("Part 2: " + day.Solve2());