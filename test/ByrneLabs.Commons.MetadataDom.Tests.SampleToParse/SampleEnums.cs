using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public enum AutoTypedEnum
    {
        A,
        B,
        C,
        D,
        E
    }

    internal enum byteEnum : byte
    {
        A = byte.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = byte.MaxValue
    }

    [Flags]
    internal enum byteFlagEnum : byte
    {
        A = byte.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = byte.MaxValue
    }

    internal enum sbyteEnum : sbyte
    {
        A = sbyte.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = sbyte.MaxValue
    }

    [Flags]
    internal enum sbyteFlagEnum : sbyte
    {
        A = sbyte.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = sbyte.MaxValue
    }

    internal enum shortEnum : short
    {
        A = short.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = short.MaxValue
    }

    [Flags]
    internal enum shortFlagEnum : short
    {
        A = short.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = short.MaxValue
    }

    internal enum ushortEnum : ushort
    {
        A = ushort.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = ushort.MaxValue
    }

    [Flags]
    internal enum ushortFlagEnum : ushort
    {
        A = ushort.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = ushort.MaxValue
    }

    internal enum intEnum
    {
        A = int.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = int.MaxValue
    }

    [Flags]
    internal enum intFlagEnum
    {
        A = int.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = int.MaxValue
    }

    internal enum uintEnum : uint
    {
        A = uint.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = uint.MaxValue
    }

    [Flags]
    internal enum uintFlagEnum : uint
    {
        A = uint.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = uint.MaxValue
    }

    internal enum longEnum : long
    {
        A = long.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = long.MaxValue
    }

    [Flags]
    internal enum longFlagEnum : long
    {
        A = long.MinValue,
        B = -2,
        C = -1,
        D = 0,
        E = 1,
        F = 2,
        G = long.MaxValue
    }

    internal enum ulongEnum : ulong
    {
        A = ulong.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = ulong.MaxValue
    }

    [Flags]
    internal enum ulongFlagEnum : ulong
    {
        A = ulong.MinValue,
        D = 0,
        E = 1,
        F = 2,
        G = ulong.MaxValue
    }
}
