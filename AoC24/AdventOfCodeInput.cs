using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24;

public static class AdventOfCodeInput
{
    public static async Task<string[]> For(int year, int day, string sessionId)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"AoC-{year}-{day}.tmp");
        if (File.Exists(tempFilePath))
            return await File.ReadAllLinesAsync(tempFilePath);

        var url = $"https://adventofcode.com/{year}/day/{day}/input";
        var client = new HttpClient();
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("Cookie", $"session={sessionId.Replace("session=", "")}");

        using var resp = await client.SendAsync(req);
        resp.EnsureSuccessStatusCode();
        var input = await resp.Content.ReadAsStringAsync();

        var lines = input.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        await File.WriteAllLinesAsync(tempFilePath, lines);
        return lines;
    }
}
