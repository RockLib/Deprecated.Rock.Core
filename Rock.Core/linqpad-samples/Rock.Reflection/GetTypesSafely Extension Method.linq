<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

#define NONEST
void Main()
{
    // Sometimes*, attempting to access the types of an assembly results in a
    // ReflectionTypeLoadException. The GetTypesSafely extension method operates
    // on an Assembly object, and gracefully handles the ReflectionTypeLoadException,
    // returning only the successfully loaded Types.
    //
    // *Specifically in LINQPad.
    
    AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypesSafely())
        .Where(t => t.Name.EndsWith("Foo")) // The two classes defined below end with "Foo"
        .Dump("GetTypesSafely");
    
    try
    {
        // A call to the LINQPad assembly's GetTypes method results in a
        // ReflectionTypeLoadException. If you inspect the excption object,
        // you'll find a Types property - those are the successfully loaded
        // types (with a few null items thrown in, which are filtered out).
        
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name.EndsWith("Foo"))
            .Dump("GetTypes");
    }
    catch (ReflectionTypeLoadException ex)
    {
        ex.Dump("GetTypes");
    }
}

public class Foo
{
}

public class AnotherFoo
{
}