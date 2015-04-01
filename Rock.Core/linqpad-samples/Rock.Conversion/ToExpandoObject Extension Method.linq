<Query Kind="Statements">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Conversion</Namespace>
  <Namespace>System.Dynamic</Namespace>
</Query>

// The ToExpandoObject extension method converts an object graph
// to an ExpandoObject. Very useful for anonymous objects.

var foo = new
{
    Bar = "abc",
    Baz = 123,
    Qux = new
    {
        Corge = Math.PI
    }
};

ExpandoObject expando = foo.ToExpandoObject();

foo.Dump("Original object");
expando.Dump("Converted to ExpandoObject");