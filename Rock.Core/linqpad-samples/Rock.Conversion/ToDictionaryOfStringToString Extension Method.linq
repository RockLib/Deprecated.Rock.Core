<Query Kind="Statements">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Conversion</Namespace>
</Query>

// The ToDictionaryOfStringToString extension method converts an object graph
// to a IDictionary<string, string>. Very useful for anonymous objects.

var foo = new
{
    Bar = "abc",
    Baz = 123,
    Qux = Math.PI
};

IDictionary<string, string> dictionary = foo.ToDictionaryOfStringToString();

foo.Dump("Original object");
dictionary.Dump("Converted to IDictionary<string, string>");