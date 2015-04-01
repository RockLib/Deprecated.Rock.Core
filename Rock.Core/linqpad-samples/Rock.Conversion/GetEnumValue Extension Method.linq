<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Conversion</Namespace>
</Query>

void Main()
{
    // The GetEnumValue<TEnum> extension method allows you to easily retrieve the enum
    // value for a given string.

    string fooStringValue = "Baz";

    Foo fooValue = fooStringValue.GetEnumValue<Foo>();
    
    fooValue.Dump();
}

public enum Foo
{
    Bar,
    Baz,
    Qux
}