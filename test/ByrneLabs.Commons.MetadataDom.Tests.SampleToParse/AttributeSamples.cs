using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public enum ByteEnum : byte
    {
        A,
        B,
        C
    }

    public enum NormalEnum
    {
        D,
        E,
        F
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ClassAttribute : Attribute
    {
        public ClassAttribute(string name)
        {
            Name = name;
        }

        public ClassAttribute(int intValue)
        {
            IntValue = intValue;
        }

        public ClassAttribute(ByteEnum byteEnum)
        {
            ByteEnumValueProperty = byteEnum;
        }

        public ClassAttribute()
        {
        }

        public ByteEnum ByteEnumValueField;

        public ByteEnum ByteEnumValueProperty { get; set; }

        public int IntValue { get; set; }

        public string Name { get; set; }

        public NormalEnum NormalEnumValue { get; set; }
    }

    [Class(1)]
    [Class(ByteEnum.A)]
    [Class(IntValue = 1)]
    [Class(NormalEnumValue = NormalEnum.D)]
    [Class(ByteEnumValueField = ByteEnum.A)]
    [Class(ByteEnumValueProperty = ByteEnum.B)]
    public class SomeClassWithAttributes
    {
    }
}
