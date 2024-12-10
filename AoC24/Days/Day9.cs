using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC24.Days;

public class Day9 : Day<long, int>
{
    private int[] _compressedFileBlocks = [];

    public override async Task GetInput()
    {
        var lines = await AdventOfCodeInput.For(2024, 9, SessionId);
        Debug.Assert(lines.Length == 1);
        Debug.Assert(lines[0].All(char.IsDigit));
        _compressedFileBlocks = lines[0].Select(c => c - '0').ToArray();
        Debug.Assert(Array.TrueForAll(_compressedFileBlocks, x => x >= 0));
    }

    private static int[] Decompress(int[] compressedFileBlocks)
    {
        Debug.Assert(Array.TrueForAll(compressedFileBlocks, x => x >= 0));

        var spaceNeeded = compressedFileBlocks.Sum();
        var decompressed = new int[spaceNeeded];
        var id = 0;
        var decompressionCursor = 0;

        // CHECKED for sanity
        for (var i = 0; i < compressedFileBlocks.Length; i++) checked
        {
            var decompressedFileBlockData = i % 2 == 0 ? id++ : -1;
            for (var j = 0; j < compressedFileBlocks[i]; j++)
                decompressed[decompressionCursor++] = decompressedFileBlockData;
        }

        return decompressed;
    }

    private static void Defragment(int[] fragmented)
    {
        var i = 0;
        var j = fragmented.Length - 1;

        // CHECKED for sanity
        while (i < j) checked
        {
            while (i < fragmented.Length && fragmented[i] != -1)
                i++;

            while (j >= 0 && fragmented[j] == -1)
                j--;

            if (j < 0 || i >= fragmented.Length)
                break;

            fragmented[i] = fragmented[j];
            fragmented[j] = -1;
            i++;
            j--;
        }
    }

    public override long Solve1()
    {
        var decompressed = Decompress(_compressedFileBlocks);
        Defragment(decompressed);

        var checksum = 0L;
        // CHECKED for sanity
        for (var i = 0; i < decompressed.Length && decompressed[i] != -1; i++)
            checked { checksum += i * decompressed[i]; }

        return checksum;
    }

    public override int Solve2()
    {
        throw new NotImplementedException();
    }
}
