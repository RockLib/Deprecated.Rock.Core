<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Collections</Namespace>
</Query>

void Main()
{
    // The AsKeyed extension method converts an IEnumerable<TItem> to an IKeyedEnumerable<TKey, TItem>.
    // An IKeyedEnumerable<TKey, TItem> allows you to enumerate its items just like IEnumerable<TItem>
    // (because it inherits from IEnumerable<TItem>). It also allow you to access an item by a key.
    
    // Start with a collection of data objects. The Id property of the Foo class will be its key.
    var foos = new[]
    {
        new Foo { Id = 1, Bar = "A" },
        new Foo { Id = 2, Bar = "B" },
        new Foo { Id = 3, Bar = "C" }
    };
    
    // The lambda expression, 'f => f.Id', specifies that the Id property of the Foo class should be the key for the collection.
    var keyedFoos = foos.AsKeyed(f => f.Id);
    
    // Access an item by its key.
    var foo2 = keyedFoos[2];
    foo2.Dump("Item with key of 2");
    
    // Enumerate the collection, just as you would an IEnumerable<TItem>
    foreach (var foo in keyedFoos)
    {
        foo.Bar.Dump();
    }
}

public class Foo
{
    public int Id { get; set; }
    public string Bar { get; set; }
}