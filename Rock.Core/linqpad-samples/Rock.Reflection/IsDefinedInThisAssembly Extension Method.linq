<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

void Main()
{
    // The IsDefinedInThisAssembly extension method operates on a MemberInfo*
    // object. It returns a value of true if the MemberInfo is defined in
    // the assembly where the call to IsDefinedInThisAssembly is being made.
    // 
    // *Inheritors of MemberInfo include: Type, ConstructorInfo, MethodInfo,
    // PropertyInfo, FieldInfo, and EventInfo.

    Type type;
    
    // The string type is not defined in this assembly. Nore are its constructors,
    // methods, or properties.
    type = typeof(string);
    type.Assembly.FullName.Dump();

    type.IsDefinedInThisAssembly().Dump("Type");
    type.GetConstructors().First().IsDefinedInThisAssembly().Dump("Constructor");
    type.GetMethods().First().IsDefinedInThisAssembly().Dump("Method");
    type.GetProperties().First().IsDefinedInThisAssembly().Dump("Property");
    
    // The Foo type *is* defined in this assembly. And so are its constructors,
    // methods, and properties.
    type = typeof(Foo);
    type.Assembly.FullName.Dump();
    
    type.IsDefinedInThisAssembly().Dump("Type");
    type.GetConstructors().First().IsDefinedInThisAssembly().Dump("Constructor");
    type.GetMethods().First().IsDefinedInThisAssembly().Dump("Method");
    type.GetProperties().First().IsDefinedInThisAssembly().Dump("Property");
}

public class Foo
{
    public void Bar()
    {
    }
    
    public int Baz { get; set; }
}