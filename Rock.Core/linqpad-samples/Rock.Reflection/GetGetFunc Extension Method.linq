<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

void Main()
{
    // The GetGetFunc extension method creates an optimized "accessor function" for a
    // PropertyInfo. Using the resulting function eliminates almost all of the perfomance
    // penality that accompanies reflection calls.
    
    // This is the property that we want an "accessor function" for.
    PropertyInfo barProperty = typeof(Foo).GetProperty("Bar");
    
    // Get the optimized "accessor function".
    Func<object, object> getBarValue = barProperty.GetGetFunc();
    
    // This is the object whose Bar property will be accessed.
    Foo foo = new Foo { Bar = "abc" };
    
    // See how long it takes for the optimized accessor function.
    Stopwatch stopwatch = Stopwatch.StartNew();
    
    for (int i = 0; i < _iterations; i++)
    {
        getBarValue(foo);
    }
    
    stopwatch.Stop();    
    stopwatch.Elapsed.Dump("Optimized GetFunc");
    
    // See how long it takes to access the value via PropertyInfo.GetValue.
    stopwatch.Restart();
    
    for (int i = 0; i < _iterations; i++)
    {
        barProperty.GetValue(foo);
    }

    stopwatch.Stop();    
    stopwatch.Elapsed.Dump("Unoptimized PropertyInfo.GetValue");
}

private const int _iterations = 10000000;
    
public class Foo
{
    public string Bar { get; set; }
}