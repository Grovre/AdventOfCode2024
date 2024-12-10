using System.Diagnostics.CodeAnalysis;

namespace AoC24;

[SuppressMessage("Blocker Code Smell", "S2368:Public methods should not have multidimensional array parameters", Justification = "None needed")]
public readonly struct Int2(int i, int j)
{
    public readonly int I = i;
    public readonly int J = j;

    public bool InBounds<T>(T[][] arr) =>
        0 <= I && I < arr.Length && 0 <= J && J < arr[0].Length;
    public bool InBounds<T>(ReadOnlySpan<T[]> span) =>
        0 <= I && I < span.Length && 0 <= J && J < span[0].Length;
    public bool InBounds(string[] str) =>
        0 <= I && I < str.Length && 0 <= J && J < str[0].Length;

    public T ElementIn<T>(T[][] arr) => arr[I][J];
    public T ElementIn<T>(ReadOnlySpan<T[]> span) => span[I][J];
    public char ElementIn(string[] str) => str[I][J];

    public override string ToString() => $"<{I}, {J}>";
    public override bool Equals(object? obj) => base.Equals(obj);
    public bool Equals(Int2 other) => I == other.I && J == other.J;
    public override int GetHashCode() => HashCode.Combine(I, J);

    public static implicit operator Int2((int I, int J) tuple) => new(tuple.I, tuple.J);
    public static implicit operator (int I, int J)(Int2 int2) => (int2.I, int2.J);

    public static Int2 operator +(Int2 a, Int2 b) => new(a.I + b.I, a.J + b.J);
    public static Int2 operator -(Int2 a, Int2 b) => new(a.I - b.I, a.J - b.J);
    public static Int2 operator *(Int2 a, Int2 b) => new(a.I * b.I, a.J * b.J);
    public static Int2 operator /(Int2 a, Int2 b) => new(a.I / b.I, a.J / b.J);
    public static Int2 operator %(Int2 a, Int2 b) => new(a.I % b.I, a.J % b.J);
    public static Int2 operator +(Int2 a, int b) => new(a.I + b, a.J + b);
    public static Int2 operator -(Int2 a, int b) => new(a.I - b, a.J - b);
    public static Int2 operator *(Int2 a, int b) => new(a.I * b, a.J * b);
    public static Int2 operator /(Int2 a, int b) => new(a.I / b, a.J / b);
    public static Int2 operator %(Int2 a, int b) => new(a.I % b, a.J % b);

    public static Int2 operator <(Int2 a, Int2 b) => new(a.I < b.I ? 1 : 0, a.J < b.J ? 1 : 0);
    public static Int2 operator >(Int2 a, Int2 b) => new(a.I > b.I ? 1 : 0, a.J > b.J ? 1 : 0);
    public static Int2 operator <=(Int2 a, Int2 b) => new(a.I <= b.I ? 1 : 0, a.J <= b.J ? 1 : 0);
    public static Int2 operator >=(Int2 a, Int2 b) => new(a.I >= b.I ? 1 : 0, a.J >= b.J ? 1 : 0);
    public static Int2 operator ==(Int2 a, Int2 b) => new(a.I == b.I ? 1 : 0, a.J == b.J ? 1 : 0);
    public static Int2 operator !=(Int2 a, Int2 b) => new(a.I != b.I ? 1 : 0, a.J != b.J ? 1 : 0);
    public static Int2 operator <(Int2 a, int b) => new(a.I < b ? 1 : 0, a.J < b ? 1 : 0);
    public static Int2 operator >(Int2 a, int b) => new(a.I > b ? 1 : 0, a.J > b ? 1 : 0);
    public static Int2 operator <=(Int2 a, int b) => new(a.I <= b ? 1 : 0, a.J <= b ? 1 : 0);
    public static Int2 operator >=(Int2 a, int b) => new(a.I >= b ? 1 : 0, a.J >= b ? 1 : 0);
    public static Int2 operator ==(Int2 a, int b) => new(a.I == b ? 1 : 0, a.J == b ? 1 : 0);
    public static Int2 operator !=(Int2 a, int b) => new(a.I != b ? 1 : 0, a.J != b ? 1 : 0);
}