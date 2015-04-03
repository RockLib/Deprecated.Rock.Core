<Query Kind="Program">
  <NuGetReference>Rock.Core</NuGetReference>
  <Namespace>Rock.Serialization</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

void Main()
{
    var serializer = new XmlSerializer(typeof(Foo));
    
    // LINQPad makes it a little difficult to get an assembly-qualified
    // name, so get it from the type directly.
    var barType = typeof(Bar).AssemblyQualifiedName;
    
    string xml = string.Format(
@"<Foo>
    <Bar type='{0}'>
        <Baz>abcd</Baz>
        <Qux>123</Qux>
    </Bar>
    <Grault>-1.1</Grault>
</Foo>", barType);
    
    Foo foo;
    
    // Use the BCL XmlSerializer to deserialize the Foo object from XML.
    using (var reader = new StringReader(xml))
    {
        foo = (Foo)serializer.Deserialize(reader);
    }
    
    // To get the instance of IBar, call CreateInstance on the proxy.
    var bar = foo.Bar.CreateInstance();
    
    foo.Dump();
    bar.Dump();
}

public class Foo
{
    public XmlDeserializationProxy<IBar> Bar { get; set; }
    public double Grault { get; set; }
}

public interface IBar
{
    string Baz { get; }
    int Qux { get; }
}

public class Bar : IBar
{
    private readonly string _baz;

    public Bar(string baz)
    {
        _baz = baz;
    }
    
    public string Baz { get { return _baz;  } }
    public int Qux { get; set; }
}