// See https://aka.ms/new-console-template for more information

using AoC24;
using AoC24.Days;
using System.Diagnostics;

var day = new Day9
{
    SessionId = Environment.GetEnvironmentVariable("aoc-session-id") ?? string.Empty
};

if (day.SessionId == string.Empty)
    Console.WriteLine("Warning: env var 'aoc-session-id' not set");

await day.Setup();
int part = default;
try
{
    part = 1;
    Console.WriteLine("Part 1: " + day.Solve1());
    part = 2;
    Console.WriteLine("Part 2: " + day.Solve2());
}
catch (NotImplementedException)
{
    Console.WriteLine($"Part {part} not implemented");
}