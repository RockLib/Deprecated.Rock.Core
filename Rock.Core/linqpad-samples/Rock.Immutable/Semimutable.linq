<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Immutable</Namespace>
</Query>

void Main()
{
    // The Semimutable class represents a "semi-mutable" value. The value can be changed
    // up to the point at which its value is read. Then the value is "locked" and will
    // not change again.
    
    // It's like Schrödinger's cat - once you open the box, the cat's fate is sealed.
    // Likewise, once you read the value of a Semimutable object, that value is locked.
    
    // The default value - to be used if nothing sets the value - is specified via
    // the constructor.
    Semimutable<int> semimutable1 = new Semimutable<int>(-1);
    semimutable1.Value.Dump("Default value specified");
    
    // If no default value is passed to the constructor, the default value of the generic
    // type is used. In the case if int, 0 is the default value.
    Semimutable<int> semimutable2 = new Semimutable<int>();
    semimutable2.Value.Dump("Default value not specified");
    
    // The value can be changed up until the point that the value is read.
    Semimutable<int> semimutable3 = new Semimutable<int>(-1);
    semimutable3.Value = 1;
    semimutable3.Value.Dump("Value set to 1");
    
    // Setting the value here has no effect because the value has already been read.
    semimutable3.Value = 2;
    semimutable3.Value.Dump("Value set to 2 after reading Value");
}