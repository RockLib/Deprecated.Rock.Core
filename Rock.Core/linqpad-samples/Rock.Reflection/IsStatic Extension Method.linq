<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

void Main()
{
    // .NET also does not provide an easy way of determining whether a property is static.
    // The IsStatic extension method solves this problem.

    PropertyInfo[] fooProperties = typeof(Foo).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
    
    foreach (PropertyInfo property in fooProperties)
    {
        // Is this property static?
        property.IsStatic().Dump("Foo." + property.Name + " is static");
    }
}

public class Foo
{
    public string Bar { get; set; }
    public static string Baz { get; set; }
    internal string Qux { private get; set; }
    internal static string Grault { get; set; }
}