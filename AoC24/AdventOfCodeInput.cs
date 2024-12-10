using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;

public static class AdventOfCodeInput
{
    private static readonly HttpClient _client = new();
    public static async Task<string[]> For(int year, int day, string sessionId)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"AoC-{year}-{day}.in");
        if (File.Exists(tempFilePath))
            return await File.ReadAllLinesAsync(tempFilePath);

        var url = $"https://adventofcode.com/{year}/day/{day}/input";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("Cookie", $"session={sessionId.Replace("session=", "")}");

        using var resp = await _client.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var input = await resp.Content.ReadAsStringAsync();

        var lines = input.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        await File.WriteAllLinesAsync(tempFilePath, lines);
        return lines;
    }

    public static class Answer
    {
        /*
         * File looks like:
         * 1:123
         * 2:456
         */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3218:Inner class members should not shadow outer class \"static\" or type members", Justification = "None needed")]
        public static async Task<bool?> For<T>(T ans, int year, int day, int part, string sessionId) where T : unmanaged
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"AoC-{year}-{day}.ans");
            if (File.Exists(tempFilePath))
            {
                var lines = await File.ReadAllLinesAsync(tempFilePath);
                var partAnsMap = lines
                    .Select(l => l.Split(':'))
                    .ToDictionary(l => int.Parse(l[0]), l => l[1]);

                if (partAnsMap.TryGetValue(part, out var partAns))
                {
                    return ans.ToString() == partAns;
                }
            }

            var ansStr = ans.ToString();
            Trace.Assert(ansStr != null);

            var post = new HttpRequestMessage(HttpMethod.Post, $"https://adventofcode.com/{year}/day/{day}/answer")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["level"] = part.ToString(),
                    ["answer"] = ansStr
                }),
                Headers = {
                    { "Cookie", $"session={sessionId.Replace("session=", "")}" }
                }
            };

            var resp = await _client.SendAsync(post);

            var html = await resp.Content.ReadAsStringAsync();
            if (html.Contains("That's the right answer"))
            {
                using var fs = File.OpenWrite(tempFilePath);
                using var writer = new StreamWriter(fs);
                await writer.WriteLineAsync($"{part}:{ansStr}");
                return true;
            }
            else if (html.Contains("That's not the right answer"))
            {
                using var fs = File.OpenWrite(tempFilePath);
                using var writer = new StreamWriter(fs);
                await writer.WriteLineAsync($"{part}:{ansStr}");
                return false;
            }

            return null;
        }
    }
}
