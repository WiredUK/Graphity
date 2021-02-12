using System.Collections.Generic;

namespace Graphity.Tests.Fixtures
{
    public class ComparisonExpressionsFixture
    {
        public IEnumerable<Parent> Parents { get; }

        public ComparisonExpressionsFixture()
        {
            Parents = new List<Parent>
            {
                new Parent
                {
                    StringProperty = "string1",
                    ByteProperty = 1,
                    ShortProperty = 1,
                    IntProperty = 1,
                    LongProperty = 1,
                    FloatProperty = 1,
                    DecimalProperty = 1,
                    DoubleProperty = 1,
                    BoolProperty = true,
                    Child = new Child { IgnoredValue = 123 }
                },
                new Parent
                {
                    StringProperty = "string2",
                    ByteProperty = 2,
                    ShortProperty = 2,
                    IntProperty = 2,
                    LongProperty = 2,
                    FloatProperty = 2,
                    DecimalProperty = 2,
                    DoubleProperty = 2,
                    BoolProperty = true
                },
                new Parent
                {
                    StringProperty = "string3",
                    ByteProperty = 3,
                    ShortProperty = 3,
                    IntProperty = 3,
                    LongProperty = 3,
                    FloatProperty = 3,
                    DecimalProperty = 3,
                    DoubleProperty = 3,
                    BoolProperty = false
                },
                new Parent
                {
                    StringProperty = "string4",
                    ByteProperty = 4,
                    ShortProperty = 4,
                    IntProperty = 4,
                    LongProperty = 4,
                    FloatProperty = 4,
                    DecimalProperty = 4,
                    DoubleProperty = 4,
                    BoolProperty = false
                },
                new Parent
                {
                    StringProperty = "string5",
                    ByteProperty = 5,
                    ShortProperty = 5,
                    IntProperty = 5,
                    LongProperty = 5,
                    FloatProperty = 5,
                    DecimalProperty = 5,
                    DoubleProperty = 5,
                    BoolProperty = true
                }
            };
        }

        public class Parent
        {
            public string StringProperty { get; set; }
            public byte ByteProperty { get; set; }
            public short ShortProperty { get; set; }
            public int IntProperty { get; set; }
            public long LongProperty { get; set; }
            public decimal DecimalProperty { get; set; }
            public float FloatProperty { get; set; }
            public double DoubleProperty { get; set; }
            public bool BoolProperty { get; set; }

            public Child Child { get; set; }
        }

        public class Child
        {
            public int IgnoredValue { get; set; }
        }
    }
}