<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Collections</Namespace>
</Query>

void Main()
{
    // DeepEqualityComparer is an implementation of IEqualityComparer that probes the public properties
    // of a class, comparing the values to the another instance of the same type. If all values,
    // recursively, are equal, then the objects are considered equal.
    
    DeepEqualityComparer comparer = new DeepEqualityComparer();
    
    // All of the properties of the object returned by CreateFoo method *except one* have the same value.
    // The parameter passed to the method determines the value of that one property (which happens to be
    // named Garply).
    
    Foo foo1 = CreateFoo(456);
    Foo foo2 = CreateFoo(789);
    
    // Because there is an unequal property deep in the object hierarchy, the comparer returns false.
    comparer.Equals(foo1, foo2).Dump("comparer.Equals(foo1[finalGarply:456], foo2[finalGarply:789])");
    
    Foo foo3 = CreateFoo(456);
    
    // Verify, for demonstration purposes, that foo1 and foo3 are different instances.
    ReferenceEquals(foo1, foo3).Dump("ReferenceEquals(foo1[finalGarply:456], foo3[finalGarply:456])");
    
    // All of the properties in the object hierarchy are equal, so the comparer returns true.
    comparer.Equals(foo1, foo3).Dump("comparer.Equals(foo1[finalGarply:456], foo2[finalGarply:456])");
    
//    // Uncomment to dump each instance of Foo.
//    foo1.Dump("foo1");
//    foo2.Dump("foo2");
//    foo3.Dump("foo3");
}

public class Foo
{
    public IEnumerable<Bar> Bars { get; set; }
    public Baz Baz { get; set; }
}

public class Bar
{
    public string Corge { get; set; }
}

public class Baz
{
    public Dictionary<string, Qux> Grault { get; set; }
}

public class Qux
{
    public int Garply { get; set; }
    public double Waldo { get; set; }
}

private Foo CreateFoo(int finalGarplyValue)
{
    return new Foo
    {
        Bars = new[]
        {
            new Bar { Corge = "abc" },
            new Bar { Corge = "xyz" }
        },
        Baz = new Baz
        {
            Grault = new Dictionary<string, Qux>
            {
                {
                    "Hakuna",
                    new Qux
                    {
                        Garply = 123,
                        Waldo = Math.PI
                    } 
                },
                {
                    "Matata",
                    new Qux
                    {
                        Garply = finalGarplyValue,
                        Waldo = Math.E
                    } 
                }
            }
        }
    };
}