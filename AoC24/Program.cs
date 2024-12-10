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
    var ans = day.Solve1();
    Console.WriteLine($"Part 1: {ans}, correct? {await day.CheckAnswer1(ans, 2024, 10, 1)}");
    part = "Part 2";
    ans = day.Solve2();
    Console.WriteLine($"Part 2: {ans}, correct? {await day.CheckAnswer2(ans, 2024, 10, 2)}");
}
catch (NotImplementedException)
{
    Console.WriteLine($"{part} not implemented");
}