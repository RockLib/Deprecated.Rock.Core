<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Reflection</Namespace>
</Query>

void Main()
{
    // The GetClosedGenericType inspects a type, checking to see if it implements a
    // specified open generic type. If it does, then the closed generic type that it
    // implements is returned.

    // The List<int> class implements IEnumerable<int>.
    typeof(List<int>).GetClosedGenericType(typeof(IEnumerable<>)).Dump("List<int>");
    
    // The MyEnumerable class implements IEnumerable<Guid>.
    typeof(MyEnumerable).GetClosedGenericType(typeof(IEnumerable<>)).Dump("MyEnumerable");
    
    // The ICollection<DateTime> interface inherits IEnumerable<DateTime>.
    typeof(ICollection<DateTime>).GetClosedGenericType(typeof(IEnumerable<>)).Dump("ICollection<DateTime>");
    
    // The Dictionary<string, double> class implements IEnumerable<KeyValuePair<string, double>>.
    typeof(Dictionary<string, double>).GetClosedGenericType(typeof(IEnumerable<>)).Dump("Dictionary<string, double>");
    
    // The string class implements IEnumerable<char>.
    typeof(string).GetClosedGenericType(typeof(IEnumerable<>)).Dump("string");
    
    // ArrayList does not implement generic IEnumerable<>.
    typeof(ArrayList).GetClosedGenericType(typeof(IEnumerable<>)).Dump("ArrayList");
}

private class MyEnumerable : IEnumerable<Guid>
{
    public IEnumerator<Guid> GetEnumerator() { throw new NotImplementedException(); }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}