// See https://aka.ms/new-console-template for more information

using AoC24;
using AoC24.Days;
using System.Diagnostics;

var day = new Day10
{
    SessionId = Environment.GetEnvironmentVariable("aoc-session-id") ?? string.Empty
};

if (day.SessionId == string.Empty)
    Console.WriteLine("Warning: env var 'aoc-session-id' not set");

var part = "Unknown";
try
{
    part = "Setup";
    await day.Setup();
    part = "Part 1";
    Console.WriteLine("Part 1: " + day.Solve1());
    part = "Part 2";
    Console.WriteLine("Part 2: " + day.Solve2());
}
catch (NotImplementedException)
{
    Console.WriteLine($"{part} not implemented");
}