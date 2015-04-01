<Query Kind="Statements">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Collections</Namespace>
</Query>

// The AddSafely extension method operates on instances of Dictionary<TKey, TValye>.
// When called, if the key is not present in the dictionary, then a new item is added.
// If the key is present in the dictionary, no item is added.

var dictionary = new Dictionary<int, string>
{
    { 1, "One" }
};

// The value for 2 will be "Two", because it is a new key.
dictionary.AddSafely(2, "Two");

// The value for 1 will not be changed because 1 already exists as a key.
dictionary.AddSafely(1, "Uno");

dictionary.Dump();