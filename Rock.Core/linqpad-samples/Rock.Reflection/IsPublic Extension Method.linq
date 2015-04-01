<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

void Main()
{
    // Surprisingly, .NET does not provide an easy way of determining whether a property is public.
    // The IsPublic extension method solves this problem.

    PropertyInfo[] fooProperties = typeof(Foo).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    
    foreach (PropertyInfo property in fooProperties)
    {
        property.IsPublic().Dump("Foo." + property.Name + " is public");
    }
}

public class Foo
{
    public string Bar { get; set; }
    public string Baz { get; private set; }
    public string Qux { private get; set; }
    internal string Grault { get; set; }
    public string Corge { get { return null; } }
    public string Garply { set {} }
}