using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Rock.Collections;

namespace Rock.Core.UnitTests.Collections
{
    public class DefaultMemberLocatorTests
    {
        [TestCaseSource("GetTestCases")]
        public void AppropriateMembersAreReturned(MemberInfo member, bool shouldMemberBeReturned)
        {
            var defaultMemberLocator = new DefaultMemberLocator();

            var result = defaultMemberLocator.GetFieldsAndProperties(typeof(Foo));

            Assert.That(result.Any(m => m.Name == member.Name), Is.EqualTo(shouldMemberBeReturned));
        }

        private static IEnumerable<TestCaseData> GetTestCases()
        {
            var fieldsAndProperties =
                typeof(Foo)
                    .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => (m is FieldInfo && m.CustomAttributes.All(a => a.AttributeType != typeof(CompilerGeneratedAttribute))) || m is PropertyInfo)
                    .OrderBy(m => m.Name)
                    .Select(m => new { Member = m, Attribute = (MemberTestCaseAttribute)(m.GetCustomAttributes(typeof(MemberTestCaseAttribute)).Single()) });

            return fieldsAndProperties.Select(x => new TestCaseData(x.Member, x.Attribute.ShouldBeLocated).SetName(string.Format("A {0} should {1}be included", x.Attribute.TestCaseName, (x.Attribute.ShouldBeLocated ? "" : "NOT "))));
        }

        public class Foo
        {
            [MemberTestCase(false, "public const field")]public const int A = 1;
            [MemberTestCase(false, "private const field")]private const int B = 1;

            [MemberTestCase(false, "public static readonly field")]public static readonly int C;
            [MemberTestCase(false, "private static readonly field")]private static readonly int D;

            [MemberTestCase(false, "public static field")]public static int E;
            [MemberTestCase(false, "private static field")]private static int F;

            [MemberTestCase(false, "public static property")]public static int G { get; set; }
            [MemberTestCase(false, "private static property")]private static int H { get; set; }

            [MemberTestCase(false, "public static readonly property")]public static int I { get { return 0; } }
            [MemberTestCase(false, "private static readonly property")]private static int J { get { return 0; } }

            [MemberTestCase(false, "public static writeonly property")]public static int K { set {} }
            [MemberTestCase(false, "private static writeonly property")]private static int L { set {} }

            [MemberTestCase(true, "public readonly field")]public readonly int M;
            [MemberTestCase(false, "private readonly field")]private readonly int N;

            [MemberTestCase(true, "public field")]public int O;
            [MemberTestCase(false, "private field")]private int P;

            [MemberTestCase(true, "public property")]public int Q { get; set; }
            [MemberTestCase(false, "private property")]private int R { get; set; }

            [MemberTestCase(true, "public readonly property")]public int S { get { return 0; } }
            [MemberTestCase(false, "private readonly property")]private int T { get { return 0; } }

            [MemberTestCase(false, "public writeonly property")]public int U { set {} }
            [MemberTestCase(false, "private writeonly property")]private int V { set {} }
        }

        public class MemberTestCaseAttribute : Attribute
        {
            private readonly bool _shouldBeLocated;
            private readonly string _testCaseName;

            public MemberTestCaseAttribute(bool shouldBeLocated, string testCaseName)
            {
                _shouldBeLocated = shouldBeLocated;
                _testCaseName = testCaseName;
            }

            public bool ShouldBeLocated
            {
                get { return _shouldBeLocated; }
            }

            public string TestCaseName
            {
                get { return _testCaseName; }
            }
        }
    }
}